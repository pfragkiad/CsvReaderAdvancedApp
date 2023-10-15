namespace CsvWinAnalyzer
{
    partial class frmDatabase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            txtConnectionString = new TextBox();
            label2 = new Label();
            txtCreateTable = new TextBox();
            btnTestConnection = new Button();
            btnTableExists = new Button();
            label3 = new Label();
            txtTargetTable = new TextBox();
            btnCreateTable = new Button();
            btnSaveToDatabase = new Button();
            btnClearTable = new Button();
            progressBar1 = new ProgressBar();
            statusStrip1 = new StatusStrip();
            tstStatus = new ToolStripStatusLabel();
            btnUpdate = new Button();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(37, 23);
            label1.Name = "label1";
            label1.Size = new Size(105, 15);
            label1.TabIndex = 0;
            label1.Text = "Connection string:";
            // 
            // txtConnectionString
            // 
            txtConnectionString.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtConnectionString.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            txtConnectionString.Location = new Point(160, 20);
            txtConnectionString.Multiline = true;
            txtConnectionString.Name = "txtConnectionString";
            txtConnectionString.Size = new Size(601, 94);
            txtConnectionString.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(37, 191);
            label2.Name = "label2";
            label2.Size = new Size(106, 15);
            label2.TabIndex = 2;
            label2.Text = "Create table query:";
            // 
            // txtCreateTable
            // 
            txtCreateTable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtCreateTable.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            txtCreateTable.Location = new Point(160, 188);
            txtCreateTable.Multiline = true;
            txtCreateTable.Name = "txtCreateTable";
            txtCreateTable.ScrollBars = ScrollBars.Vertical;
            txtCreateTable.Size = new Size(601, 158);
            txtCreateTable.TabIndex = 3;
            // 
            // btnTestConnection
            // 
            btnTestConnection.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestConnection.Location = new Point(628, 120);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(133, 23);
            btnTestConnection.TabIndex = 4;
            btnTestConnection.Text = "Test connection";
            btnTestConnection.UseVisualStyleBackColor = true;
            btnTestConnection.Click += btnTestConnection_Click;
            // 
            // btnTableExists
            // 
            btnTableExists.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnTableExists.Location = new Point(628, 352);
            btnTableExists.Name = "btnTableExists";
            btnTableExists.Size = new Size(133, 23);
            btnTableExists.TabIndex = 4;
            btnTableExists.Text = "Check if table exists";
            btnTableExists.UseVisualStyleBackColor = true;
            btnTableExists.Click += btnTableExists_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(37, 150);
            label3.Name = "label3";
            label3.Size = new Size(71, 15);
            label3.TabIndex = 5;
            label3.Text = "Target table:";
            // 
            // txtTargetTable
            // 
            txtTargetTable.Location = new Point(160, 147);
            txtTargetTable.Name = "txtTargetTable";
            txtTargetTable.Size = new Size(202, 23);
            txtTargetTable.TabIndex = 6;
            txtTargetTable.Text = "NEW_TABLE";
            txtTargetTable.KeyDown += txtTargetTable_KeyDown;
            // 
            // btnCreateTable
            // 
            btnCreateTable.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCreateTable.Location = new Point(489, 352);
            btnCreateTable.Name = "btnCreateTable";
            btnCreateTable.Size = new Size(133, 23);
            btnCreateTable.TabIndex = 4;
            btnCreateTable.Text = "Create table";
            btnCreateTable.UseVisualStyleBackColor = true;
            btnCreateTable.Click += btnCreateTable_Click;
            // 
            // btnSaveToDatabase
            // 
            btnSaveToDatabase.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSaveToDatabase.Location = new Point(160, 352);
            btnSaveToDatabase.Name = "btnSaveToDatabase";
            btnSaveToDatabase.Size = new Size(133, 23);
            btnSaveToDatabase.TabIndex = 4;
            btnSaveToDatabase.Text = "Save to database...";
            btnSaveToDatabase.UseVisualStyleBackColor = true;
            btnSaveToDatabase.Click += btnSaveToDatabase_Click;
            // 
            // btnClearTable
            // 
            btnClearTable.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClearTable.Location = new Point(350, 352);
            btnClearTable.Name = "btnClearTable";
            btnClearTable.Size = new Size(133, 23);
            btnClearTable.TabIndex = 4;
            btnClearTable.Text = "Clear table";
            btnClearTable.UseVisualStyleBackColor = true;
            btnClearTable.Click += btnClearTable_Click;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(160, 381);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(601, 23);
            progressBar1.TabIndex = 7;
            progressBar1.Visible = false;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tstStatus });
            statusStrip1.Location = new Point(0, 421);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(798, 22);
            statusStrip1.TabIndex = 8;
            statusStrip1.Text = "statusStrip1";
            // 
            // tstStatus
            // 
            tstStatus.Name = "tstStatus";
            tstStatus.Size = new Size(39, 17);
            tstStatus.Text = "Ready";
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(368, 147);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 23);
            btnUpdate.TabIndex = 9;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // frmDatabase
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(798, 443);
            Controls.Add(btnUpdate);
            Controls.Add(statusStrip1);
            Controls.Add(progressBar1);
            Controls.Add(txtTargetTable);
            Controls.Add(label3);
            Controls.Add(btnSaveToDatabase);
            Controls.Add(btnClearTable);
            Controls.Add(btnCreateTable);
            Controls.Add(btnTableExists);
            Controls.Add(btnTestConnection);
            Controls.Add(txtCreateTable);
            Controls.Add(label2);
            Controls.Add(txtConnectionString);
            Controls.Add(label1);
            Name = "frmDatabase";
            Text = "Copy to database";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtConnectionString;
        private Label label2;
        private TextBox txtCreateTable;
        private Button btnTestConnection;
        private Button btnTableExists;
        private Label label3;
        private TextBox txtTargetTable;
        private Button btnCreateTable;
        private Button btnSaveToDatabase;
        private Button btnClearTable;
        private ProgressBar progressBar1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel tstStatus;
        private Button btnUpdate;
    }
}