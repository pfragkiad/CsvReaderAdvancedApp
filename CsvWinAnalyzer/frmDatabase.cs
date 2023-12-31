﻿using CsvReaderAdvanced.Files;
using CsvReaderAdvanced.Schemas;
using Microsoft.Data.SqlClient;
using SqlServerExplorerLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsvWinAnalyzer
{
    public partial class frmDatabase : Form
    {
        private readonly SqlServerExplorer _sql;
        private readonly CsvFileFactory _fileFactory;

        public frmDatabase(
            SqlServerExplorer explorer,
            CsvFileFactory csvFiles)
        {
            InitializeComponent();

            txtConnectionString.Text = FormSettings.Default.ConnectionString;
            txtTargetTable.Text = FormSettings.Default.TargetTable;

            _sql = explorer;
            _fileFactory = csvFiles;
        }

        List<CsvFieldTypeInfo> _items = new();
        public List<CsvFieldTypeInfo> SelectedHeaders
        {
            get => _items;
            set
            {
                _items = value;

                UpdateCreateTableQuery();
            }
        }

        public string? SourcePath { get; internal set; }
        public Encoding? Encoding { get; internal set; }

        //TODO: Move update create table functionality within the CSVLIb
        private void UpdateCreateTableQuery()
        {
            List<string> declarations = new();

            var file = _fileFactory.GetFile(SourcePath!, Encoding!, true);


            foreach (var item in _items)
            {
                if (item.BaseType== BaseType.Unknown) continue;

                string columnName = file.Header!.Value.Tokens[item.Column]; //item.SubItems[1].Text;

                if (item.BaseType == BaseType.String)
                {
                    //max
                    //int length = int.Parse(item.SubItems[4].Text);
                    int length = (int)item.Maximum!;
                    declarations.Add($"[{columnName}] nvarchar({length}) NULL");
                }
                else if (item.BaseType == BaseType.Integer)
                    declarations.Add($"[{columnName}] int NULL");
                else if (item.BaseType == BaseType.Long)
                    declarations.Add($"[{columnName}] bigint NULL");
                else if (item.BaseType == BaseType.Boolean)
                    declarations.Add($"[{columnName}] bit NULL");
                else if (item.BaseType == BaseType.Float)
                    declarations.Add($"[{columnName}] real NULL");
                else if (item.BaseType == BaseType.Double)
                    declarations.Add($"[{columnName}] float NULL");
                else if (item.BaseType == BaseType.DateTime)
                    declarations.Add($"[{columnName}] datetime NULL");
                else if (item.BaseType == BaseType.DateTimeOffset)
                    declarations.Add($"[{columnName}] datetimeoffset NULL");
            }

            txtCreateTable.Text = $"CREATE TABLE [{txtTargetTable.Text}] (\r\n   " + string.Join(",\r\n   ", declarations) + "\r\n)";
        }

        DataTable GetDataTable()
        {
            DataTable table = new();

            var file = _fileFactory.GetFile(SourcePath!, Encoding!, true);

            foreach (var item in _items)
            {
                BaseType baseType = item.BaseType;
                if (baseType == BaseType.Unknown) continue;
                string columnName = file.Header!.Value.Tokens[item.Column];

                if (baseType == BaseType.String)
                    table.Columns.Add(columnName, typeof(string));
                else if (baseType == BaseType.Integer)
                    table.Columns.Add(columnName, typeof(int));
                else if (baseType == BaseType.Long)
                    table.Columns.Add(columnName, typeof(long));
                else if (baseType == BaseType.Boolean)
                    table.Columns.Add(columnName, typeof(bool));
                else if (baseType == BaseType.Float)
                    table.Columns.Add(columnName, typeof(float));
                else if (baseType == BaseType.Double)
                    table.Columns.Add(columnName, typeof(double));
                else if (baseType == BaseType.DateTime)
                    table.Columns.Add(columnName, typeof(DateTime));
                else if (baseType == BaseType.DateTimeOffset)
                    table.Columns.Add(columnName, typeof(DateTimeOffset));
            }

            int count = _items.First().ValuesCount;

            const int batchSize = 100000;
            int iBatches = count / batchSize;
            if (iBatches * batchSize < count) iBatches++;
            progressBar1.Minimum = 0; progressBar1.Value = 0;
            progressBar1.Maximum = iBatches;
            progressBar1.Visible = true;

            table.BeginLoadData();

            var fields = _items.Where(f => f.BaseType != BaseType.Unknown).ToList();

            int iRow = 0;
            foreach (var l in file.Read(true))
            {
                if (!l.HasValue) continue;
                var lt = l.Value;
                object?[] values = new object?[fields.Count];

                iRow++; if (iRow % batchSize == 0) { progressBar1.Value++; progressBar1.Refresh(); }

                int i = 0;
                foreach (var f in fields)
                {
                    if (f.BaseType == BaseType.String)
                        values[i] = lt.GetString(f.Column);
                    else if (f.BaseType == BaseType.Integer)
                        values[i] = (int?)lt.GetInt(f.Column);
                    else if (f.BaseType == BaseType.Long)
                        values[i] = (long?)lt.GetLong(f.Column);
                    else if (f.BaseType == BaseType.Boolean)
                        values[i] = (bool?)lt.GetBoolean(f.Column);
                    else if (f.BaseType == BaseType.Float)
                        values[i] = (float?)lt.GetFloat(f.Column);
                    else if (f.BaseType == BaseType.Double)
                        values[i] = (double?)lt.GetDouble(f.Column);
                    else if (f.BaseType == BaseType.DateTime)
                        values[i] = (DateTime?)lt.GetDateTime(f.Column);
                    else if (f.BaseType == BaseType.DateTimeOffset)
                        values[i] = (DateTimeOffset?)lt.GetDateTimeOffset(f.Column);
                    i++;
                }

                table.Rows.Add(values);
            }
            table.EndLoadData();

            progressBar1.Visible = false;

            return table;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            FormSettings.Default.ConnectionString = txtConnectionString.Text;
            FormSettings.Default.TargetTable = txtTargetTable.Text;

            FormSettings.Default.Save();

            base.OnClosing(e);

        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            bool tested = await _sql.TestConnection(txtConnectionString.Text);
            if (tested) ShowInfo("Test is successful!");
            else ShowError("Could not connect to the database!");

        }

        private async void btnTableExists_Click(object sender, EventArgs e)
        {
            bool exists = await _sql.TableExists(txtConnectionString.Text, txtTargetTable.Text);
            if (exists) ShowWarning("Table already exists!");
            else ShowInfo("Table does not exist!");

        }

        private void txtTargetTable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) UpdateCreateTableQuery();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateCreateTableQuery();
        }

        private async void btnCreateTable_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                await _sql.Execute(txtConnectionString.Text, txtCreateTable.Text);
                bool created = await _sql.TableExists(txtConnectionString.Text, txtTargetTable.Text);
                StopWaiting();
                if (created) ShowInfo("Table was successfully created!");
                else ShowError("Table could not be created!");

            }
            catch (Exception ex)
            {
                StopWaiting();
                ShowError(ex.Message);
            }
        }

        private void UpdateProgressBar(int value)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                progressBar1.Value = value;
            }
        }

        private async void btnSaveToDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();


                string cs = txtConnectionString.Text; string t = txtTargetTable.Text;
                if (!(await _sql.TableExists(cs, t)))
                {
                    await _sql.Execute(cs, txtCreateTable.Text);
                    bool created = await _sql.TableExists(cs, t);
                    if (!created) throw new InvalidOperationException("Table could not be created!");
                }
                else //table already exists
                {
                    //check IF the table is empty!
                    bool isEmpty = await _sql.IsTableEmpty(cs, t);
                    if (!isEmpty) //ShowInfo("Table is empty!");
                    {
                        var reply = ShowWarningAndAsk("Table is NOT empty. Do you want to proceed?");
                        if (reply != DialogResult.Yes) { StopWaiting(); return; }
                    }
                }

                tstStatus.Text = "Loading datatable..."; statusStrip1.Refresh();
                //proceed with the bulk copy
                var table = GetDataTable();

                tstStatus.Text = "Saving to database..."; statusStrip1.Refresh(); Application.DoEvents();
                SqlConnection connection = new(cs);
                connection.Open();
                SqlBulkCopy copier = new(connection);
                copier.BulkCopyTimeout = 0;
                copier.DestinationTableName = t;

                const int batchSize = 100000;
                int iBatches = table.Rows.Count / batchSize;
                if (iBatches * batchSize < table.Rows.Count) iBatches++;
                progressBar1.Minimum = 0; progressBar1.Value = 0;
                progressBar1.Maximum = iBatches;
                int iProcessedBatches = 0;

                copier.BatchSize = batchSize;
                copier.NotifyAfter = batchSize;
                copier.SqlRowsCopied += (o2, e2) => { UpdateProgressBar(++iProcessedBatches); };

                progressBar1.Visible = true;

                await copier.WriteToServerAsync(table);
                connection.Close();

                progressBar1.Visible = false;
                tstStatus.Text = "Ready"; statusStrip1.Refresh();


                StopWaiting();

                ShowInfo("Successfully imported!");
            }
            catch (Exception ex)
            {
                StopWaiting();
                tstStatus.Text = "Ready";
                ShowError(ex.Message);
            }
        }

        private async void btnClearTable_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                StopWaiting();

                await _sql.TruncateTable(txtConnectionString.Text, txtTargetTable.Text);

                ShowInfo("Successfully truncated table!");
            }
            catch (Exception ex)
            {
                StopWaiting();
                ShowError(ex.Message);
            }
        }


    }
}
