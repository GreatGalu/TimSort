using System.Drawing;
using System.Windows.Forms;
using TimSort.Services;

namespace TimSort
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            pnlSidebar = new Panel();
            grpMetrics = new GroupBox();
            lblMetricVerification = new Label();
            lblMetricMovements = new Label();
            lblMetricComparisons = new Label();
            lblMetricTime = new Label();
            grpExecution = new GroupBox();
            lblSpeed = new Label();
            tbSpeed = new TrackBar();
            chkSound = new CheckBox();
            chkRecordTrace = new CheckBox();
            btnStopVisualSort = new Button();
            btnLiveVisualSort = new Button();
            btnSort = new Button();
            grpSortDirection = new GroupBox();
            rbDescending = new RadioButton();
            rbAscending = new RadioButton();
            grpDataConfig = new GroupBox();
            btnSaveFile = new Button();
            btnLoadFile = new Button();
            btnGenerate = new Button();
            numElementCount = new NumericUpDown();
            lblCount = new Label();
            cmbDataType = new ComboBox();
            lblDataType = new Label();
            pnlMainContainer = new Panel();
            statusStripMain = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            tabControlMain = new TabControl();
            tabVisualizer = new TabPage();
            pnlVisualizer = new DoubleBufferedPanel();
            pnlVisualizerHeader = new Panel();
            lblVisualizerStatus = new Label();
            tabMatrix = new TabPage();
            dgvMatrix = new DataGridView();
            pnlMatrixControls = new Panel();
            lblMatrixLegend = new Label();
            cmbMatrixViewMode = new ComboBox();
            lblMatrixMode = new Label();
            tabData = new TabPage();
            listBoxResults = new ListBox();
            tabMovements = new TabPage();
            listBoxMovements = new ListBox();
            pnlMovementsHeader = new Panel();
            btnExportMovements = new Button();
            lblMovementsTitle = new Label();
            tabTrace = new TabPage();
            listBoxTrace = new ListBox();
            panelTraceHeader = new Panel();
            btnSaveTrace = new Button();
            lblTraceSummary = new Label();
            pnlSidebar.SuspendLayout();
            grpMetrics.SuspendLayout();
            grpExecution.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbSpeed).BeginInit();
            grpSortDirection.SuspendLayout();
            grpDataConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numElementCount).BeginInit();
            pnlMainContainer.SuspendLayout();
            statusStripMain.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabVisualizer.SuspendLayout();
            pnlVisualizerHeader.SuspendLayout();
            tabMatrix.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMatrix).BeginInit();
            pnlMatrixControls.SuspendLayout();
            tabData.SuspendLayout();
            tabMovements.SuspendLayout();
            pnlMovementsHeader.SuspendLayout();
            tabTrace.SuspendLayout();
            panelTraceHeader.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSidebar
            // 
            pnlSidebar.AutoScroll = true;
            pnlSidebar.BackColor = SystemColors.Control;
            pnlSidebar.Controls.Add(grpMetrics);
            pnlSidebar.Controls.Add(grpExecution);
            pnlSidebar.Controls.Add(grpSortDirection);
            pnlSidebar.Controls.Add(grpDataConfig);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Padding = new Padding(10);
            pnlSidebar.Size = new Size(310, 681);
            pnlSidebar.TabIndex = 0;
            // 
            // grpMetrics
            // 
            grpMetrics.Controls.Add(lblMetricVerification);
            grpMetrics.Controls.Add(lblMetricMovements);
            grpMetrics.Controls.Add(lblMetricComparisons);
            grpMetrics.Controls.Add(lblMetricTime);
            grpMetrics.Dock = DockStyle.Top;
            grpMetrics.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpMetrics.Location = new Point(10, 520);
            grpMetrics.Name = "grpMetrics";
            grpMetrics.Size = new Size(290, 150);
            grpMetrics.TabIndex = 3;
            grpMetrics.TabStop = false;
            grpMetrics.Text = "📊 Métricas y Verificación";
            // 
            // lblMetricVerification
            // 
            lblMetricVerification.AutoEllipsis = true;
            lblMetricVerification.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblMetricVerification.ForeColor = Color.ForestGreen;
            lblMetricVerification.Location = new Point(12, 95);
            lblMetricVerification.Name = "lblMetricVerification";
            lblMetricVerification.Size = new Size(265, 45);
            lblMetricVerification.TabIndex = 3;
            lblMetricVerification.Text = "Estado: Pendiente de ordenamiento.";
            // 
            // lblMetricMovements
            // 
            lblMetricMovements.AutoSize = true;
            lblMetricMovements.Font = new Font("Segoe UI", 9F);
            lblMetricMovements.Location = new Point(12, 72);
            lblMetricMovements.Name = "lblMetricMovements";
            lblMetricMovements.Size = new Size(137, 15);
            lblMetricMovements.TabIndex = 2;
            lblMetricMovements.Text = "Movimientos (Swaps): 0";
            // 
            // lblMetricComparisons
            // 
            lblMetricComparisons.AutoSize = true;
            lblMetricComparisons.Font = new Font("Segoe UI", 9F);
            lblMetricComparisons.Location = new Point(12, 48);
            lblMetricComparisons.Name = "lblMetricComparisons";
            lblMetricComparisons.Size = new Size(106, 15);
            lblMetricComparisons.TabIndex = 1;
            lblMetricComparisons.Text = "Comparaciones: 0";
            // 
            // lblMetricTime
            // 
            lblMetricTime.AutoSize = true;
            lblMetricTime.Font = new Font("Segoe UI", 9F);
            lblMetricTime.Location = new Point(12, 24);
            lblMetricTime.Name = "lblMetricTime";
            lblMetricTime.Size = new Size(88, 15);
            lblMetricTime.TabIndex = 0;
            lblMetricTime.Text = "Tiempo: 0 ms";
            // 
            // grpExecution
            // 
            grpExecution.Controls.Add(lblSpeed);
            grpExecution.Controls.Add(tbSpeed);
            grpExecution.Controls.Add(chkSound);
            grpExecution.Controls.Add(chkRecordTrace);
            grpExecution.Controls.Add(btnStopVisualSort);
            grpExecution.Controls.Add(btnLiveVisualSort);
            grpExecution.Controls.Add(btnSort);
            grpExecution.Dock = DockStyle.Top;
            grpExecution.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpExecution.Location = new Point(10, 275);
            grpExecution.Name = "grpExecution";
            grpExecution.Size = new Size(290, 245);
            grpExecution.TabIndex = 2;
            grpExecution.TabStop = false;
            grpExecution.Text = "⚙ Ejecución de TimSort";
            // 
            // lblSpeed
            // 
            lblSpeed.AutoSize = true;
            lblSpeed.Font = new Font("Segoe UI", 8.25F);
            lblSpeed.Location = new Point(12, 175);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new Size(137, 13);
            lblSpeed.TabIndex = 6;
            lblSpeed.Text = "Velocidad animación: 20 ms";
            // 
            // tbSpeed
            // 
            tbSpeed.LargeChange = 20;
            tbSpeed.Location = new Point(10, 192);
            tbSpeed.Maximum = 150;
            tbSpeed.Minimum = 2;
            tbSpeed.Name = "tbSpeed";
            tbSpeed.Size = new Size(267, 45);
            tbSpeed.SmallChange = 5;
            tbSpeed.TabIndex = 5;
            tbSpeed.TickFrequency = 20;
            tbSpeed.Value = 20;
            tbSpeed.Scroll += tbSpeed_Scroll;
            // 
            // chkSound
            // 
            chkSound.AutoSize = true;
            chkSound.Checked = true;
            chkSound.CheckState = CheckState.Checked;
            chkSound.Font = new Font("Segoe UI", 8.25F);
            chkSound.Location = new Point(12, 150);
            chkSound.Name = "chkSound";
            chkSound.Size = new Size(198, 17);
            chkSound.TabIndex = 4;
            chkSound.Text = "🔊 Efecto de sonido en swaps (Beep)";
            chkSound.UseVisualStyleBackColor = true;
            // 
            // chkRecordTrace
            // 
            chkRecordTrace.AutoSize = true;
            chkRecordTrace.Checked = true;
            chkRecordTrace.CheckState = CheckState.Checked;
            chkRecordTrace.Font = new Font("Segoe UI", 8.25F);
            chkRecordTrace.Location = new Point(12, 130);
            chkRecordTrace.Name = "chkRecordTrace";
            chkRecordTrace.Size = new Size(183, 17);
            chkRecordTrace.TabIndex = 3;
            chkRecordTrace.Text = "📝 Registrar bitácora de pasos";
            chkRecordTrace.UseVisualStyleBackColor = true;
            // 
            // btnStopVisualSort
            // 
            btnStopVisualSort.BackColor = Color.MistyRose;
            btnStopVisualSort.Enabled = false;
            btnStopVisualSort.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnStopVisualSort.ForeColor = Color.DarkRed;
            btnStopVisualSort.Location = new Point(12, 95);
            btnStopVisualSort.Name = "btnStopVisualSort";
            btnStopVisualSort.Size = new Size(265, 30);
            btnStopVisualSort.TabIndex = 2;
            btnStopVisualSort.Text = "⏹ Detener Animación";
            btnStopVisualSort.UseVisualStyleBackColor = false;
            btnStopVisualSort.Click += btnStopVisualSort_Click;
            // 
            // btnLiveVisualSort
            // 
            btnLiveVisualSort.BackColor = Color.LightCyan;
            btnLiveVisualSort.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLiveVisualSort.ForeColor = Color.DarkSlateGray;
            btnLiveVisualSort.Location = new Point(12, 58);
            btnLiveVisualSort.Name = "btnLiveVisualSort";
            btnLiveVisualSort.Size = new Size(265, 33);
            btnLiveVisualSort.TabIndex = 1;
            btnLiveVisualSort.Text = "▶ Visualizar en Tiempo Real";
            btnLiveVisualSort.UseVisualStyleBackColor = false;
            btnLiveVisualSort.Click += btnLiveVisualSort_Click;
            // 
            // btnSort
            // 
            btnSort.BackColor = Color.Honeydew;
            btnSort.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSort.ForeColor = Color.DarkGreen;
            btnSort.Location = new Point(12, 22);
            btnSort.Name = "btnSort";
            btnSort.Size = new Size(265, 32);
            btnSort.TabIndex = 0;
            btnSort.Text = "⚡ Ordenar Directo (Instantáneo)";
            btnSort.UseVisualStyleBackColor = false;
            btnSort.Click += btnSort_Click;
            // 
            // grpSortDirection
            // 
            grpSortDirection.Controls.Add(rbDescending);
            grpSortDirection.Controls.Add(rbAscending);
            grpSortDirection.Dock = DockStyle.Top;
            grpSortDirection.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpSortDirection.Location = new Point(10, 215);
            grpSortDirection.Name = "grpSortDirection";
            grpSortDirection.Size = new Size(290, 60);
            grpSortDirection.TabIndex = 1;
            grpSortDirection.TabStop = false;
            grpSortDirection.Text = "Dirección de Ordenamiento";
            // 
            // rbDescending
            // 
            rbDescending.AutoSize = true;
            rbDescending.Font = new Font("Segoe UI", 9F);
            rbDescending.Location = new Point(145, 25);
            rbDescending.Name = "rbDescending";
            rbDescending.Size = new Size(111, 19);
            rbDescending.TabIndex = 1;
            rbDescending.Text = "🔽 Descendente";
            rbDescending.UseVisualStyleBackColor = true;
            // 
            // rbAscending
            // 
            rbAscending.AutoSize = true;
            rbAscending.Checked = true;
            rbAscending.Font = new Font("Segoe UI", 9F);
            rbAscending.Location = new Point(15, 25);
            rbAscending.Name = "rbAscending";
            rbAscending.Size = new Size(103, 19);
            rbAscending.TabIndex = 0;
            rbAscending.TabStop = true;
            rbAscending.Text = "🔼 Ascendente";
            rbAscending.UseVisualStyleBackColor = true;
            // 
            // grpDataConfig
            // 
            grpDataConfig.Controls.Add(btnSaveFile);
            grpDataConfig.Controls.Add(btnLoadFile);
            grpDataConfig.Controls.Add(btnGenerate);
            grpDataConfig.Controls.Add(numElementCount);
            grpDataConfig.Controls.Add(lblCount);
            grpDataConfig.Controls.Add(cmbDataType);
            grpDataConfig.Controls.Add(lblDataType);
            grpDataConfig.Dock = DockStyle.Top;
            grpDataConfig.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpDataConfig.Location = new Point(10, 10);
            grpDataConfig.Name = "grpDataConfig";
            grpDataConfig.Size = new Size(290, 205);
            grpDataConfig.TabIndex = 0;
            grpDataConfig.TabStop = false;
            grpDataConfig.Text = "📦 Datos de Entrada";
            // 
            // btnSaveFile
            // 
            btnSaveFile.Font = new Font("Segoe UI", 9F);
            btnSaveFile.Location = new Point(148, 168);
            btnSaveFile.Name = "btnSaveFile";
            btnSaveFile.Size = new Size(129, 28);
            btnSaveFile.TabIndex = 6;
            btnSaveFile.Text = "💾 Guardar...";
            btnSaveFile.UseVisualStyleBackColor = true;
            btnSaveFile.Click += btnSaveFile_Click;
            // 
            // btnLoadFile
            // 
            btnLoadFile.Font = new Font("Segoe UI", 9F);
            btnLoadFile.Location = new Point(12, 168);
            btnLoadFile.Name = "btnLoadFile";
            btnLoadFile.Size = new Size(130, 28);
            btnLoadFile.TabIndex = 5;
            btnLoadFile.Text = "📂 Importar...";
            btnLoadFile.UseVisualStyleBackColor = true;
            btnLoadFile.Click += btnLoadFile_Click;
            // 
            // btnGenerate
            // 
            btnGenerate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnGenerate.Location = new Point(12, 130);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(265, 32);
            btnGenerate.TabIndex = 4;
            btnGenerate.Text = "⚡ Generar Datos Aleatorios";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // numElementCount
            // 
            numElementCount.Font = new Font("Segoe UI", 9F);
            numElementCount.Location = new Point(12, 95);
            numElementCount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numElementCount.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numElementCount.Name = "numElementCount";
            numElementCount.Size = new Size(265, 23);
            numElementCount.TabIndex = 3;
            numElementCount.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // lblCount
            // 
            lblCount.AutoSize = true;
            lblCount.Font = new Font("Segoe UI", 8.25F);
            lblCount.Location = new Point(12, 78);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(127, 13);
            lblCount.TabIndex = 2;
            lblCount.Text = "Cantidad de elementos:";
            // 
            // cmbDataType
            // 
            cmbDataType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDataType.Font = new Font("Segoe UI", 9F);
            cmbDataType.FormattingEnabled = true;
            cmbDataType.Location = new Point(12, 45);
            cmbDataType.Name = "cmbDataType";
            cmbDataType.Size = new Size(265, 23);
            cmbDataType.TabIndex = 1;
            cmbDataType.SelectedIndexChanged += cmbDataType_SelectedIndexChanged;
            // 
            // lblDataType
            // 
            lblDataType.AutoSize = true;
            lblDataType.Font = new Font("Segoe UI", 8.25F);
            lblDataType.Location = new Point(12, 28);
            lblDataType.Name = "lblDataType";
            lblDataType.Size = new Size(74, 13);
            lblDataType.TabIndex = 0;
            lblDataType.Text = "Tipo de dato:";
            // 
            // pnlMainContainer
            // 
            pnlMainContainer.Controls.Add(statusStripMain);
            pnlMainContainer.Controls.Add(tabControlMain);
            pnlMainContainer.Dock = DockStyle.Fill;
            pnlMainContainer.Location = new Point(310, 0);
            pnlMainContainer.Name = "pnlMainContainer";
            pnlMainContainer.Size = new Size(874, 681);
            pnlMainContainer.TabIndex = 1;
            // 
            // statusStripMain
            // 
            statusStripMain.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStripMain.Location = new Point(0, 659);
            statusStripMain.Name = "statusStripMain";
            statusStripMain.Size = new Size(874, 22);
            statusStripMain.TabIndex = 1;
            statusStripMain.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(32, 17);
            statusLabel.Text = "Listo";
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabVisualizer);
            tabControlMain.Controls.Add(tabMatrix);
            tabControlMain.Controls.Add(tabData);
            tabControlMain.Controls.Add(tabMovements);
            tabControlMain.Controls.Add(tabTrace);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(874, 681);
            tabControlMain.TabIndex = 0;
            // 
            // tabVisualizer
            // 
            tabVisualizer.Controls.Add(pnlVisualizer);
            tabVisualizer.Controls.Add(pnlVisualizerHeader);
            tabVisualizer.Location = new Point(4, 24);
            tabVisualizer.Name = "tabVisualizer";
            tabVisualizer.Padding = new Padding(3);
            tabVisualizer.Size = new Size(866, 653);
            tabVisualizer.TabIndex = 0;
            tabVisualizer.Text = "📊 Gráfica de Barras en Tiempo Real";
            tabVisualizer.UseVisualStyleBackColor = true;
            // 
            // pnlVisualizer
            // 
            pnlVisualizer.BackColor = Color.FromArgb(24, 28, 36);
            pnlVisualizer.Dock = DockStyle.Fill;
            pnlVisualizer.Location = new Point(3, 38);
            pnlVisualizer.Name = "pnlVisualizer";
            pnlVisualizer.Size = new Size(860, 612);
            pnlVisualizer.TabIndex = 1;
            pnlVisualizer.Paint += pnlVisualizer_Paint;
            // 
            // pnlVisualizerHeader
            // 
            pnlVisualizerHeader.BackColor = SystemColors.ControlLight;
            pnlVisualizerHeader.Controls.Add(lblVisualizerStatus);
            pnlVisualizerHeader.Dock = DockStyle.Top;
            pnlVisualizerHeader.Location = new Point(3, 3);
            pnlVisualizerHeader.Name = "pnlVisualizerHeader";
            pnlVisualizerHeader.Size = new Size(860, 35);
            pnlVisualizerHeader.TabIndex = 0;
            // 
            // lblVisualizerStatus
            // 
            lblVisualizerStatus.AutoSize = true;
            lblVisualizerStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblVisualizerStatus.Location = new Point(10, 10);
            lblVisualizerStatus.Name = "lblVisualizerStatus";
            lblVisualizerStatus.Size = new Size(393, 15);
            lblVisualizerStatus.TabIndex = 0;
            lblVisualizerStatus.Text = "Presione 'Visualizar en Tiempo Real' para ver la animación con sonido.";
            // 
            // tabMatrix
            // 
            tabMatrix.Controls.Add(dgvMatrix);
            tabMatrix.Controls.Add(pnlMatrixControls);
            tabMatrix.Location = new Point(4, 24);
            tabMatrix.Name = "tabMatrix";
            tabMatrix.Padding = new Padding(3);
            tabMatrix.Size = new Size(866, 653);
            tabMatrix.TabIndex = 1;
            tabMatrix.Text = "▦ Matriz Horizontal por Filas (Pasos)";
            tabMatrix.UseVisualStyleBackColor = true;
            // 
            // dgvMatrix
            // 
            dgvMatrix.AllowUserToAddRows = false;
            dgvMatrix.AllowUserToDeleteRows = false;
            dgvMatrix.BackgroundColor = Color.White;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvMatrix.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 8.25F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvMatrix.DefaultCellStyle = dataGridViewCellStyle2;
            dgvMatrix.Dock = DockStyle.Fill;
            dgvMatrix.Location = new Point(3, 43);
            dgvMatrix.Name = "dgvMatrix";
            dgvMatrix.ReadOnly = true;
            dgvMatrix.RowHeadersWidth = 140;
            dgvMatrix.Size = new Size(860, 607);
            dgvMatrix.TabIndex = 1;
            // 
            // pnlMatrixControls
            // 
            pnlMatrixControls.BackColor = SystemColors.ControlLight;
            pnlMatrixControls.Controls.Add(lblMatrixLegend);
            pnlMatrixControls.Controls.Add(cmbMatrixViewMode);
            pnlMatrixControls.Controls.Add(lblMatrixMode);
            pnlMatrixControls.Dock = DockStyle.Top;
            pnlMatrixControls.Location = new Point(3, 3);
            pnlMatrixControls.Name = "pnlMatrixControls";
            pnlMatrixControls.Size = new Size(860, 40);
            pnlMatrixControls.TabIndex = 0;
            // 
            // lblMatrixLegend
            // 
            lblMatrixLegend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblMatrixLegend.AutoSize = true;
            lblMatrixLegend.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblMatrixLegend.ForeColor = Color.DarkGoldenrod;
            lblMatrixLegend.Location = new Point(510, 13);
            lblMatrixLegend.Name = "lblMatrixLegend";
            lblMatrixLegend.Size = new Size(335, 13);
            lblMatrixLegend.TabIndex = 2;
            lblMatrixLegend.Text = "★ Celdas en Oro/Salmón = Elementos modificados/swaps en esa pasada";
            // 
            // cmbMatrixViewMode
            // 
            cmbMatrixViewMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMatrixViewMode.FormattingEnabled = true;
            cmbMatrixViewMode.Items.AddRange(new object[] { "Evolución por Pasadas / Hitos del Algoritmo (Runs y Merges)", "Arreglo en Bloques de 50 Elementos por Fila", "Arreglo en Bloques de 40 Elementos por Fila" });
            cmbMatrixViewMode.Location = new Point(115, 8);
            cmbMatrixViewMode.Name = "cmbMatrixViewMode";
            cmbMatrixViewMode.Size = new Size(375, 23);
            cmbMatrixViewMode.TabIndex = 1;
            cmbMatrixViewMode.SelectedIndexChanged += cmbMatrixViewMode_SelectedIndexChanged;
            // 
            // lblMatrixMode
            // 
            lblMatrixMode.AutoSize = true;
            lblMatrixMode.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMatrixMode.Location = new Point(10, 12);
            lblMatrixMode.Name = "lblMatrixMode";
            lblMatrixMode.Size = new Size(99, 15);
            lblMatrixMode.TabIndex = 0;
            lblMatrixMode.Text = "Modo de Matriz:";
            // 
            // tabData
            // 
            tabData.Controls.Add(listBoxResults);
            tabData.Location = new Point(4, 24);
            tabData.Name = "tabData";
            tabData.Padding = new Padding(3);
            tabData.Size = new Size(866, 653);
            tabData.TabIndex = 2;
            tabData.Text = "📋 Datos del Arreglo";
            tabData.UseVisualStyleBackColor = true;
            // 
            // listBoxResults
            // 
            listBoxResults.Dock = DockStyle.Fill;
            listBoxResults.Font = new Font("Segoe UI", 9F);
            listBoxResults.FormattingEnabled = true;
            listBoxResults.ItemHeight = 15;
            listBoxResults.Location = new Point(3, 3);
            listBoxResults.Name = "listBoxResults";
            listBoxResults.Size = new Size(860, 647);
            listBoxResults.TabIndex = 0;
            // 
            // tabMovements
            // 
            tabMovements.Controls.Add(listBoxMovements);
            tabMovements.Controls.Add(pnlMovementsHeader);
            tabMovements.Location = new Point(4, 24);
            tabMovements.Name = "tabMovements";
            tabMovements.Padding = new Padding(3);
            tabMovements.Size = new Size(866, 653);
            tabMovements.TabIndex = 3;
            tabMovements.Text = "📜 Bitácora de Movimientos";
            tabMovements.UseVisualStyleBackColor = true;
            // 
            // listBoxMovements
            // 
            listBoxMovements.Dock = DockStyle.Fill;
            listBoxMovements.Font = new Font("Consolas", 9F);
            listBoxMovements.FormattingEnabled = true;
            listBoxMovements.HorizontalScrollbar = true;
            listBoxMovements.ItemHeight = 14;
            listBoxMovements.Location = new Point(3, 43);
            listBoxMovements.Name = "listBoxMovements";
            listBoxMovements.Size = new Size(860, 607);
            listBoxMovements.TabIndex = 1;
            // 
            // pnlMovementsHeader
            // 
            pnlMovementsHeader.BackColor = SystemColors.ControlLight;
            pnlMovementsHeader.Controls.Add(btnExportMovements);
            pnlMovementsHeader.Controls.Add(lblMovementsTitle);
            pnlMovementsHeader.Dock = DockStyle.Top;
            pnlMovementsHeader.Location = new Point(3, 3);
            pnlMovementsHeader.Name = "pnlMovementsHeader";
            pnlMovementsHeader.Size = new Size(860, 40);
            pnlMovementsHeader.TabIndex = 0;
            // 
            // btnExportMovements
            // 
            btnExportMovements.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportMovements.Location = new Point(710, 6);
            btnExportMovements.Name = "btnExportMovements";
            btnExportMovements.Size = new Size(140, 28);
            btnExportMovements.TabIndex = 1;
            btnExportMovements.Text = "💾 Exportar Bitácora";
            btnExportMovements.UseVisualStyleBackColor = true;
            btnExportMovements.Click += btnExportMovements_Click;
            // 
            // lblMovementsTitle
            // 
            lblMovementsTitle.AutoSize = true;
            lblMovementsTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMovementsTitle.Location = new Point(10, 12);
            lblMovementsTitle.Name = "lblMovementsTitle";
            lblMovementsTitle.Size = new Size(244, 15);
            lblMovementsTitle.TabIndex = 0;
            lblMovementsTitle.Text = "Registro exacto de desplazamientos e inserciones:";
            // 
            // tabTrace
            // 
            tabTrace.Controls.Add(listBoxTrace);
            tabTrace.Controls.Add(panelTraceHeader);
            tabTrace.Location = new Point(4, 24);
            tabTrace.Name = "tabTrace";
            tabTrace.Padding = new Padding(3);
            tabTrace.Size = new Size(866, 653);
            tabTrace.TabIndex = 4;
            tabTrace.Text = "🔍 Trazabilidad y Comparaciones";
            tabTrace.UseVisualStyleBackColor = true;
            // 
            // listBoxTrace
            // 
            listBoxTrace.Dock = DockStyle.Fill;
            listBoxTrace.Font = new Font("Consolas", 9F);
            listBoxTrace.FormattingEnabled = true;
            listBoxTrace.HorizontalScrollbar = true;
            listBoxTrace.ItemHeight = 14;
            listBoxTrace.Location = new Point(3, 43);
            listBoxTrace.Name = "listBoxTrace";
            listBoxTrace.Size = new Size(860, 607);
            listBoxTrace.TabIndex = 1;
            // 
            // panelTraceHeader
            // 
            panelTraceHeader.BackColor = SystemColors.ControlLight;
            panelTraceHeader.Controls.Add(btnSaveTrace);
            panelTraceHeader.Controls.Add(lblTraceSummary);
            panelTraceHeader.Dock = DockStyle.Top;
            panelTraceHeader.Location = new Point(3, 3);
            panelTraceHeader.Name = "panelTraceHeader";
            panelTraceHeader.Size = new Size(860, 40);
            panelTraceHeader.TabIndex = 0;
            // 
            // btnSaveTrace
            // 
            btnSaveTrace.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveTrace.Location = new Point(710, 6);
            btnSaveTrace.Name = "btnSaveTrace";
            btnSaveTrace.Size = new Size(140, 28);
            btnSaveTrace.TabIndex = 1;
            btnSaveTrace.Text = "💾 Exportar Traza";
            btnSaveTrace.UseVisualStyleBackColor = true;
            btnSaveTrace.Click += btnSaveTrace_Click;
            // 
            // lblTraceSummary
            // 
            lblTraceSummary.AutoSize = true;
            lblTraceSummary.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTraceSummary.Location = new Point(10, 12);
            lblTraceSummary.Name = "lblTraceSummary";
            lblTraceSummary.Size = new Size(307, 15);
            lblTraceSummary.TabIndex = 0;
            lblTraceSummary.Text = "Ejecute TimSort para registrar comparaciones y pasos.";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 681);
            Controls.Add(pnlMainContainer);
            Controls.Add(pnlSidebar);
            MinimumSize = new Size(1000, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Algoritmo de Ordenamiento TimSort - Visualizador Avanzado y Matriz de Pasos";
            pnlSidebar.ResumeLayout(false);
            grpMetrics.ResumeLayout(false);
            grpMetrics.PerformLayout();
            grpExecution.ResumeLayout(false);
            grpExecution.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbSpeed).EndInit();
            grpSortDirection.ResumeLayout(false);
            grpSortDirection.PerformLayout();
            grpDataConfig.ResumeLayout(false);
            grpDataConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numElementCount).EndInit();
            pnlMainContainer.ResumeLayout(false);
            pnlMainContainer.PerformLayout();
            statusStripMain.ResumeLayout(false);
            statusStripMain.PerformLayout();
            tabControlMain.ResumeLayout(false);
            tabVisualizer.ResumeLayout(false);
            pnlVisualizerHeader.ResumeLayout(false);
            pnlVisualizerHeader.PerformLayout();
            tabMatrix.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMatrix).EndInit();
            pnlMatrixControls.ResumeLayout(false);
            pnlMatrixControls.PerformLayout();
            tabData.ResumeLayout(false);
            tabMovements.ResumeLayout(false);
            pnlMovementsHeader.ResumeLayout(false);
            pnlMovementsHeader.PerformLayout();
            tabTrace.ResumeLayout(false);
            panelTraceHeader.ResumeLayout(false);
            panelTraceHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlSidebar;
        private GroupBox grpDataConfig;
        private Label lblDataType;
        private ComboBox cmbDataType;
        private Label lblCount;
        private NumericUpDown numElementCount;
        private Button btnGenerate;
        private Button btnLoadFile;
        private Button btnSaveFile;
        private GroupBox grpSortDirection;
        private RadioButton rbAscending;
        private RadioButton rbDescending;
        private GroupBox grpExecution;
        private Button btnSort;
        private Button btnLiveVisualSort;
        private Button btnStopVisualSort;
        private CheckBox chkRecordTrace;
        private CheckBox chkSound;
        private Label lblSpeed;
        private TrackBar tbSpeed;
        private GroupBox grpMetrics;
        private Label lblMetricTime;
        private Label lblMetricComparisons;
        private Label lblMetricMovements;
        private Label lblMetricVerification;
        private Panel pnlMainContainer;
        private StatusStrip statusStripMain;
        private ToolStripStatusLabel statusLabel;
        private TabControl tabControlMain;
        private TabPage tabVisualizer;
        private DoubleBufferedPanel pnlVisualizer;
        private Panel pnlVisualizerHeader;
        private Label lblVisualizerStatus;
        private TabPage tabMatrix;
        private Panel pnlMatrixControls;
        private Label lblMatrixMode;
        private ComboBox cmbMatrixViewMode;
        private Label lblMatrixLegend;
        private DataGridView dgvMatrix;
        private TabPage tabData;
        private ListBox listBoxResults;
        private TabPage tabMovements;
        private Panel pnlMovementsHeader;
        private Label lblMovementsTitle;
        private Button btnExportMovements;
        private ListBox listBoxMovements;
        private TabPage tabTrace;
        private Panel panelTraceHeader;
        private Label lblTraceSummary;
        private Button btnSaveTrace;
        private ListBox listBoxTrace;
    }
}
