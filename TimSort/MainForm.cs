using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimSort.Models;
using TimSort.Services;

namespace TimSort
{
    public partial class MainForm : Form
    {
        // Lista de estrategias para manejar cada tipo de dato de forma polimórfica (Patrón Strategy)
        private readonly List<IDataStrategy> _strategies = new();

        // Estrategia actualmente seleccionada en el ComboBox
        private IDataStrategy? CurrentStrategy => cmbDataType.SelectedItem as IDataStrategy;

        // Variables de estado para el visualizador gráfico en tiempo real
        private CancellationTokenSource? _visualCts;
        private int _visualCompareIdx1 = -1;
        private int _visualCompareIdx2 = -1;
        private int _visualMoveIdx1 = -1;
        private int _visualMoveIdx2 = -1;
        private bool _isFullySorted = false;
        private int _sortedSweepIndex = -1;

        public MainForm()
        {
            InitializeComponent();
            InitializeDataStrategies();
            cmbMatrixViewMode.SelectedIndex = 0;
        }

        /// <summary>
        /// Inicializa las estrategias para cada tipo de dato:
        /// Enteros, Flotantes, Letras, Palabras (con comparadores normalizados) y Objeto Personalizado.
        /// </summary>
        private void InitializeDataStrategies()
        {
            _strategies.Add(new DataStrategy<int>(
                "Enteros (int)",
                count => FileHandler.GenerateRandomData<int>(count, 1, 1000)
            ));

            _strategies.Add(new DataStrategy<double>(
                "Flotantes (double)",
                count => FileHandler.GenerateRandomData<double>(count, 1, 1000)
            ));

            // Letras con normalización Unicode de mayúsculas/minúsculas y acentos/diéresis (ej. u, U, ú, ü)
            _strategies.Add(new DataStrategy<char>(
                "Letras (char - Normalizadas)",
                count => FileHandler.GenerateRandomData<char>(count),
                NormalizedCharComparer.Instance
            ));

            // Palabras con normalización de mayúsculas/minúsculas y acentos
            _strategies.Add(new DataStrategy<string>(
                "Palabras (string - Normalizadas)",
                count => FileHandler.GenerateRandomData<string>(count),
                NormalizedStringComparer.Instance
            ));

            _strategies.Add(new DataStrategy<CustomItem>(
                "Objeto Personalizado (CustomItem)",
                count => FileHandler.GenerateRandomData<CustomItem>(count, 1, 1000)
            ));

            cmbDataType.DisplayMember = nameof(IDataStrategy.DisplayName);
            cmbDataType.DataSource = _strategies;
            cmbDataType.SelectedIndex = 0;
        }

        #region Eventos de Controles de Datos

        private void cmbDataType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null) return;

            ResetVisualizerState();
            UpdateDisplay(CurrentStrategy.GetItems());
            PopulateMatrix();
            PopulateMovements(CurrentStrategy.LastMetrics?.MovementsLog);
            UpdateTraceDisplay(CurrentStrategy.LastMetrics?.TraceSteps);

            if (CurrentStrategy.LastMetrics != null)
            {
                UpdateMetrics(CurrentStrategy.LastMetrics);
            }
            else
            {
                ResetMetricsDisplay();
            }

            statusLabel.Text = CurrentStrategy.HasData
                ? $"Tipo actual: {CurrentStrategy.DisplayName} ({CurrentStrategy.Count} elementos en memoria)."
                : $"Seleccionado: {CurrentStrategy.DisplayName}. Genere o importe datos para comenzar.";

            pnlVisualizer.Invalidate();
        }

        private void btnGenerate_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null) return;

            try
            {
                int count = (int)numElementCount.Value;
                Cursor = Cursors.WaitCursor;

                CurrentStrategy.Generate(count);
                ResetVisualizerState();
                UpdateDisplay(CurrentStrategy.GetItems());
                PopulateMatrix();
                PopulateMovements(null);
                UpdateTraceDisplay(null);
                ResetMetricsDisplay();

                statusLabel.Text = $"Generados {CurrentStrategy.Count} elementos correctamente ({CurrentStrategy.DisplayName}).";
                lblVisualizerStatus.Text = $"Arreglo generado con {CurrentStrategy.Count} elementos. Listo para ordenar.";
                pnlVisualizer.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnLoadFile_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null) return;

            using var ofd = new OpenFileDialog
            {
                Title = $"Importar datos para {CurrentStrategy.DisplayName}",
                Filter = "Todos los compatibles (*.json;*.xml;*.txt;*.csv)|*.json;*.xml;*.txt;*.csv|" +
                         "Archivos de Texto (*.txt)|*.txt|" +
                         "Archivos CSV (*.csv)|*.csv|" +
                         "Archivos JSON (*.json)|*.json|" +
                         "Archivos XML (*.xml)|*.xml|" +
                         "Todos los archivos (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    CurrentStrategy.Load(ofd.FileName);
                    numElementCount.Value = Math.Clamp(CurrentStrategy.Count, numElementCount.Minimum, numElementCount.Maximum);

                    ResetVisualizerState();
                    UpdateDisplay(CurrentStrategy.GetItems());
                    PopulateMatrix();
                    PopulateMovements(null);
                    UpdateTraceDisplay(null);
                    ResetMetricsDisplay();

                    statusLabel.Text = $"¡Importados {CurrentStrategy.Count} elementos desde '{Path.GetFileName(ofd.FileName)}'!";
                    lblVisualizerStatus.Text = $"Archivo '{Path.GetFileName(ofd.FileName)}' cargado ({CurrentStrategy.Count} elementos).";
                    MessageBox.Show($"Se importaron {CurrentStrategy.Count} elementos con éxito como '{CurrentStrategy.DisplayName}'.",
                        "Importación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    pnlVisualizer.Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo importar el archivo:\n{ex.Message}", "Error de Importación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void btnSaveFile_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null || !CurrentStrategy.HasData)
            {
                MessageBox.Show("No hay datos para guardar. Genere o importe datos primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Guardar datos ordenados o importados",
                Filter = "Archivo de Texto (*.txt)|*.txt|" +
                         "Archivo CSV (*.csv)|*.csv|" +
                         "Archivo JSON (*.json)|*.json|" +
                         "Archivo XML (*.xml)|*.xml",
                FileName = $"datos_{CurrentStrategy.DisplayName.Split(' ')[0].ToLower()}"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    CurrentStrategy.Save(sfd.FileName);
                    MessageBox.Show("¡Archivo guardado con éxito!", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    statusLabel.Text = $"Archivo guardado: {Path.GetFileName(sfd.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Ordenamiento Instantáneo (Directo)

        private void btnSort_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null || !CurrentStrategy.HasData)
            {
                MessageBox.Show("No hay datos cargados o generados para ordenar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool ascending = rbAscending.Checked;
                bool recordTrace = chkRecordTrace.Checked;
                Cursor = Cursors.WaitCursor;

                var metrics = CurrentStrategy.Sort(ascending, recordTrace, maxTraceSteps: 2000);

                // Estado visual ordenado
                _isFullySorted = true;
                _sortedSweepIndex = CurrentStrategy.Count;
                _visualCompareIdx1 = -1;
                _visualCompareIdx2 = -1;
                _visualMoveIdx1 = -1;
                _visualMoveIdx2 = -1;

                UpdateDisplay(CurrentStrategy.GetItems());
                UpdateMetrics(metrics);
                PopulateMatrix();
                PopulateMovements(metrics.MovementsLog);
                UpdateTraceDisplay(metrics.TraceSteps);

                string dirText = ascending ? "Ascendente" : "Descendente";
                statusLabel.Text = $"¡Ordenado ({dirText}) en {metrics.ElapsedMilliseconds} ms! {metrics.ComparisonCount:N0} comp., {metrics.MovementCount:N0} mov.";
                lblVisualizerStatus.Text = $"Ordenado completado ({dirText}) en {metrics.ElapsedMilliseconds} ms. {metrics.VerificationMessage}";

                if (chkSound.Checked)
                {
                    SoundService.PlaySuccessChord();
                }

                pnlVisualizer.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el ordenamiento: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Visualizador Gráfico en Tiempo Real con Animación y Sonido

        private async void btnLiveVisualSort_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null || !CurrentStrategy.HasData)
            {
                MessageBox.Show("No hay datos cargados o generados para ordenar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cambiar a la pestaña del visualizador
            tabControlMain.SelectedTab = tabVisualizer;

            SetExecutionState(isRunning: true);
            _visualCts = new CancellationTokenSource();
            ResetVisualizerState();

            bool ascending = rbAscending.Checked;
            int delayMs = tbSpeed.Value;
            var token = _visualCts.Token;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var metrics = await CurrentStrategy.SortVisualAsync(
                    ascending,
                    async update =>
                    {
                        if (token.IsCancellationRequested) return;

                        // Actualizar índices visuales
                        if (update.Type == VisualUpdateType.Compare)
                        {
                            _visualCompareIdx1 = update.Index1;
                            _visualCompareIdx2 = update.Index2;
                            _visualMoveIdx1 = -1;
                            _visualMoveIdx2 = -1;
                        }
                        else if (update.Type == VisualUpdateType.Move || update.Type == VisualUpdateType.Write)
                        {
                            _visualMoveIdx1 = update.Index1;
                            _visualMoveIdx2 = update.Index2;
                            _visualCompareIdx1 = -1;
                            _visualCompareIdx2 = -1;

                            // Efecto de sonido corto según el valor del elemento modificado
                            if (chkSound.Checked && update.Index1 >= 0)
                            {
                                double norm = CurrentStrategy.GetNormalizedHeight(update.Index1);
                                SoundService.PlayFrequency(norm);
                            }
                        }

                        // Actualizar encabezado del visualizador y métricas en vivo
                        lblVisualizerStatus.Text = $"{update.Description} | Comparaciones: {update.CurrentComparisons:N0} | Movimientos: {update.CurrentMovements:N0}";
                        lblMetricComparisons.Text = $"Comparaciones: {update.CurrentComparisons:N0}";
                        lblMetricMovements.Text = $"Movimientos (Swaps): {update.CurrentMovements:N0}";

                        pnlVisualizer.Invalidate();

                        if (delayMs > 0)
                        {
                            await Task.Delay(delayMs, token);
                        }
                    },
                    token
                );

                stopwatch.Stop();
                metrics.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                // Animación de barrido final verde (Green Sweep)
                await AnimateSortedSweep(token);

                _isFullySorted = true;
                pnlVisualizer.Invalidate();

                UpdateDisplay(CurrentStrategy.GetItems());
                UpdateMetrics(metrics);
                PopulateMatrix();
                PopulateMovements(metrics.MovementsLog);
                UpdateTraceDisplay(metrics.TraceSteps);

                if (chkSound.Checked)
                {
                    SoundService.PlaySuccessChord();
                }

                lblVisualizerStatus.Text = $"¡Ordenamiento finalizado con éxito! {metrics.VerificationMessage}";
                statusLabel.Text = $"¡Visualización completada en {metrics.ElapsedMilliseconds} ms!";
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                lblVisualizerStatus.Text = "⏹ Animación detenida por el usuario.";
                statusLabel.Text = "Visualización cancelada.";
                ResetVisualizerState();
                UpdateDisplay(CurrentStrategy.GetItems());
                pnlVisualizer.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en el visualizador: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetExecutionState(isRunning: false);
                _visualCts?.Dispose();
                _visualCts = null;
            }
        }

        private void btnStopVisualSort_Click(object? sender, EventArgs e)
        {
            _visualCts?.Cancel();
            btnStopVisualSort.Enabled = false;
        }

        private async Task AnimateSortedSweep(CancellationToken token)
        {
            if (CurrentStrategy == null) return;
            int count = CurrentStrategy.Count;
            int step = Math.Max(1, count / 40);

            for (int i = 0; i <= count; i += step)
            {
                if (token.IsCancellationRequested) break;
                _sortedSweepIndex = i;
                pnlVisualizer.Invalidate();
                await Task.Delay(10, token);
            }
            _sortedSweepIndex = count;
        }

        private void tbSpeed_Scroll(object? sender, EventArgs e)
        {
            lblSpeed.Text = $"Velocidad animación: {tbSpeed.Value} ms";
        }

        private void SetExecutionState(bool isRunning)
        {
            btnSort.Enabled = !isRunning;
            btnLiveVisualSort.Enabled = !isRunning;
            btnStopVisualSort.Enabled = isRunning;
            btnGenerate.Enabled = !isRunning;
            btnLoadFile.Enabled = !isRunning;
            btnSaveFile.Enabled = !isRunning;
            cmbDataType.Enabled = !isRunning;
            numElementCount.Enabled = !isRunning;
            rbAscending.Enabled = !isRunning;
            rbDescending.Enabled = !isRunning;
        }

        private void ResetVisualizerState()
        {
            _visualCompareIdx1 = -1;
            _visualCompareIdx2 = -1;
            _visualMoveIdx1 = -1;
            _visualMoveIdx2 = -1;
            _isFullySorted = false;
            _sortedSweepIndex = -1;
        }

        #endregion

        #region Renderizado del Visualizador Gráfico (Gráfica de Barras)

        private void pnlVisualizer_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            int w = pnlVisualizer.ClientSize.Width;
            int h = pnlVisualizer.ClientSize.Height;

            if (CurrentStrategy is null || CurrentStrategy.Count == 0)
            {
                using var font = new Font("Segoe UI", 12F, FontStyle.Regular);
                using var brush = new SolidBrush(Color.DarkGray);
                string msg = "No hay datos para mostrar. Genere o importe datos primero.";
                var size = g.MeasureString(msg, font);
                g.DrawString(msg, font, brush, (w - size.Width) / 2, (h - size.Height) / 2);
                return;
            }

            int count = CurrentStrategy.Count;
            float barWidth = (float)w / count;
            var items = CurrentStrategy.GetItems();

            using var defaultBrush = new SolidBrush(Color.FromArgb(41, 128, 185)); // Azul brillante
            using var compareBrush = new SolidBrush(Color.FromArgb(231, 76, 60));   // Rojo Carmesí (Comparación)
            using var moveBrush = new SolidBrush(Color.FromArgb(243, 156, 18));    // Naranja (Movimiento / Swap)
            using var sortedBrush = new SolidBrush(Color.FromArgb(46, 204, 113));  // Verde Esmeralda (Ordenado)
            using var textBrush = new SolidBrush(Color.White);
            using var fontText = new Font("Segoe UI", count <= 25 ? 8F : 6.5F, FontStyle.Bold);

            int bottomMargin = count <= 50 ? 25 : 6;
            int usableHeight = h - bottomMargin - 15;

            for (int i = 0; i < count; i++)
            {
                double norm = CurrentStrategy.GetNormalizedHeight(i);
                float barH = Math.Max(4, (float)(norm * usableHeight));
                float x = i * barWidth;
                float y = h - bottomMargin - barH;

                Brush brushToUse = defaultBrush;

                if (_isFullySorted || i < _sortedSweepIndex)
                {
                    brushToUse = sortedBrush;
                }
                else if (i == _visualMoveIdx1 || i == _visualMoveIdx2)
                {
                    brushToUse = moveBrush;
                }
                else if (i == _visualCompareIdx1 || i == _visualCompareIdx2)
                {
                    brushToUse = compareBrush;
                }

                float drawW = Math.Max(1f, barWidth - (count > 150 ? 0 : 1f));
                g.FillRectangle(brushToUse, x, y, drawW, barH);

                // Si hay pocos elementos, mostrar etiquetas textuales legibles
                if (count <= 50)
                {
                    string label = items[i]?.ToString() ?? "";
                    if (label.Length > 8) label = label[..7] + "..";
                    var size = g.MeasureString(label, fontText);
                    float textX = x + (drawW - size.Width) / 2;
                    g.DrawString(label, fontText, textBrush, textX, h - bottomMargin + 4);
                }
            }
        }

        #endregion

        #region Matriz Horizontal por Filas (DataGridView de Pasos)

        private void cmbMatrixViewMode_SelectedIndexChanged(object? sender, EventArgs e)
        {
            PopulateMatrix();
        }

        /// <summary>
        /// Llena la matriz horizontal DataGridView según el modo seleccionado:
        /// 1. Evolución por pasadas / hitos (runs y merges), resaltando celdas modificadas en oro/salmón.
        /// 2. Bloques horizontales de 40 o 50 elementos por fila para el arreglo actual.
        /// </summary>
        private void PopulateMatrix()
        {
            if (CurrentStrategy is null || CurrentStrategy.Count == 0)
            {
                dgvMatrix.Columns.Clear();
                dgvMatrix.Rows.Clear();
                return;
            }

            dgvMatrix.SuspendLayout();
            try
            {
                dgvMatrix.Columns.Clear();
                dgvMatrix.Rows.Clear();

                string mode = cmbMatrixViewMode.SelectedItem?.ToString() ?? "";

                if (mode.Contains("Bloques"))
                {
                    int blockSize = mode.Contains("40") ? 40 : 50;
                    var items = CurrentStrategy.GetItems();
                    int total = items.Count;

                    for (int c = 0; c < blockSize; c++)
                    {
                        dgvMatrix.Columns.Add($"Col_{c}", $"[{c}]");
                        dgvMatrix.Columns[c].Width = 55;
                    }

                    int numRows = (int)Math.Ceiling((double)total / blockSize);
                    for (int r = 0; r < numRows; r++)
                    {
                        int start = r * blockSize;
                        int end = Math.Min(start + blockSize, total);
                        int rowIndex = dgvMatrix.Rows.Add();
                        dgvMatrix.Rows[rowIndex].HeaderCell.Value = $"Bloque [{start}..{end - 1}]";

                        for (int c = 0; c < (end - start); c++)
                        {
                            dgvMatrix.Rows[rowIndex].Cells[c].Value = items[start + c]?.ToString();
                        }
                    }
                }
                else
                {
                    // Modo Evolución por Pasadas / Hitos del Algoritmo
                    var metrics = CurrentStrategy.LastMetrics;
                    if (metrics == null || metrics.Snapshots.Count == 0)
                    {
                        dgvMatrix.Columns.Add("Info", "Información");
                        dgvMatrix.Columns[0].Width = 500;
                        int r = dgvMatrix.Rows.Add();
                        dgvMatrix.Rows[r].Cells[0].Value = "Ejecute TimSort ('Ordenar Directo' o 'Visualizar') para registrar las pasadas y cambios.";
                        return;
                    }

                    int maxCols = Math.Min(50, CurrentStrategy.Count);
                    for (int c = 0; c < maxCols; c++)
                    {
                        dgvMatrix.Columns.Add($"Col_{c}", $"Pos {c}");
                        dgvMatrix.Columns[c].Width = 60;
                    }

                    foreach (var snapshot in metrics.Snapshots)
                    {
                        int rowIndex = dgvMatrix.Rows.Add();
                        dgvMatrix.Rows[rowIndex].HeaderCell.Value = snapshot.Phase;

                        for (int c = 0; c < maxCols; c++)
                        {
                            if (c < snapshot.Data.Length)
                            {
                                var cell = dgvMatrix.Rows[rowIndex].Cells[c];
                                cell.Value = snapshot.Data[c]?.ToString();

                                // Resaltar celdas modificadas en esta pasada
                                if (snapshot.ChangedIndices.Contains(c))
                                {
                                    cell.Style.BackColor = Color.Gold;
                                    cell.Style.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
                                    cell.ToolTipText = $"Modificado en: {snapshot.Phase}";
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                dgvMatrix.ResumeLayout();
            }
        }

        #endregion

        #region Bitácora de Movimientos y Exportación

        private void PopulateMovements(IReadOnlyList<string>? movements)
        {
            listBoxMovements.BeginUpdate();
            try
            {
                listBoxMovements.DataSource = null;
                if (movements != null && movements.Count > 0)
                {
                    listBoxMovements.DataSource = movements.ToList();
                }
                else
                {
                    listBoxMovements.Items.Clear();
                }
            }
            finally
            {
                listBoxMovements.EndUpdate();
            }
        }

        private void btnExportMovements_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy?.LastMetrics == null || CurrentStrategy.LastMetrics.MovementsLog.Count == 0)
            {
                MessageBox.Show("No hay movimientos registrados para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Guardar Bitácora de Movimientos TimSort",
                Filter = "Archivo de Registro (*.log)|*.log|Archivo de Texto (*.txt)|*.txt",
                FileName = $"movimientos_timsort_{DateTime.Now:yyyyMMdd_HHmmss}.log"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("================================================================================");
                    sb.AppendLine("                     BITÁCORA DE MOVIMIENTOS - TIMSORT                          ");
                    sb.AppendLine("================================================================================");
                    sb.AppendLine($"Fecha/Hora:           {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sb.AppendLine($"Tipo de dato:         {CurrentStrategy.DisplayName}");
                    sb.AppendLine($"Cantidad elementos:   {CurrentStrategy.Count:N0}");
                    sb.AppendLine($"Total Movimientos:    {CurrentStrategy.LastMetrics.MovementCount:N0}");
                    sb.AppendLine($"Verificación:         {CurrentStrategy.LastMetrics.VerificationMessage}");
                    sb.AppendLine("================================================================================");
                    foreach (var move in CurrentStrategy.LastMetrics.MovementsLog)
                    {
                        sb.AppendLine(move);
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("¡Bitácora exportada exitosamente!", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSaveTrace_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy?.LastMetrics == null || CurrentStrategy.LastMetrics.TraceSteps.Count == 0)
            {
                MessageBox.Show("Aún no hay pasos registrados para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Guardar Traza de Operaciones TimSort",
                Filter = "Archivo de Registro (*.log)|*.log|Archivo de Texto (*.txt)|*.txt",
                FileName = $"traza_timsort_{DateTime.Now:yyyyMMdd_HHmmss}.log"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var m = CurrentStrategy.LastMetrics;
                    var sb = new StringBuilder();
                    sb.AppendLine("================================================================================");
                    sb.AppendLine("                   INFORME DE TRAZABILIDAD - TIMSORT                          ");
                    sb.AppendLine("================================================================================");
                    sb.AppendLine($"Fecha/Hora:           {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sb.AppendLine($"Tipo de dato:         {CurrentStrategy.DisplayName}");
                    sb.AppendLine($"Cantidad elementos:   {CurrentStrategy.Count:N0}");
                    sb.AppendLine($"Tamaño MinRun:        {m.MinRun}");
                    sb.AppendLine($"Total Comparaciones:  {m.ComparisonCount:N0}");
                    sb.AppendLine($"Total Movimientos:    {m.MovementCount:N0}");
                    sb.AppendLine($"Tiempo de Ejecución:  {m.ElapsedMilliseconds} ms ({m.ElapsedTicks:N0} ticks)");
                    sb.AppendLine($"Verificación:         {m.VerificationMessage}");
                    sb.AppendLine("================================================================================");
                    foreach (var step in m.TraceSteps)
                    {
                        sb.AppendLine(step);
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("¡Traza guardada exitosamente!", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al exportar la traza: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Métricas y Visualización General

        private void UpdateMetrics(SortMetrics metrics)
        {
            lblMetricTime.Text = $"Tiempo: {metrics.ElapsedMilliseconds} ms ({metrics.ElapsedTicks:N0} ticks)";
            lblMetricComparisons.Text = $"Comparaciones: {metrics.ComparisonCount:N0}";
            lblMetricMovements.Text = $"Movimientos (Swaps): {metrics.MovementCount:N0}";

            lblMetricVerification.Text = metrics.VerificationMessage;
            lblMetricVerification.ForeColor = metrics.IsSortedSuccessfully ? Color.ForestGreen : Color.Crimson;

            lblTraceSummary.Text = $"Comparaciones: {metrics.ComparisonCount:N0} | Movimientos: {metrics.MovementCount:N0} | MinRun: {metrics.MinRun} | Tiempo: {metrics.ElapsedMilliseconds} ms";
        }

        private void ResetMetricsDisplay()
        {
            lblMetricTime.Text = "Tiempo: 0 ms";
            lblMetricComparisons.Text = "Comparaciones: 0";
            lblMetricMovements.Text = "Movimientos (Swaps): 0";
            lblMetricVerification.Text = "Estado: Pendiente de ordenamiento.";
            lblMetricVerification.ForeColor = Color.DarkSlateGray;
            lblTraceSummary.Text = "Ejecute TimSort para registrar comparaciones y pasos.";
        }

        private void UpdateDisplay<T>(IEnumerable<T>? items)
        {
            listBoxResults.BeginUpdate();
            try
            {
                listBoxResults.DataSource = null;
                if (items != null)
                {
                    listBoxResults.DataSource = items.ToList();
                }
            }
            finally
            {
                listBoxResults.EndUpdate();
            }
        }

        private void UpdateTraceDisplay(IReadOnlyList<string>? steps)
        {
            listBoxTrace.BeginUpdate();
            try
            {
                listBoxTrace.DataSource = null;
                if (steps != null && steps.Count > 0)
                {
                    listBoxTrace.DataSource = steps.ToList();
                }
                else
                {
                    listBoxTrace.Items.Clear();
                }
            }
            finally
            {
                listBoxTrace.EndUpdate();
            }
        }

        #endregion

        private void chkSound_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
