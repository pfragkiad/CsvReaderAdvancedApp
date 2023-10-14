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
            colMin = new ColumnHeader();
            colMax = new ColumnHeader();
            colAllValues = new ColumnHeader();
            colNullValues = new ColumnHeader();
            colUnparsedValues = new ColumnHeader();
            mnuField = new ContextMenuStrip(components);
            mnuFindDataTypeFast = new ToolStripMenuItem();
            mnuFindDataType = new ToolStripMenuItem();
            mnuChangeDataType = new ToolStripMenuItem();
            mnuBoolean = new ToolStripMenuItem();
            mnuInteger = new ToolStripMenuItem();
            mnuLong = new ToolStripMenuItem();
            mnuFloat = new ToolStripMenuItem();
            mnuDouble = new ToolStripMenuItem();
            mnuDateTime = new ToolStripMenuItem();
            mnuDateTimeOffset = new ToolStripMenuItem();
            mnuString = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            mnuAnalyze = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            tstStatus = new ToolStripStatusLabel();
            btnExport = new Button();
            btnAnalyzeAllField = new Button();
            btnFindDataTypes = new Button();
            btnExportToDatabase = new Button();
            mnuField.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtFilePath
            // 
            txtFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilePath.Location = new Point(88, 35);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new Size(1099, 23);
            txtFilePath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(1112, 64);
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
            lvwHeader.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwHeader.Columns.AddRange(new ColumnHeader[] { colIndex, colColumnName, colDataType, colMin, colMax, colAllValues, colNullValues, colUnparsedValues });
            lvwHeader.ContextMenuStrip = mnuField;
            lvwHeader.FullRowSelect = true;
            lvwHeader.Location = new Point(88, 93);
            lvwHeader.Name = "lvwHeader";
            lvwHeader.Size = new Size(1099, 313);
            lvwHeader.TabIndex = 5;
            lvwHeader.UseCompatibleStateImageBehavior = false;
            lvwHeader.View = View.Details;
            lvwHeader.DoubleClick += lvwHeader_DoubleClick;
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
            // colMin
            // 
            colMin.Text = "Min";
            // 
            // colMax
            // 
            colMax.Text = "Max";
            // 
            // colAllValues
            // 
            colAllValues.Text = "All #";
            // 
            // colNullValues
            // 
            colNullValues.Text = "Null #";
            // 
            // colUnparsedValues
            // 
            colUnparsedValues.Text = "Unparsed #";
            colUnparsedValues.Width = 80;
            // 
            // mnuField
            // 
            mnuField.Items.AddRange(new ToolStripItem[] { mnuFindDataTypeFast, mnuFindDataType, mnuChangeDataType, toolStripMenuItem1, mnuAnalyze });
            mnuField.Name = "mnuField";
            mnuField.Size = new Size(189, 98);
            // 
            // mnuFindDataTypeFast
            // 
            mnuFindDataTypeFast.Name = "mnuFindDataTypeFast";
            mnuFindDataTypeFast.Size = new Size(188, 22);
            mnuFindDataTypeFast.Text = "Find data type (fast)...";
            mnuFindDataTypeFast.Click += mnuFindDataTypeFast_Click;
            // 
            // mnuFindDataType
            // 
            mnuFindDataType.Name = "mnuFindDataType";
            mnuFindDataType.Size = new Size(188, 22);
            mnuFindDataType.Text = "Find data type...";
            mnuFindDataType.Click += mnuFindDataType_Click;
            // 
            // mnuChangeDataType
            // 
            mnuChangeDataType.DropDownItems.AddRange(new ToolStripItem[] { mnuBoolean, mnuInteger, mnuLong, mnuFloat, mnuDouble, mnuDateTime, mnuDateTimeOffset, mnuString });
            mnuChangeDataType.Name = "mnuChangeDataType";
            mnuChangeDataType.Size = new Size(188, 22);
            mnuChangeDataType.Text = "Change data type to";
            // 
            // mnuBoolean
            // 
            mnuBoolean.Name = "mnuBoolean";
            mnuBoolean.Size = new Size(156, 22);
            mnuBoolean.Text = "Boolean";
            mnuBoolean.Click += mnuBoolean_Click;
            // 
            // mnuInteger
            // 
            mnuInteger.Name = "mnuInteger";
            mnuInteger.Size = new Size(156, 22);
            mnuInteger.Text = "Integer";
            mnuInteger.Click += mnuInteger_Click;
            // 
            // mnuLong
            // 
            mnuLong.Name = "mnuLong";
            mnuLong.Size = new Size(156, 22);
            mnuLong.Text = "Long";
            mnuLong.Click += mnuLong_Click;
            // 
            // mnuFloat
            // 
            mnuFloat.Name = "mnuFloat";
            mnuFloat.Size = new Size(156, 22);
            mnuFloat.Text = "Float";
            mnuFloat.Click += mnuFloat_Click;
            // 
            // mnuDouble
            // 
            mnuDouble.Name = "mnuDouble";
            mnuDouble.Size = new Size(156, 22);
            mnuDouble.Text = "Double";
            mnuDouble.Click += mnuDouble_Click;
            // 
            // mnuDateTime
            // 
            mnuDateTime.Name = "mnuDateTime";
            mnuDateTime.Size = new Size(156, 22);
            mnuDateTime.Text = "DateTime";
            mnuDateTime.Click += mnuDateTime_Click;
            // 
            // mnuDateTimeOffset
            // 
            mnuDateTimeOffset.Name = "mnuDateTimeOffset";
            mnuDateTimeOffset.Size = new Size(156, 22);
            mnuDateTimeOffset.Text = "DateTimeOffset";
            mnuDateTimeOffset.Click += mnuDateTimeOffset_Click;
            // 
            // mnuString
            // 
            mnuString.Name = "mnuString";
            mnuString.Size = new Size(156, 22);
            mnuString.Text = "String";
            mnuString.Click += mnuString_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(185, 6);
            // 
            // mnuAnalyze
            // 
            mnuAnalyze.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            mnuAnalyze.Name = "mnuAnalyze";
            mnuAnalyze.Size = new Size(188, 22);
            mnuAnalyze.Text = "Analyze...";
            mnuAnalyze.Click += mnuAnalyze_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tstStatus });
            statusStrip1.Location = new Point(0, 485);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1222, 22);
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
            btnExport.Location = new Point(88, 426);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(100, 41);
            btnExport.TabIndex = 1;
            btnExport.Text = "Export partial CSV file...";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // btnAnalyzeAllField
            // 
            btnAnalyzeAllField.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAnalyzeAllField.Location = new Point(300, 426);
            btnAnalyzeAllField.Name = "btnAnalyzeAllField";
            btnAnalyzeAllField.Size = new Size(100, 41);
            btnAnalyzeAllField.TabIndex = 2;
            btnAnalyzeAllField.Text = "Analyze all fields...";
            btnAnalyzeAllField.UseVisualStyleBackColor = true;
            btnAnalyzeAllField.Click += btnAnalyzeAllField_Click;
            // 
            // btnFindDataTypes
            // 
            btnFindDataTypes.Location = new Point(194, 426);
            btnFindDataTypes.Name = "btnFindDataTypes";
            btnFindDataTypes.Size = new Size(100, 41);
            btnFindDataTypes.TabIndex = 7;
            btnFindDataTypes.Text = "Find all data types...";
            btnFindDataTypes.UseVisualStyleBackColor = true;
            btnFindDataTypes.Click += btnFindDataTypes_Click;
            // 
            // btnExportToDatabase
            // 
            btnExportToDatabase.Location = new Point(406, 426);
            btnExportToDatabase.Name = "btnExportToDatabase";
            btnExportToDatabase.Size = new Size(100, 41);
            btnExportToDatabase.TabIndex = 8;
            btnExportToDatabase.Text = "Export to database...";
            btnExportToDatabase.UseVisualStyleBackColor = true;
            btnExportToDatabase.Click += btnExportToDatabase_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1222, 507);
            Controls.Add(btnExportToDatabase);
            Controls.Add(btnFindDataTypes);
            Controls.Add(btnAnalyzeAllField);
            Controls.Add(statusStrip1);
            Controls.Add(lvwHeader);
            Controls.Add(label1);
            Controls.Add(btnExport);
            Controls.Add(btnBrowse);
            Controls.Add(txtFilePath);
            Name = "frmMain";
            Text = "CSV analyzer";
            mnuField.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
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
        private ToolStripMenuItem mnuFindDataTypeFast;
        private ColumnHeader colMax;
        private Button btnExport;
        private Button btnAnalyzeAllField;
        private ColumnHeader colMin;
        private ColumnHeader colAllValues;
        private ColumnHeader colNullValues;
        private ColumnHeader colUnparsedValues;
        private ToolStripMenuItem mnuChangeDataType;
        private ToolStripMenuItem mnuBoolean;
        private ToolStripMenuItem mnuInteger;
        private ToolStripMenuItem mnuLong;
        private ToolStripMenuItem mnuFloat;
        private ToolStripMenuItem mnuDouble;
        private ToolStripMenuItem mnuDateTime;
        private ToolStripMenuItem mnuDateTimeOffset;
        private ToolStripMenuItem mnuString;
        private ToolStripMenuItem mnuFindDataType;
        private Button btnFindDataTypes;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mnuAnalyze;
        private Button btnExportToDatabase;
    }
}