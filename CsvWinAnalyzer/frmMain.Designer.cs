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
            toolStripMenuItem3 = new ToolStripSeparator();
            mnuExportSelectedToDatabase = new ToolStripMenuItem();
            mnuExportSelectedToCsvFile = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            tstStatus = new ToolStripStatusLabel();
            menuStrip1 = new MenuStrip();
            mnuFile = new ToolStripMenuItem();
            mnuFileExit = new ToolStripMenuItem();
            mnuTools = new ToolStripMenuItem();
            mnuFindDataTypes = new ToolStripMenuItem();
            mnuFindDataTypesAndRanges = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            mnuExportAllToDatabase = new ToolStripMenuItem();
            mnuField.SuspendLayout();
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtFilePath
            // 
            txtFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilePath.Location = new Point(88, 35);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new Size(982, 23);
            txtFilePath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(995, 64);
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
            lvwHeader.Size = new Size(982, 411);
            lvwHeader.TabIndex = 5;
            lvwHeader.UseCompatibleStateImageBehavior = false;
            lvwHeader.View = View.Details;
            lvwHeader.DoubleClick += lvwHeader_DoubleClick;
            lvwHeader.KeyDown += lvwHeader_KeyDown;
            // 
            // colIndex
            // 
            colIndex.Text = "#";
            colIndex.Width = 40;
            // 
            // colColumnName
            // 
            colColumnName.Text = "Name";
            colColumnName.Width = 250;
            // 
            // colDataType
            // 
            colDataType.Text = "Type";
            // 
            // colMin
            // 
            colMin.Text = "Min";
            colMin.Width = 80;
            // 
            // colMax
            // 
            colMax.Text = "Max";
            colMax.Width = 80;
            // 
            // colAllValues
            // 
            colAllValues.Text = "All #";
            colAllValues.Width = 80;
            // 
            // colNullValues
            // 
            colNullValues.Text = "Null #";
            colNullValues.Width = 80;
            // 
            // colUnparsedValues
            // 
            colUnparsedValues.Text = "Unparsed #";
            colUnparsedValues.Width = 80;
            // 
            // mnuField
            // 
            mnuField.Items.AddRange(new ToolStripItem[] { mnuFindDataTypeFast, mnuFindDataType, mnuChangeDataType, toolStripMenuItem1, mnuAnalyze, toolStripMenuItem3, mnuExportSelectedToDatabase, mnuExportSelectedToCsvFile });
            mnuField.Name = "mnuField";
            mnuField.Size = new Size(228, 148);
            // 
            // mnuFindDataTypeFast
            // 
            mnuFindDataTypeFast.Name = "mnuFindDataTypeFast";
            mnuFindDataTypeFast.Size = new Size(227, 22);
            mnuFindDataTypeFast.Text = "Find data type (fast)...";
            mnuFindDataTypeFast.Click += mnuFindDataTypeFast_Click;
            // 
            // mnuFindDataType
            // 
            mnuFindDataType.Name = "mnuFindDataType";
            mnuFindDataType.Size = new Size(227, 22);
            mnuFindDataType.Text = "Find data type...";
            mnuFindDataType.Click += mnuFindDataType_Click;
            // 
            // mnuChangeDataType
            // 
            mnuChangeDataType.DropDownItems.AddRange(new ToolStripItem[] { mnuBoolean, mnuInteger, mnuLong, mnuFloat, mnuDouble, mnuDateTime, mnuDateTimeOffset, mnuString });
            mnuChangeDataType.Name = "mnuChangeDataType";
            mnuChangeDataType.Size = new Size(227, 22);
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
            toolStripMenuItem1.Size = new Size(224, 6);
            // 
            // mnuAnalyze
            // 
            mnuAnalyze.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            mnuAnalyze.Name = "mnuAnalyze";
            mnuAnalyze.Size = new Size(227, 22);
            mnuAnalyze.Text = "Analyze...";
            mnuAnalyze.Click += mnuAnalyze_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(224, 6);
            // 
            // mnuExportSelectedToDatabase
            // 
            mnuExportSelectedToDatabase.Name = "mnuExportSelectedToDatabase";
            mnuExportSelectedToDatabase.Size = new Size(227, 22);
            mnuExportSelectedToDatabase.Text = "Export selected to database...";
            mnuExportSelectedToDatabase.Click += mnuExportSelectedToDatabase_Click;
            // 
            // mnuExportSelectedToCsvFile
            // 
            mnuExportSelectedToCsvFile.Name = "mnuExportSelectedToCsvFile";
            mnuExportSelectedToCsvFile.Size = new Size(227, 22);
            mnuExportSelectedToCsvFile.Text = "Export selected to CSV file...";
            mnuExportSelectedToCsvFile.Click += mnuExportSelectedToCsvFile_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tstStatus });
            statusStrip1.Location = new Point(0, 522);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1105, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // tstStatus
            // 
            tstStatus.Name = "tstStatus";
            tstStatus.Size = new Size(39, 17);
            tstStatus.Text = "Ready";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { mnuFile, mnuTools });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1105, 24);
            menuStrip1.TabIndex = 9;
            menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuFileExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(37, 20);
            mnuFile.Text = "File";
            // 
            // mnuFileExit
            // 
            mnuFileExit.Name = "mnuFileExit";
            mnuFileExit.ShortcutKeys = Keys.Alt | Keys.F4;
            mnuFileExit.Size = new Size(135, 22);
            mnuFileExit.Text = "Exit";
            mnuFileExit.Click += mnuFileExit_Click;
            // 
            // mnuTools
            // 
            mnuTools.DropDownItems.AddRange(new ToolStripItem[] { mnuFindDataTypes, mnuFindDataTypesAndRanges, toolStripMenuItem2, mnuExportAllToDatabase });
            mnuTools.Name = "mnuTools";
            mnuTools.Size = new Size(46, 20);
            mnuTools.Text = "Tools";
            // 
            // mnuFindDataTypes
            // 
            mnuFindDataTypes.Name = "mnuFindDataTypes";
            mnuFindDataTypes.Size = new Size(224, 22);
            mnuFindDataTypes.Text = "Find data types...";
            mnuFindDataTypes.Click += mnuFindDataTypes_Click;
            // 
            // mnuFindDataTypesAndRanges
            // 
            mnuFindDataTypesAndRanges.Name = "mnuFindDataTypesAndRanges";
            mnuFindDataTypesAndRanges.Size = new Size(224, 22);
            mnuFindDataTypesAndRanges.Text = "Find data types and ranges...";
            mnuFindDataTypesAndRanges.Click += mnuFindDataTypesAndRanges_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(221, 6);
            // 
            // mnuExportAllToDatabase
            // 
            mnuExportAllToDatabase.Name = "mnuExportAllToDatabase";
            mnuExportAllToDatabase.Size = new Size(224, 22);
            mnuExportAllToDatabase.Text = "Export all to database...";
            mnuExportAllToDatabase.Click += mnuExportAllToDatabase_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1105, 544);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(lvwHeader);
            Controls.Add(label1);
            Controls.Add(btnBrowse);
            Controls.Add(txtFilePath);
            MainMenuStrip = menuStrip1;
            Name = "frmMain";
            Text = "CSV analyzer";
            mnuField.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
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
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mnuAnalyze;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuFileExit;
        private ToolStripMenuItem mnuTools;
        private ToolStripMenuItem mnuFindDataTypes;
        private ToolStripMenuItem mnuFindDataTypesAndRanges;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem mnuExportAllToDatabase;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem mnuExportSelectedToDatabase;
        private ToolStripMenuItem mnuExportSelectedToCsvFile;
    }
}