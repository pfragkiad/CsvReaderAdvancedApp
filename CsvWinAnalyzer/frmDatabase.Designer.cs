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
            txtConnectionString.Location = new Point(160, 20);
            txtConnectionString.Multiline = true;
            txtConnectionString.Name = "txtConnectionString";
            txtConnectionString.Size = new Size(652, 62);
            txtConnectionString.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(37, 159);
            label2.Name = "label2";
            label2.Size = new Size(106, 15);
            label2.TabIndex = 2;
            label2.Text = "Create table query:";
            // 
            // txtCreateTable
            // 
            txtCreateTable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtCreateTable.Location = new Point(160, 159);
            txtCreateTable.Multiline = true;
            txtCreateTable.Name = "txtCreateTable";
            txtCreateTable.ScrollBars = ScrollBars.Vertical;
            txtCreateTable.Size = new Size(652, 90);
            txtCreateTable.TabIndex = 3;
            // 
            // btnTestConnection
            // 
            btnTestConnection.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestConnection.Location = new Point(679, 88);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(133, 23);
            btnTestConnection.TabIndex = 4;
            btnTestConnection.Text = "Test connection";
            btnTestConnection.UseVisualStyleBackColor = true;
            btnTestConnection.Click += btnTestConnection_Click;
            // 
            // btnTableExists
            // 
            btnTableExists.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTableExists.Location = new Point(679, 255);
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
            label3.Location = new Point(37, 118);
            label3.Name = "label3";
            label3.Size = new Size(71, 15);
            label3.TabIndex = 5;
            label3.Text = "Target table:";
            // 
            // txtTargetTable
            // 
            txtTargetTable.Location = new Point(160, 115);
            txtTargetTable.Name = "txtTargetTable";
            txtTargetTable.Size = new Size(202, 23);
            txtTargetTable.TabIndex = 6;
            txtTargetTable.Text = "NEW_TABLE";
            txtTargetTable.KeyDown += txtTargetTable_KeyDown;
            // 
            // frmDatabase
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(849, 329);
            Controls.Add(txtTargetTable);
            Controls.Add(label3);
            Controls.Add(btnTableExists);
            Controls.Add(btnTestConnection);
            Controls.Add(txtCreateTable);
            Controls.Add(label2);
            Controls.Add(txtConnectionString);
            Controls.Add(label1);
            Name = "frmDatabase";
            Text = "Copy to database";
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
    }
}