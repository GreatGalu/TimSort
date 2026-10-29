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
            cmbDataType = new ComboBox();
            numElementCount = new NumericUpDown();
            rbAscending = new RadioButton();
            rbDescending = new RadioButton();
            groupBox1 = new GroupBox();
            chkRecordTrace = new CheckBox();
            btnGenerate = new Button();
            btnSort = new Button();
            btnLoadFile = new Button();
            btnSaveFile = new Button();
            lblStatus = new Label();
            lblDataType = new Label();
            lblCount = new Label();
            tabControlMain = new TabControl();
            tabResults = new TabPage();
            listBoxResults = new ListBox();
            tabTrace = new TabPage();
            listBoxTrace = new ListBox();
            panelTraceHeader = new Panel();
            btnSaveTrace = new Button();
            lblTraceSummary = new Label();
            ((System.ComponentModel.ISupportInitialize)numElementCount).BeginInit();
            groupBox1.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabResults.SuspendLayout();
            tabTrace.SuspendLayout();
            panelTraceHeader.SuspendLayout();
            SuspendLayout();
            // 
            // cmbDataType
            // 
            cmbDataType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDataType.FormattingEnabled = true;
            cmbDataType.Location = new Point(15, 30);
            cmbDataType.Name = "cmbDataType";
            cmbDataType.Size = new Size(245, 23);
            cmbDataType.TabIndex = 1;
            cmbDataType.SelectedIndexChanged += cmbDataType_SelectedIndexChanged;
            // 
            // numElementCount
            // 
            numElementCount.Location = new Point(15, 78);
            numElementCount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numElementCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numElementCount.Name = "numElementCount";
            numElementCount.Size = new Size(245, 23);
            numElementCount.TabIndex = 3;
            numElementCount.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // rbAscending
            // 
            rbAscending.AutoSize = true;
            rbAscending.Checked = true;
            rbAscending.Location = new Point(15, 24);
            rbAscending.Name = "rbAscending";
            rbAscending.Size = new Size(87, 19);
            rbAscending.TabIndex = 0;
            rbAscending.TabStop = true;
            rbAscending.Text = "Ascendente";
            rbAscending.UseVisualStyleBackColor = true;
            // 
            // rbDescending
            // 
            rbDescending.AutoSize = true;
            rbDescending.Location = new Point(125, 24);
            rbDescending.Name = "rbDescending";
            rbDescending.Size = new Size(93, 19);
            rbDescending.TabIndex = 1;
            rbDescending.Text = "Descendente";
            rbDescending.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbAscending);
            groupBox1.Controls.Add(rbDescending);
            groupBox1.Location = new Point(15, 110);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(245, 60);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Dirección de ordenamiento";
            // 
            // chkRecordTrace
            // 
            chkRecordTrace.AutoSize = true;
            chkRecordTrace.Checked = true;
            chkRecordTrace.CheckState = CheckState.Checked;
            chkRecordTrace.Location = new Point(15, 178);
            chkRecordTrace.Name = "chkRecordTrace";
            chkRecordTrace.Size = new Size(192, 19);
            chkRecordTrace.TabIndex = 5;
            chkRecordTrace.Text = "Registrar traza (comparaciones)";
            chkRecordTrace.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(15, 205);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(245, 34);
            btnGenerate.TabIndex = 6;
            btnGenerate.Text = "⚡ Generar Datos";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // btnSort
            // 
            btnSort.Location = new Point(15, 245);
            btnSort.Name = "btnSort";
            btnSort.Size = new Size(245, 34);
            btnSort.TabIndex = 7;
            btnSort.Text = "🔄 Ordenar con TimSort";
            btnSort.UseVisualStyleBackColor = true;
            btnSort.Click += btnSort_Click;
            // 
            // btnLoadFile
            // 
            btnLoadFile.Location = new Point(15, 285);
            btnLoadFile.Name = "btnLoadFile";
            btnLoadFile.Size = new Size(245, 34);
            btnLoadFile.TabIndex = 8;
            btnLoadFile.Text = "📂 Importar Archivo (JSON/XML/TXT)";
            btnLoadFile.UseVisualStyleBackColor = true;
            btnLoadFile.Click += btnLoadFile_Click;
            // 
            // btnSaveFile
            // 
            btnSaveFile.Location = new Point(15, 325);
            btnSaveFile.Name = "btnSaveFile";
            btnSaveFile.Size = new Size(245, 34);
            btnSaveFile.TabIndex = 9;
            btnSaveFile.Text = "💾 Guardar en Archivo";
            btnSaveFile.UseVisualStyleBackColor = true;
            btnSaveFile.Click += btnSaveFile_Click;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(15, 490);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(32, 15);
            lblStatus.TabIndex = 10;
            lblStatus.Text = "Listo";
            lblStatus.Click += lblStatus_Click;
            // 
            // lblDataType
            // 
            lblDataType.AutoSize = true;
            lblDataType.Location = new Point(15, 12);
            lblDataType.Name = "lblDataType";
            lblDataType.Size = new Size(77, 15);
            lblDataType.TabIndex = 0;
            lblDataType.Text = "Tipo de dato:";
            // 
            // lblCount
            // 
            lblCount.AutoSize = true;
            lblCount.Location = new Point(15, 60);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(132, 15);
            lblCount.TabIndex = 2;
            lblCount.Text = "Cantidad de elementos:";
            // 
            // tabControlMain
            // 
            tabControlMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControlMain.Controls.Add(tabResults);
            tabControlMain.Controls.Add(tabTrace);
            tabControlMain.Location = new Point(275, 12);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(635, 495);
            tabControlMain.TabIndex = 11;
            // 
            // tabResults
            // 
            tabResults.Controls.Add(listBoxResults);
            tabResults.Location = new Point(4, 24);
            tabResults.Name = "tabResults";
            tabResults.Padding = new Padding(3);
            tabResults.Size = new Size(627, 467);
            tabResults.TabIndex = 0;
            tabResults.Text = "📋 Datos del Arreglo";
            tabResults.UseVisualStyleBackColor = true;
            // 
            // listBoxResults
            // 
            listBoxResults.Dock = DockStyle.Fill;
            listBoxResults.FormattingEnabled = true;
            listBoxResults.ItemHeight = 15;
            listBoxResults.Location = new Point(3, 3);
            listBoxResults.Name = "listBoxResults";
            listBoxResults.Size = new Size(621, 461);
            listBoxResults.TabIndex = 0;
            // 
            // tabTrace
            // 
            tabTrace.Controls.Add(listBoxTrace);
            tabTrace.Controls.Add(panelTraceHeader);
            tabTrace.Location = new Point(4, 24);
            tabTrace.Name = "tabTrace";
            tabTrace.Padding = new Padding(3);
            tabTrace.Size = new Size(627, 467);
            tabTrace.TabIndex = 1;
            tabTrace.Text = "🔍 Trazabilidad y Pasos";
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
            listBoxTrace.Size = new Size(621, 421);
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
            panelTraceHeader.Size = new Size(621, 40);
            panelTraceHeader.TabIndex = 0;
            // 
            // btnSaveTrace
            // 
            btnSaveTrace.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveTrace.Location = new Point(480, 6);
            btnSaveTrace.Name = "btnSaveTrace";
            btnSaveTrace.Size = new Size(135, 28);
            btnSaveTrace.TabIndex = 1;
            btnSaveTrace.Text = "💾 Exportar Traza";
            btnSaveTrace.UseVisualStyleBackColor = true;
            btnSaveTrace.Click += btnSaveTrace_Click;
            // 
            // lblTraceSummary
            // 
            lblTraceSummary.AutoSize = true;
            lblTraceSummary.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTraceSummary.Location = new Point(8, 12);
            lblTraceSummary.Name = "lblTraceSummary";
            lblTraceSummary.Size = new Size(307, 15);
            lblTraceSummary.TabIndex = 0;
            lblTraceSummary.Text = "Ejecuta TimSort para registrar comparaciones y swaps.";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(925, 520);
            Controls.Add(tabControlMain);
            Controls.Add(lblDataType);
            Controls.Add(cmbDataType);
            Controls.Add(lblCount);
            Controls.Add(numElementCount);
            Controls.Add(groupBox1);
            Controls.Add(chkRecordTrace);
            Controls.Add(btnGenerate);
            Controls.Add(btnSort);
            Controls.Add(btnLoadFile);
            Controls.Add(btnSaveFile);
            Controls.Add(lblStatus);
            MinimumSize = new Size(850, 480);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Algoritmo TimSort - Analizador de Comparaciones y Movimientos";
            ((System.ComponentModel.ISupportInitialize)numElementCount).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabControlMain.ResumeLayout(false);
            tabResults.ResumeLayout(false);
            tabTrace.ResumeLayout(false);
            panelTraceHeader.ResumeLayout(false);
            panelTraceHeader.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbDataType;
        private NumericUpDown numElementCount;
        private RadioButton rbAscending;
        private RadioButton rbDescending;
        private GroupBox groupBox1;
        private CheckBox chkRecordTrace;
        private Button btnGenerate;
        private Button btnSort;
        private Button btnLoadFile;
        private Button btnSaveFile;
        private Label lblStatus;
        private Label lblDataType;
        private Label lblCount;
        private TabControl tabControlMain;
        private TabPage tabResults;
        private ListBox listBoxResults;
        private TabPage tabTrace;
        private ListBox listBoxTrace;
        private Panel panelTraceHeader;
        private Label lblTraceSummary;
        private Button btnSaveTrace;
    }
}
