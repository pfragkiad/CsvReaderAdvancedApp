namespace CsvWinAnalyzer
{
    partial class frmMain
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
            components = new System.ComponentModel.Container();
            txtFilePath = new TextBox();
            btnBrowse = new Button();
            label1 = new Label();
            dlgBrowse = new OpenFileDialog();
            lvwHeader = new ListView();
            colIndex = new ColumnHeader();
            colColumnName = new ColumnHeader();
            colDataType = new ColumnHeader();
            colMax = new ColumnHeader();
            mnuField = new ContextMenuStrip(components);
            mnuFindDataType = new ToolStripMenuItem();
            mnuFindMax = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            tstStatus = new ToolStripStatusLabel();
            btnExport = new Button();
            grpField = new GroupBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            lblDataType = new Label();
            lblAllValues = new Label();
            lblNullValues = new Label();
            lblNonNullValues = new Label();
            lblMin = new Label();
            lblMax = new Label();
            btnAnalyzeField = new Button();
            mnuField.SuspendLayout();
            statusStrip1.SuspendLayout();
            grpField.SuspendLayout();
            SuspendLayout();
            // 
            // txtFilePath
            // 
            txtFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilePath.Location = new Point(88, 35);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new Size(746, 23);
            txtFilePath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(759, 64);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 38);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 2;
            label1.Text = "File path:";
            // 
            // dlgBrowse
            // 
            dlgBrowse.DefaultExt = "csv";
            dlgBrowse.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dlgBrowse.Title = "Select a CSV or other text file";
            // 
            // lvwHeader
            // 
            lvwHeader.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lvwHeader.Columns.AddRange(new ColumnHeader[] { colIndex, colColumnName, colDataType, colMax });
            lvwHeader.ContextMenuStrip = mnuField;
            lvwHeader.FullRowSelect = true;
            lvwHeader.Location = new Point(88, 93);
            lvwHeader.Name = "lvwHeader";
            lvwHeader.Size = new Size(435, 336);
            lvwHeader.TabIndex = 5;
            lvwHeader.UseCompatibleStateImageBehavior = false;
            lvwHeader.View = View.Details;
            lvwHeader.KeyDown += lvwHeader_KeyDown;
            // 
            // colIndex
            // 
            colIndex.Text = "#";
            // 
            // colColumnName
            // 
            colColumnName.Text = "Name";
            colColumnName.Width = 100;
            // 
            // colDataType
            // 
            colDataType.Text = "Type";
            // 
            // colMax
            // 
            colMax.Text = "Max/Max length";
            colMax.Width = 100;
            // 
            // mnuField
            // 
            mnuField.Items.AddRange(new ToolStripItem[] { mnuFindDataType, mnuFindMax });
            mnuField.Name = "mnuField";
            mnuField.Size = new Size(189, 48);
            // 
            // mnuFindDataType
            // 
            mnuFindDataType.Name = "mnuFindDataType";
            mnuFindDataType.Size = new Size(188, 22);
            mnuFindDataType.Text = "Find data type (fast)...";
            mnuFindDataType.Click += mnuFindDataType_Click;
            // 
            // mnuFindMax
            // 
            mnuFindMax.Name = "mnuFindMax";
            mnuFindMax.Size = new Size(188, 22);
            mnuFindMax.Text = "Fill max...";
            mnuFindMax.Click += mnuFindMax_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tstStatus });
            statusStrip1.Location = new Point(0, 485);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(869, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // tstStatus
            // 
            tstStatus.Name = "tstStatus";
            tstStatus.Size = new Size(23, 17);
            tstStatus.Text = "OK";
            // 
            // btnExport
            // 
            btnExport.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExport.Location = new Point(88, 444);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(75, 23);
            btnExport.TabIndex = 1;
            btnExport.Text = "Export...";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // grpField
            // 
            grpField.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpField.Controls.Add(btnAnalyzeField);
            grpField.Controls.Add(lblMax);
            grpField.Controls.Add(lblMin);
            grpField.Controls.Add(lblNonNullValues);
            grpField.Controls.Add(lblNullValues);
            grpField.Controls.Add(lblAllValues);
            grpField.Controls.Add(lblDataType);
            grpField.Controls.Add(label4);
            grpField.Controls.Add(label7);
            grpField.Controls.Add(label3);
            grpField.Controls.Add(label6);
            grpField.Controls.Add(label5);
            grpField.Controls.Add(label2);
            grpField.Location = new Point(544, 93);
            grpField.Name = "grpField";
            grpField.Size = new Size(290, 336);
            grpField.TabIndex = 7;
            grpField.TabStop = false;
            grpField.Text = "Field properties";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 157);
            label2.Name = "label2";
            label2.Size = new Size(97, 15);
            label2.TabIndex = 0;
            label2.Text = "Non-null values: ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(17, 83);
            label3.Name = "label3";
            label3.Size = new Size(63, 15);
            label3.TabIndex = 0;
            label3.Text = "All values: ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(17, 119);
            label4.Name = "label4";
            label4.Size = new Size(68, 15);
            label4.TabIndex = 0;
            label4.Text = "Null values:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(17, 196);
            label5.Name = "label5";
            label5.Size = new Size(126, 15);
            label5.TabIndex = 0;
            label5.Text = "Minimum/min length:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(17, 239);
            label6.Name = "label6";
            label6.Size = new Size(130, 15);
            label6.TabIndex = 0;
            label6.Text = "Maximum/max length:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(17, 45);
            label7.Name = "label7";
            label7.Size = new Size(60, 15);
            label7.TabIndex = 0;
            label7.Text = "Data type:";
            // 
            // lblDataType
            // 
            lblDataType.AutoSize = true;
            lblDataType.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblDataType.Location = new Point(171, 45);
            lblDataType.Name = "lblDataType";
            lblDataType.Size = new Size(22, 17);
            lblDataType.TabIndex = 1;
            lblDataType.Text = "int";
            // 
            // lblAllValues
            // 
            lblAllValues.AutoSize = true;
            lblAllValues.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblAllValues.Location = new Point(171, 83);
            lblAllValues.Name = "lblAllValues";
            lblAllValues.Size = new Size(36, 17);
            lblAllValues.TabIndex = 1;
            lblAllValues.Text = "1000";
            // 
            // lblNullValues
            // 
            lblNullValues.AutoSize = true;
            lblNullValues.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblNullValues.Location = new Point(171, 119);
            lblNullValues.Name = "lblNullValues";
            lblNullValues.Size = new Size(36, 17);
            lblNullValues.TabIndex = 1;
            lblNullValues.Text = "1000";
            // 
            // lblNonNullValues
            // 
            lblNonNullValues.AutoSize = true;
            lblNonNullValues.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblNonNullValues.Location = new Point(171, 157);
            lblNonNullValues.Name = "lblNonNullValues";
            lblNonNullValues.Size = new Size(36, 17);
            lblNonNullValues.TabIndex = 1;
            lblNonNullValues.Text = "1000";
            // 
            // lblMin
            // 
            lblMin.AutoSize = true;
            lblMin.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblMin.Location = new Point(171, 196);
            lblMin.Name = "lblMin";
            lblMin.Size = new Size(36, 17);
            lblMin.TabIndex = 1;
            lblMin.Text = "1000";
            // 
            // lblMax
            // 
            lblMax.AutoSize = true;
            lblMax.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblMax.Location = new Point(171, 237);
            lblMax.Name = "lblMax";
            lblMax.Size = new Size(36, 17);
            lblMax.TabIndex = 1;
            lblMax.Text = "1000";
            // 
            // btnAnalyzeField
            // 
            btnAnalyzeField.Location = new Point(17, 298);
            btnAnalyzeField.Name = "btnAnalyzeField";
            btnAnalyzeField.Size = new Size(75, 23);
            btnAnalyzeField.TabIndex = 2;
            btnAnalyzeField.Text = "Analyze...";
            btnAnalyzeField.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(869, 507);
            Controls.Add(grpField);
            Controls.Add(statusStrip1);
            Controls.Add(lvwHeader);
            Controls.Add(label1);
            Controls.Add(btnExport);
            Controls.Add(btnBrowse);
            Controls.Add(txtFilePath);
            Name = "frmMain";
            Text = "CSV analyzer";
            Load += frmMain_Load;
            mnuField.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            grpField.ResumeLayout(false);
            grpField.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtFilePath;
        private Button btnBrowse;
        private Label label1;
        private OpenFileDialog dlgBrowse;
        private ListView lvwHeader;
        private ColumnHeader colIndex;
        private ColumnHeader colColumnName;
        private ColumnHeader colDataType;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel tstStatus;
        private ContextMenuStrip mnuField;
        private ToolStripMenuItem mnuFindDataType;
        private ColumnHeader colMax;
        private ToolStripMenuItem mnuFindMax;
        private Button btnExport;
        private GroupBox grpField;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label6;
        private Label label5;
        private Label label7;
        private Label lblDataType;
        private Label lblNonNullValues;
        private Label lblNullValues;
        private Label lblAllValues;
        private Label lblMax;
        private Label lblMin;
        private Button btnAnalyzeField;
    }
}