using CsvReaderAdvanced.Schemas;
using SqlServerExplorerLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsvWinAnalyzer
{
    public partial class frmDatabase : Form
    {
        private readonly SqlServerExplorer _explorer;

        public frmDatabase(SqlServerExplorer explorer)
        {
            InitializeComponent();
            txtConnectionString.Text = FormSettings.Default.ConnectionString;
            _explorer = explorer;
        }

        List<ListViewItem> items;
        public List<ListViewItem> SelectedHeaders
        {
            get => items;
            set
            {
                items = value;

                UpdateCreateTableQuery();
            }
        }

        private void UpdateCreateTableQuery()
        {
            List<string> declarations = new();
            foreach (ListViewItem item in items)
            {
                if (item.Tag is null) continue;

                BaseType baseType = (BaseType)item.Tag;
                if (baseType == BaseType.Unknown) continue;

                string columnName = item.SubItems[1].Text;

                if (baseType == BaseType.String)
                {
                    //max
                    int length = int.Parse(item.SubItems[4].Text);
                    declarations.Add($"[{columnName}] nvarchar({length}) NULL");
                }
                else if (baseType == BaseType.Integer)
                    declarations.Add($"[{columnName}] int NULL");
                else if (baseType == BaseType.Long)
                    declarations.Add($"[{columnName}] bigint NULL");
                else if (baseType == BaseType.Boolean)
                    declarations.Add($"[{columnName}] bit NULL");
                else if (baseType == BaseType.Float)
                    declarations.Add($"[{columnName}] real NULL");
                else if (baseType == BaseType.Double)
                    declarations.Add($"[{columnName}] float NULL");
                else if (baseType == BaseType.DateTime)
                    declarations.Add($"[{columnName}] datetime NULL");
                else if (baseType == BaseType.DateTimeOffset)
                    declarations.Add($"[{columnName}] datetimeoffset NULL");
            }

            txtCreateTable.Text = $"CREATE TABLE [{txtTargetTable.Text}](\r\n" + string.Join(",\r\n", declarations) + ")";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            FormSettings.Default.ConnectionString = txtConnectionString.Text;
            FormSettings.Default.Save();

            base.OnClosing(e);

        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            bool tested = await _explorer.TestConnection(txtConnectionString.Text);
            if (tested) ShowInfo("Test is successful!");
            else ShowError("Could not connect to the database!");

        }

        private async void btnTableExists_Click(object sender, EventArgs e)
        {
            bool exists = await _explorer.TableExists(txtConnectionString.Text, txtTargetTable.Text);
            if (exists) ShowWarning("Table already exists!");
            else ShowInfo("Table does not exist!");

        }

        private void txtTargetTable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) UpdateCreateTableQuery();
        }
    }
}
