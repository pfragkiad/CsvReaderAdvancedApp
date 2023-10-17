using CsvReaderAdvanced.Files;

using CsvReaderAdvanced.Schemas;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;

namespace CsvWinAnalyzer
{
    public partial class frmMain : Form
    {
        private readonly IServiceProvider _provider;
        private readonly CsvFileFactory _fileFactory;

        public frmMain(
            IServiceProvider provider,
            CsvFileFactory csvFiles)
        {
            InitializeComponent();

            _provider = provider;
            _fileFactory = csvFiles;

            //initialize base on settings
            txtFilePath.Text = FormSettings.Default.Filepath;
            if (txtFilePath.Text.Length > 0 && File.Exists(txtFilePath.Text)) ReadHeader();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            FormSettings.Default.Filepath = txtFilePath.Text;
            FormSettings.Default.Save();
            base.OnClosing(e);
        }



        #region File loading

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

        private void ReadHeader()
        {
            var file = _fileFactory.GetFile(txtFilePath.Text, Encoding.UTF8, true);

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
        #endregion


        #region ListView functions

        private static void SetSubItem(ListViewItem item, int index, string text)
        {
            if (item.SubItems.Count < index + 1)
                item.SubItems.Add(text);
            else
                item.SubItems[index].Text = text;
        }

        private static void SetDataType(ListViewItem item, BaseType baseType)
        {
            SetSubItem(item, 2, baseType.ToString());
            item.ForeColor = baseType == BaseType.Unknown ? Color.Red : Color.Blue;

            if (item.Tag is not null)
            {
                CsvFieldTypeInfo info = (CsvFieldTypeInfo)item.Tag;
                info.BaseType = baseType;
            }
            else
                item.Tag = new CsvFieldTypeInfo() { Column = int.Parse(item.Text) - 1, BaseType = baseType };
        }


        private static void SetDataType(ListViewItem item, CsvFieldTypeInfo info)
        {
            SetSubItem(item, 2, info.BaseType.ToString());
            item.ForeColor = info.BaseType == BaseType.Unknown ? Color.Red : Color.Blue;
            item.Tag = info;
        }

        #endregion

        #region Main properties

        private string SourcePath { get => txtFilePath.Text; }
        private List<ListViewItem> SelectedHeaders { get => lvwHeader.SelectedItems.Cast<ListViewItem>().ToList(); }

        private List<ListViewItem> AllHeaders { get => lvwHeader.Items.Cast<ListViewItem>().ToList(); }

        #endregion


        #region ListView events


        private void lvwHeader_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control == true)
                foreach (ListViewItem item in lvwHeader.Items)
                    item.Selected = true;
        }

        private void lvwHeader_DoubleClick(object sender, EventArgs e)
        {
            AnalyzeHeaders(SelectedHeaders);
        }

        #endregion


        //TODO: Ensure data type (run all lines)




        #region Analyze fields



        private void mnuAnalyze_Click(object sender, EventArgs e)
        {
            AnalyzeHeaders(SelectedHeaders);
        }

        private void btnAnalyzeAllField_Click(object sender, EventArgs e)
        {
            AnalyzeHeaders(AllHeaders);
        }


        Dictionary<ListViewItem, CsvFieldTypeInfo> fieldStats = new();
        private void AnalyzeHeaders(IEnumerable<ListViewItem> items)
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

            fieldStats = items.ToDictionary(
                l => l,
                l => l.Tag is not null ? (CsvFieldTypeInfo)l.Tag :
                    new CsvFieldTypeInfo()
                    {
                        Column = int.Parse(l.Text) - 1,
                        BaseType = BaseType.Unknown
                    });

            var file = _fileFactory.GetFile(path, encoding, true);

            tstStatus.Text = "Analyzing data types..."; statusStrip1.Refresh(); Application.DoEvents();
            file.UpdateFieldBaseTypes(fieldStats.Values);

            tstStatus.Text = "Retrieving field stats..."; statusStrip1.Refresh(); Application.DoEvents();
            file.UpdateFieldStats(fieldStats.Values);

            tstStatus.Text = "Updating listview...";

            lvwHeader.SuspendLayout();
            foreach (var entry in fieldStats)
            {
                ListViewItem item = entry.Key;

                var stats = entry.Value;
                SetDataType(item, stats);

                if (stats.BaseType == BaseType.Unknown)
                {
                    SetSubItem(item, 3, "-");
                    SetSubItem(item, 4, "-");
                    SetSubItem(item, 5, "-");
                    SetSubItem(item, 6, "-");
                    SetSubItem(item, 7, "-");
                }
                else
                {
                    SetSubItem(item, 3, stats.Minimum?.ToString() ?? "-");
                    SetSubItem(item, 4, stats.Maximum?.ToString() ?? "-");
                    SetSubItem(item, 5, stats.ValuesCount.ToString());
                    SetSubItem(item, 6, stats.NullValuesCount.ToString());
                    SetSubItem(item, 7, stats.UnparsedValuesCount.ToString());
                }
            }
            lvwHeader.ResumeLayout();

            tstStatus.Text = "Ready";
            StopWaiting();

            w.Stop(); ShowInfo($"Analyzed in {w.Elapsed.TotalMinutes:0.00} minutes.");
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

        private void mnuFindDataTypesAndRanges_Click(object sender, EventArgs e)
        {
            AnalyzeHeaders(AllHeaders);
        }

        private void mnuFindDataTypes_Click(object sender, EventArgs e)
        {
            AnalyzeDataTypes(AllHeaders);
        }


        private void mnuFindDataTypeFast_Click(object sender, EventArgs e)
        {
            AnalyzeDataTypes(SelectedHeaders, 200);

        }

        private void mnuFindDataType_Click(object sender, EventArgs e)
        {
            AnalyzeDataTypes(SelectedHeaders);
        }

        private void AnalyzeDataTypes(IEnumerable<ListViewItem> items, int maxRows = int.MaxValue)
        {
            string p = SourcePath;
            if (p.Length == 0 || !File.Exists(p))
            {
                ShowWarning("The current path is empty or does not exist!");
                return;
            }
            if (items.Count() == 0) return;

            Wait();


            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;


            Dictionary<ListViewItem, CsvFieldTypeInfo> fieldStats = items.ToDictionary(
                l => l,
                l =>
                new CsvFieldTypeInfo()
                {
                    Column = int.Parse(l.Text) - 1,
                    BaseType = l.Tag is null ? BaseType.Unknown : (BaseType)l.Tag
                });


            var file = _fileFactory.GetFile(path, encoding, true);

            tstStatus.Text = "Analyzing data types..."; statusStrip1.Refresh(); Application.DoEvents();
            file.UpdateFieldBaseTypes(fieldStats.Values, maxRows);

            lvwHeader.SuspendLayout();
            foreach (var entry in fieldStats)
            {
                ListViewItem item = entry.Key;
                var stats = entry.Value;
                SetDataType(item, stats);
            }

            tstStatus.Text = "Ready";

            StopWaiting();
        }


        #endregion

        #region Export partial to CSV/database

        private string? GetTargetPath(string suffix = "_filtered")
        {
            string p = SourcePath;
            if (p.Length > 0 && !File.Exists(p)) return null;

            return Path.Combine(Path.GetDirectoryName(p), Path.GetFileNameWithoutExtension(p) + suffix + Path.GetExtension(p));
        }


        private void ExportPartialCsv()
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

                var file = _fileFactory.GetFile(p, Encoding.Default, true);
                file.SavePartialAs(d.FileName, ';', headers.Select(c => int.Parse(c.Text) - 1).ToArray());
                StopWaiting();
                ShowInfo("Successfully exported!");
            }
            catch (Exception ex)
            {
                StopWaiting();
                ShowError($"Error: {ex.Message}");
            }
        }

        private void ExportToDatabase(IEnumerable<ListViewItem> items)
        {
            frmDatabase frm = _provider.GetRequiredService<frmDatabase>();
            frm.SourcePath = this.SourcePath;
            frm.Encoding = Encoding.UTF8;

            frm.SelectedHeaders = items.Where(l => l.Tag is not null).Select(l => (CsvFieldTypeInfo)l.Tag).ToList();
            frm.ShowDialog();
        }


        private void btnExportSelectedToDatabase_Click(object sender, EventArgs e)
        {
            ExportToDatabase(SelectedHeaders);
        }

        private void mnuExportAllToDatabase_Click(object sender, EventArgs e)
        {
            ExportToDatabase(AllHeaders);
        }

        private void mnuExportSelectedToDatabase_Click(object sender, EventArgs e)
        {
            ExportToDatabase(SelectedHeaders);
        }

        private void mnuExportSelectedToCsvFile_Click(object sender, EventArgs e)
        {
            ExportPartialCsv();
        }

        #endregion

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }




    }
}