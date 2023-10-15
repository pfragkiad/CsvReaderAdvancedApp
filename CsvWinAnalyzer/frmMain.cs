using CsvReaderAdvanced;
using CsvReaderAdvanced.Interfaces;
using CsvReaderAdvanced.Schemas;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace CsvWinAnalyzer
{
    public partial class frmMain : Form
    {
        private readonly IServiceProvider _provider;
        private readonly CsvFileFactory _csvFiles;

        public frmMain(
            IServiceProvider provider,
            CsvFileFactory csvFiles)
        {
            InitializeComponent();

            _provider = provider;
            _csvFiles = csvFiles;

            //initialize base on settings
            txtFilePath.Text = FormSettings.Default.Filepath;
            if (txtFilePath.Text.Length > 0 && File.Exists(txtFilePath.Text)) ReadHeader();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var reply = dlgBrowse.ShowDialog();
            if (reply != DialogResult.OK) return;

            txtFilePath.Text = dlgBrowse.FileName;
            ReadHeader();

            //Cursor.Current = Cursors.WaitCursor;
            //tstStatus.Text = "Checking file..."; statusStrip1.Refresh();
            //CountLines();
            //btnReadHeader.PerformClick();

            //Cursor.Current = Cursors.Default;
        }

        //private void CountLines()
        //{
        //    int count = 0; int nonEmptyLines = 0;
        //    using StreamReader reader = new StreamReader(txtFilePath.Text, Encoding.UTF8);
        //    while (!reader.EndOfStream)
        //    {
        //        string? line = reader.ReadLine();
        //        if (line is null) break;
        //        count++;
        //        if (line.Length > 0) nonEmptyLines++;
        //    }
        //    tstStatus.Text = $"Lines: {count}, Non-empty Lines: {nonEmptyLines}";
        //}

        protected override void OnClosing(CancelEventArgs e)
        {
            FormSettings.Default.Filepath = txtFilePath.Text;
            FormSettings.Default.Save();
            base.OnClosing(e);
        }


        private void btnReadHeader_Click(object sender, EventArgs e)
        {
            ReadHeader();
        }

        private void ReadHeader()
        {
            var file = _csvFiles.GetFile(txtFilePath.Text, Encoding.UTF8, true);

            lvwHeader.Items.Clear();
            foreach (var entry in file.ExistingColumns)
            {
                string columnName = entry.Key;
                int column = entry.Value;
                //var item = lvwHeader.Items.Add($"{column:000}");
                var item = lvwHeader.Items.Add($"{column + 1}");
                item.SubItems.Add(columnName);
            }
            lvwHeader.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        enum Types { Unknown, Integer, Float, DateTime, String }



        private static void SetSubItem(ListViewItem item, int index, string text)
        {
            if (item.SubItems.Count < index + 1)
                item.SubItems.Add(text);
            else
                item.SubItems[index].Text = text;
        }

        private static void SetDataType(ListViewItem item, BaseType dataType)
        {
            SetSubItem(item, 2, dataType.ToString());
            item.ForeColor = dataType == BaseType.Unknown ? Color.Red : Color.Blue;
            item.Tag = dataType;
        }


        private void lvwHeader_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control == true)
                foreach (ListViewItem item in lvwHeader.Items)
                    item.Selected = true;
        }


        private void btnExport_Click(object sender, EventArgs e)
        {
            string p = SourcePath;
            if (p.Length == 0 || !File.Exists(p))
            {
                ShowWarning("The current path is emppty or does not exist!");
                return;
            }

            var headers = SelectedHeaders;
            if (headers.Count == 0)
            {
                ShowWarning("Select at least one header first!");
                return;
            }

            string pTarget = GetTargetPath()!;

            SaveFileDialog d = new SaveFileDialog()
            {
                DefaultExt = "csv",
                Filter = "CSV files (*.csv)|*.csv|Text files (*.txt|*.txt|All files (*.*)|*.txt",
                FilterIndex = 1,
                InitialDirectory = Path.GetDirectoryName(p),
                FileName = Path.GetFileName(pTarget),
                Title = "Select the save location"
            };
            var reply = d.ShowDialog();
            if (reply != DialogResult.OK) return;

            Wait();

            try
            {

                var file = _csvFiles.GetFile(p, Encoding.Default, true);
                file.SavePartialAs(p, d.FileName, ';', Encoding.Default, headers.Select(c => int.Parse(c.Text) - 1).ToArray());
                StopWaiting();
                ShowInfo("Successfully exported!");
            }
            catch (Exception ex)
            {
                StopWaiting();
                ShowError($"Error: {ex.Message}");
            }
        }



        //TODO: Ensure data type (run all lines)



        private string? GetTargetPath(string suffix = "_filtered")
        {
            string p = SourcePath;
            if (p.Length > 0 && !File.Exists(p)) return null;

            return Path.Combine(Path.GetDirectoryName(p), Path.GetFileNameWithoutExtension(p) + suffix + Path.GetExtension(p));
        }

        private string SourcePath { get => txtFilePath.Text; }
        private List<ListViewItem> SelectedHeaders { get => lvwHeader.SelectedItems.Cast<ListViewItem>().ToList(); }

        private List<ListViewItem> AllHeaders { get => lvwHeader.Items.Cast<ListViewItem>().ToList(); }

        private List<int> SelectedColumns { get => SelectedHeaders.Select(l => int.Parse(l.Text) - 1).ToList(); }

        #region Analyze fields


        private void lvwHeader_DoubleClick(object sender, EventArgs e)
        {
            AnalyzeHeaders(SelectedHeaders);
        }

        private void mnuAnalyze_Click(object sender, EventArgs e)
        {
            AnalyzeHeaders(SelectedHeaders);

        }

        private void btnAnalyzeAllField_Click(object sender, EventArgs e)
        {
            AnalyzeHeaders2(AllHeaders);
        }


        private void AnalyzeHeaders(IEnumerable<ListViewItem> items)
        {
            string p = SourcePath;
            if (p.Length == 0 || !File.Exists(p))
            {
                ShowWarning("The current path is emppty or does not exist!");
                return;
            }

            Wait();


            //if (SelectedColumns.Count == 0) return;

            //ListViewItem selectedItem = SelectedHeaders[0];
            //int column = SelectedColumns[0];
            //string columnName = selectedItem.SubItems[1].Text;
            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;

            foreach (var item in items)
            {
                item.EnsureVisible();

                var file = _csvFiles.GetFile(path, encoding, true);
                int column = int.Parse(item.Text) - 1;
                string columnName = item.SubItems[1].Text;

                if (item.Tag is null)
                {
                    var dataType = file.GetBaseType(column, path, encoding, BaseType.Unknown, true);
                    SetDataType(item, dataType);
                }

                BaseType assumedType = (BaseType)item.Tag!;
                tstStatus.Text = $"Analyzing [{columnName}]..."; statusStrip1.Refresh();

                if (assumedType == BaseType.Unknown)
                {
                    var dataType = file.GetBaseType(column, path, encoding, BaseType.Unknown, true);
                    SetDataType(item, dataType);
                }

                //lblDataType.Text = assumedType.ToString();
                if (assumedType == BaseType.Unknown)
                {
                    //lblAllValues.Text = lblMax.Text = lblMin.Text = lblNullValues.Text = lblUnparsedValues.Text = "-";
                    SetSubItem(item, 3, "-");
                    SetSubItem(item, 4, "-");
                    SetSubItem(item, 5, "-");
                    SetSubItem(item, 6, "-");
                    SetSubItem(item, 7, "-");
                }
                else
                {
                    var stats = file.GetFieldStats(column, path, encoding, assumedType);

                    SetSubItem(item, 3, stats.Minimum?.ToString() ?? "-");
                    SetSubItem(item, 4, stats.Maximum?.ToString() ?? "-");
                    SetSubItem(item, 5, stats.ValuesCount.ToString());
                    SetSubItem(item, 6, stats.NullValuesCount.ToString());
                    SetSubItem(item, 7, stats.UnparsedValuesCount.ToString());

                    //lblAllValues.Text = stats.ValuesCount.ToString();
                    //lblUnparsedValues.Text = stats.UnparsedValuesCount.ToString();
                    //lblNullValues.Text = stats.NullValuesCount.ToString();
                    //lblMin.Text = stats.Minimum?.ToString() ?? "-";
                    //lblMax.Text = stats.Maximum?.ToString() ?? "-";
                }

                lvwHeader.Refresh();
            }

            lvwHeader.SuspendLayout();

            foreach (var col in lvwHeader.Columns.Cast<ColumnHeader>())
            {
                col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                if (col.Width < 80) col.Width = 80;
            }

            lvwHeader.ResumeLayout();

            tstStatus.Text = "OK";
            StopWaiting();
        }

        private void AnalyzeHeaders3(IEnumerable<ListViewItem> items)
        {
            Stopwatch w = Stopwatch.StartNew();

            string p = SourcePath;
            if (p.Length == 0 || !File.Exists(p))
            {
                ShowWarning("The current path is emppty or does not exist!");
                return;
            }

            if (items.Count() == 0) return;

            Wait();

            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;

            //var file = _csvFiles.ReadWholeFile(path, encoding, true);

            List<ListViewItem> list = items.ToList();
            BaseType?[] initialBaseTypes = list.Select(l => (BaseType?)(l.Tag is null ? null : (BaseType)l.Tag)).ToArray();
            BaseType[] baseTypes = new BaseType[list.Count];
            int[] columns = list.Select(l=> int.Parse(l.Text)-1).ToArray();
            CsvFieldStats[] fieldStats = new CsvFieldStats[list.Count];

            tstStatus.Text = "Analyzing data types..."; statusStrip1.Refresh(); Application.DoEvents();

            Parallel.ForEach(Enumerable.Range(0, list.Count), i =>
            {
                var file = _csvFiles.GetFile(path, encoding, true);

                if (initialBaseTypes[i] is null)
                    baseTypes[i] = file.GetBaseType(columns[i], path, encoding, BaseType.Unknown, true);
                else
                    baseTypes[i] = initialBaseTypes[i]!.Value;
            });


            tstStatus.Text = "Retrieving field stats..."; statusStrip1.Refresh(); Application.DoEvents();
         
            Parallel.ForEach(Enumerable.Range(0, list.Count), i =>
            {
                var file = _csvFiles.GetFile(path, encoding, true);

                fieldStats[i] = file.GetFieldStats(columns[i], path, encoding, baseTypes[i], true);
            });


            tstStatus.Text = "Updating listview...";

            for(int i=0;i<list.Count; i++)  
            {
                ListViewItem item = list[i];

                SetDataType(item, baseTypes[i]);

                if (baseTypes[i] == BaseType.Unknown)
                {
                    SetSubItem(item, 3, "-");
                    SetSubItem(item, 4, "-");
                    SetSubItem(item, 5, "-");
                    SetSubItem(item, 6, "-");
                    SetSubItem(item, 7, "-");
                }
                else
                {
                    var stats = fieldStats[i];
                    SetSubItem(item, 3, stats.Minimum?.ToString() ?? "-");
                    SetSubItem(item, 4, stats.Maximum?.ToString() ?? "-");
                    SetSubItem(item, 5, stats.ValuesCount.ToString());
                    SetSubItem(item, 6, stats.NullValuesCount.ToString());
                    SetSubItem(item, 7, stats.UnparsedValuesCount.ToString());

                }

                lvwHeader.Refresh();
            }

            foreach (var col in lvwHeader.Columns.Cast<ColumnHeader>())
            {
                col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                if (col.Width < 80) col.Width = 80;
            }

            tstStatus.Text = "OK";
            StopWaiting();

            w.Stop();
            MessageBox.Show(w.Elapsed.TotalMinutes.ToString("0.0"));
        }

        private void AnalyzeHeaders2(IEnumerable<ListViewItem> items) //read file once
        {
            Stopwatch w = Stopwatch.StartNew();

            string p = SourcePath;
            if (p.Length == 0 || !File.Exists(p))
            {
                ShowWarning("The current path is emppty or does not exist!");
                return;
            }

            if (items.Count() == 0) return;

            Wait();

            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;

            var file = _csvFiles.ReadWholeFile(path, encoding, true);

            List<ListViewItem> list = items.ToList();
            BaseType?[] initialBaseTypes = list.Select(l => (BaseType?)(l.Tag is null ? null : (BaseType)l.Tag)).ToArray();
            BaseType[] baseTypes = new BaseType[list.Count];
            int[] columns = list.Select(l => int.Parse(l.Text) - 1).ToArray();
            CsvFieldStats[] fieldStats = new CsvFieldStats[list.Count];

            tstStatus.Text = "Analyzing data types..."; statusStrip1.Refresh(); Application.DoEvents();

            Parallel.ForEach(Enumerable.Range(0, list.Count), i =>
            {
                if (initialBaseTypes[i] is null)
                    baseTypes[i] = file.GetBaseType(columns[i], BaseType.Unknown, true);
                else
                    baseTypes[i] = initialBaseTypes[i]!.Value;
            });


            tstStatus.Text = "Retrieving field stats..."; statusStrip1.Refresh(); Application.DoEvents();

            Parallel.ForEach(Enumerable.Range(0, list.Count), i =>
            {
                fieldStats[i] = file.GetFieldStats(columns[i], baseTypes[i], true);
            });


            tstStatus.Text = "Updating listview...";

            for (int i = 0; i < list.Count; i++)
            {
                ListViewItem item = list[i];

                SetDataType(item, baseTypes[i]);

                if (baseTypes[i] == BaseType.Unknown)
                {
                    SetSubItem(item, 3, "-");
                    SetSubItem(item, 4, "-");
                    SetSubItem(item, 5, "-");
                    SetSubItem(item, 6, "-");
                    SetSubItem(item, 7, "-");
                }
                else
                {
                    var stats = fieldStats[i];
                    SetSubItem(item, 3, stats.Minimum?.ToString() ?? "-");
                    SetSubItem(item, 4, stats.Maximum?.ToString() ?? "-");
                    SetSubItem(item, 5, stats.ValuesCount.ToString());
                    SetSubItem(item, 6, stats.NullValuesCount.ToString());
                    SetSubItem(item, 7, stats.UnparsedValuesCount.ToString());

                }

                lvwHeader.Refresh();
            }

            foreach (var col in lvwHeader.Columns.Cast<ColumnHeader>())
            {
                col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                if (col.Width < 80) col.Width = 80;
            }

            tstStatus.Text = "OK";
            StopWaiting();

            w.Stop();
            MessageBox.Show(w.Elapsed.TotalMinutes.ToString("0.0"));
        }

        #endregion


        #region Change data types (menu)
        private void mnuBoolean_Click(object sender, EventArgs e)
        {
            foreach (var item in SelectedHeaders)
                SetDataType(item, BaseType.Boolean);
        }

        private void mnuInteger_Click(object sender, EventArgs e)
        {
            foreach (var item in SelectedHeaders)
                SetDataType(item, BaseType.Integer);
        }

        private void mnuLong_Click(object sender, EventArgs e)
        {
            foreach (var item in SelectedHeaders)
                SetDataType(item, BaseType.Long);

        }

        private void mnuFloat_Click(object sender, EventArgs e)
        {
            foreach (var item in SelectedHeaders)
                SetDataType(item, BaseType.Float);

        }

        private void mnuDouble_Click(object sender, EventArgs e)
        {
            foreach (var item in SelectedHeaders)
                SetDataType(item, BaseType.Double);

        }

        private void mnuDateTime_Click(object sender, EventArgs e)
        {
            foreach (var item in SelectedHeaders)
                SetDataType(item, BaseType.DateTime);

        }

        private void mnuDateTimeOffset_Click(object sender, EventArgs e)
        {
            foreach (var item in SelectedHeaders)
                SetDataType(item, BaseType.DateTimeOffset);

        }

        private void mnuString_Click(object sender, EventArgs e)
        {
            foreach (var item in SelectedHeaders)
                SetDataType(item, BaseType.String);

        }
        #endregion

        #region Find data types

        private void btnFindDataTypes_Click(object sender, EventArgs e)
        {
            FindDataTypes(AllHeaders);
        }

        private void mnuFindDataTypeFast_Click(object sender, EventArgs e)
        {
            FindDataTypes(SelectedHeaders, 200);

        }

        private void mnuFindDataType_Click(object sender, EventArgs e)
        {
            FindDataTypes(SelectedHeaders);
        }

        private void FindDataTypes(IEnumerable<ListViewItem> items, int maxRows = int.MaxValue)
        {
            string p = SourcePath;
            if (p.Length == 0 || !File.Exists(p))
            {
                ShowWarning("The current path is empty or does not exist!");
                return;
            }


            if (lvwHeader.SelectedItems.Count == 0) return;

            Wait();
            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;
            var file = _csvFiles.GetFile(txtFilePath.Text, Encoding.UTF8, true);

            foreach (var item in items)
            {
                int column = int.Parse(item.Text) - 1;
                string columnName = item.SubItems[1].Text;
                tstStatus.Text = $"Processing [{columnName}] data..."; statusStrip1.Refresh();

                var dataType = file.GetBaseType(column, path, encoding, BaseType.Unknown, true, maxRows);
                SetDataType(item, dataType);
            }

            tstStatus.Text = "OK";

            StopWaiting();
        }
        #endregion


        private void btnExportSelectedToDatabase_Click(object sender, EventArgs e)
        {
            frmDatabase frm = _provider.GetRequiredService<frmDatabase>();
            frm.SelectedHeaders = SelectedHeaders;
            frm.SourcePath = this.SourcePath;
            frm.Encoding = Encoding.UTF8;
            frm.ShowDialog();
        }

        private void btnExportAllToDatabase_Click(object sender, EventArgs e)
        {
            frmDatabase frm = _provider.GetRequiredService<frmDatabase>();
            frm.SelectedHeaders = AllHeaders;
            frm.SourcePath = this.SourcePath;
            frm.Encoding = Encoding.UTF8;
            frm.ShowDialog();
        }
    }
}