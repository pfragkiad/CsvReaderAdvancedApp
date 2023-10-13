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
            btnReadHeader = new Button();
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
            mnuField.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtFilePath
            // 
            txtFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilePath.Location = new Point(88, 35);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new Size(668, 23);
            txtFilePath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(681, 64);
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
            // btnReadHeader
            // 
            btnReadHeader.Location = new Point(88, 109);
            btnReadHeader.Name = "btnReadHeader";
            btnReadHeader.Size = new Size(139, 23);
            btnReadHeader.TabIndex = 4;
            btnReadHeader.Text = "Read header";
            btnReadHeader.UseVisualStyleBackColor = true;
            btnReadHeader.Click += btnReadHeader_Click;
            // 
            // lvwHeader
            // 
            lvwHeader.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwHeader.Columns.AddRange(new ColumnHeader[] { colIndex, colColumnName, colDataType, colMax });
            lvwHeader.ContextMenuStrip = mnuField;
            lvwHeader.FullRowSelect = true;
            lvwHeader.Location = new Point(88, 138);
            lvwHeader.Name = "lvwHeader";
            lvwHeader.Size = new Size(668, 336);
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
            mnuField.Size = new Size(167, 48);
            // 
            // mnuFindDataType
            // 
            mnuFindDataType.Name = "mnuFindDataType";
            mnuFindDataType.Size = new Size(166, 22);
            mnuFindDataType.Text = "Guess data type...";
            mnuFindDataType.Click += mnuFindDataType_Click;
            // 
            // mnuFindMax
            // 
            mnuFindMax.Name = "mnuFindMax";
            mnuFindMax.Size = new Size(166, 22);
            mnuFindMax.Text = "Fill max...";
            mnuFindMax.Click += mnuFindMax_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tstStatus });
            statusStrip1.Location = new Point(0, 491);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(791, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // tstStatus
            // 
            tstStatus.Name = "tstStatus";
            tstStatus.Size = new Size(23, 17);
            tstStatus.Text = "OK";
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(791, 513);
            Controls.Add(statusStrip1);
            Controls.Add(lvwHeader);
            Controls.Add(btnReadHeader);
            Controls.Add(label1);
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
        private Button btnReadHeader;
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
    }
}