using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public MainForm()
        {
            InitializeComponent();
            InitializeDataStrategies();
        }

        /// <summary>
        /// Inicializa las estrategias para cada tipo de dato admitido:
        /// Enteros, Flotantes, Letras, Palabras y Objetos Personalizados.
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

            _strategies.Add(new DataStrategy<char>(
                "Letras (char)",
                count => FileHandler.GenerateRandomData<char>(count)
            ));

            _strategies.Add(new DataStrategy<string>(
                "Palabras (string)",
                count => FileHandler.GenerateRandomData<string>(count)
            ));

            _strategies.Add(new DataStrategy<CustomItem>(
                "Objeto Personalizado (CustomItem)",
                count => FileHandler.GenerateRandomData<CustomItem>(count, 1, 1000)
            ));

            // Configurar el ComboBox con las estrategias
            cmbDataType.DisplayMember = nameof(IDataStrategy.DisplayName);
            cmbDataType.DataSource = _strategies;
            cmbDataType.SelectedIndex = 0;
        }

        /// <summary>
        /// Evento al cambiar de tipo de dato en el ComboBox.
        /// Refresca el ListBox con los datos que tenga la estrategia actual.
        /// </summary>
        private void cmbDataType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null) return;

            UpdateDisplay(CurrentStrategy.GetItems());
            UpdateTraceDisplay(CurrentStrategy.LastMetrics?.TraceSteps);

            if (CurrentStrategy.LastMetrics != null)
            {
                var m = CurrentStrategy.LastMetrics;
                lblTraceSummary.Text = $"Comparaciones: {m.ComparisonCount:N0} | Movimientos: {m.MovementCount:N0} | Tiempo: {m.ElapsedMilliseconds} ms";
            }
            else
            {
                lblTraceSummary.Text = "Ejecuta TimSort para registrar comparaciones y movimientos.";
            }

            lblStatus.Text = CurrentStrategy.HasData
                ? $"Tipo actual: {CurrentStrategy.DisplayName} ({CurrentStrategy.Count} elementos en memoria)."
                : $"Seleccionado: {CurrentStrategy.DisplayName}. Presione 'Generar Datos' o 'Importar Archivo'.";
        }

        /// <summary>
        /// Botón para generar los elementos automáticamente según la cantidad solicitada.
        /// </summary>
        private void btnGenerate_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null)
            {
                MessageBox.Show("Por favor, seleccione un tipo de dato en la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int count = (int)numElementCount.Value;
                Cursor = Cursors.WaitCursor;

                CurrentStrategy.Generate(count);
                UpdateDisplay(CurrentStrategy.GetItems());
                UpdateTraceDisplay(null);
                lblTraceSummary.Text = "Datos generados. Presione 'Ordenar con TimSort' para analizar.";

                lblStatus.Text = $"Generados {CurrentStrategy.Count} elementos correctamente ({CurrentStrategy.DisplayName}).";
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

        /// <summary>
        /// Botón para importar datos desde un archivo (JSON, XML, TXT o CSV).
        /// </summary>
        private void btnLoadFile_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null)
            {
                MessageBox.Show("Por favor, seleccione el tipo de dato que espera importar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var ofd = new OpenFileDialog
            {
                Title = $"Importar datos para {CurrentStrategy.DisplayName}",
                Filter = "Todos los formatos compatibles (*.json;*.xml;*.txt;*.csv)|*.json;*.xml;*.txt;*.csv|" +
                         "Archivos JSON (*.json)|*.json|" +
                         "Archivos XML (*.xml)|*.xml|" +
                         "Archivos de Texto (*.txt)|*.txt|" +
                         "Archivos CSV (*.csv)|*.csv|" +
                         "Todos los archivos (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    CurrentStrategy.Load(ofd.FileName);
                    UpdateDisplay(CurrentStrategy.GetItems());
                    UpdateTraceDisplay(null);
                    lblTraceSummary.Text = "Datos importados. Presione 'Ordenar con TimSort' para analizar.";

                    lblStatus.Text = $"¡Importados exitosamente {CurrentStrategy.Count} elementos desde '{Path.GetFileName(ofd.FileName)}'!";
                    MessageBox.Show($"Se importaron {CurrentStrategy.Count} elementos correctamente como '{CurrentStrategy.DisplayName}'.\nYa puedes ordenarlos con TimSort.",
                        "Importación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo importar el archivo:\n{ex.Message}\n\nVerifica que el archivo contenga datos compatibles con '{CurrentStrategy.DisplayName}'.",
                        "Error de Importación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// Botón para ejecutar el ordenamiento TimSort (ascendente o descendente)
        /// registrando la cantidad exacta de comparaciones y movimientos.
        /// </summary>
        private void btnSort_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy is null || !CurrentStrategy.HasData)
            {
                MessageBox.Show("No hay datos cargados o generados para ordenar. Presione 'Generar Datos' o 'Importar Archivo'.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool ascending = rbAscending.Checked;
                bool recordTrace = chkRecordTrace.Checked;
                Cursor = Cursors.WaitCursor;

                // Ejecutar ordenamiento con registro de métricas y pasos (límite de 1000 pasos en UI)
                var metrics = CurrentStrategy.Sort(ascending, recordTrace, maxTraceSteps: 1000);

                // Refrescar resultados ordenados
                UpdateDisplay(CurrentStrategy.GetItems());

                // Refrescar vista de trazabilidad
                UpdateTraceDisplay(metrics.TraceSteps);

                string direccion = ascending ? "Ascendente" : "Descendente";
                lblTraceSummary.Text = $"Total Comparaciones: {metrics.ComparisonCount:N0} | Total Movimientos/Swaps: {metrics.MovementCount:N0} | MinRun: {metrics.MinRun} | Tiempo: {metrics.ElapsedMilliseconds} ms ({metrics.ElapsedTicks:N0} ticks)";
                lblStatus.Text = $"¡Ordenado ({direccion})! {metrics.ComparisonCount:N0} comp., {metrics.MovementCount:N0} mov., {metrics.ElapsedMilliseconds} ms.";

                // Si el usuario activó la traza, podemos opcionalmente enfocar la pestaña o dejarla disponible
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

        /// <summary>
        /// Botón para exportar la traza textual completa de pasos a un archivo de texto o log.
        /// </summary>
        private void btnSaveTrace_Click(object? sender, EventArgs e)
        {
            if (CurrentStrategy?.LastMetrics == null || CurrentStrategy.LastMetrics.TraceSteps.Count == 0)
            {
                MessageBox.Show("Aún no hay pasos registrados para exportar. Ejecute primero el ordenamiento con la opción 'Registrar traza' activada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Guardar Traza de Operaciones TimSort",
                Filter = "Archivo de Registro (*.log)|*.log|Archivo de Texto (*.txt)|*.txt",
                FileName = $"traza_timsort_{CurrentStrategy.DisplayName.Split(' ')[0].ToLower()}_{DateTime.Now:yyyyMMdd_HHmmss}.log"
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
                    sb.AppendLine("================================================================================");
                    sb.AppendLine("DETALLE PASO A PASO:");
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

        /// <summary>
        /// Botón para guardar los elementos actuales en archivo (JSON, XML, TXT o CSV).
        /// </summary>
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
                Filter = "Archivo JSON (*.json)|*.json|" +
                         "Archivo XML (*.xml)|*.xml|" +
                         "Archivo de Texto (*.txt)|*.txt|" +
                         "Archivo CSV (*.csv)|*.csv",
                FileName = $"datos_{CurrentStrategy.DisplayName.Split(' ')[0].ToLower()}"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    CurrentStrategy.Save(sfd.FileName);
                    MessageBox.Show("¡Archivo guardado con éxito!", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblStatus.Text = $"Archivo guardado: {Path.GetFileName(sfd.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Actualiza la visualización del ListBox de datos ordenados.
        /// </summary>
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

        /// <summary>
        /// Actualiza la lista de pasos y trazabilidad de comparaciones.
        /// </summary>
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

        private void lblStatus_Click(object sender, EventArgs e)
        {
             
        }
    }
}
