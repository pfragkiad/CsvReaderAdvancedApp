using CsvReaderAdvanced;
using CsvReaderAdvanced.Interfaces;
using CsvReaderAdvanced.Schemas;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CsvWinAnalyzer
{
    public partial class frmMain : Form
    {
        //private readonly IServiceProvider _provider;
        private readonly CsvFileFactory _csvFiles;

        public frmMain(
            IServiceProvider provider,
            CsvFileFactory csvFiles)
        {
            InitializeComponent();

            //_provider = provider;
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

        private void CountLines()
        {
            int count = 0; int nonEmptyLines = 0;
            using StreamReader reader = new StreamReader(txtFilePath.Text, Encoding.UTF8);
            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                if (line is null) break;
                count++;
                if (line.Length > 0) nonEmptyLines++;
            }
            tstStatus.Text = $"Lines: {count}, Non-empty Lines: {nonEmptyLines}";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            FormSettings.Default.Filepath = txtFilePath.Text;
            FormSettings.Default.Save();
            base.OnClosing(e);
        }





        private void btnReadHeader_Click(object sender, EventArgs e)
        {
            ReadHeader();


            //file.ReadFromFile(txtFilePath.Text, System.Text.Encoding.UTF8);            
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

        private void mnuFindDataType_Click(object sender, EventArgs e)
        {
            if (lvwHeader.SelectedItems.Count == 0) return;

            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;

            var file = _csvFiles.GetFile(txtFilePath.Text, Encoding.UTF8, true);

            int maxRows = 100;

            lvwHeader.SuspendLayout();

            foreach (var item in SelectedHeaders)
            {
                int column = int.Parse(item.Text) - 1;
                var dataType = file.GetBaseType(column, path, encoding, BaseType.Unknown, true, maxRows);
                SetDataType(item, dataType.ToString());
            }
            lvwHeader.ResumeLayout();

        }

        private static void SetDataType(ListViewItem item, string dataType)
        {
            if (item.SubItems.Count < 3)
                item.SubItems.Add(dataType);
            else
                item.SubItems[2].Text = dataType;
        }

        private void lvwHeader_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control == true)
                foreach (ListViewItem item in lvwHeader.Items)
                    item.Selected = true;
        }

        private void mnuFindMax_Click(object sender, EventArgs e)
        {
            tstStatus.Text = "Processing..."; statusStrip1.Refresh();
            Cursor.Current = Cursors.WaitCursor;
            CheckForStringMax();

            CheckForIntMax();

            CheckForFloatMax();

            tstStatus.Text = "OK";
            Cursor.Current = Cursors.Default;
        }

        private void CheckForIntMax()
        {
            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;

            //int
            foreach (ListViewItem item in lvwHeader.SelectedItems)
            {
                int column = int.Parse(item.Text) - 1;
                string dataType = item.SubItems[2].Text;

                if (dataType != "int") continue;

                int max = 0;
                var lines = _csvFiles.ReadFile(path, encoding, true);
                foreach (var line in lines)
                {
                    if (line is null) continue;
                    var tokenizedLine = line.Value;

                    var l = tokenizedLine.GetInt(column);
                    if (!l.IsParsed) throw new InvalidOperationException("Wrong data type!");
                    if (l.Value > max) max = l;
                }

                if (item.SubItems.Count < 4)
                    item.SubItems.Add(max.ToString());
                else
                    item.SubItems[2].Text = max.ToString();
                lvwHeader.Refresh();
            }
        }

        private void CheckForFloatMax()
        {
            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;

            //int
            foreach (ListViewItem item in lvwHeader.SelectedItems)
            {
                int column = int.Parse(item.Text) - 1;
                string dataType = item.SubItems[2].Text;

                if (dataType != "float") continue;

                float max = 0;
                var lines = _csvFiles.ReadFile(path, encoding, true);
                foreach (var line in lines)
                {
                    if (line is null) continue;
                    var tokenizedLine = line.Value;

                    var l = tokenizedLine.GetFloat(column);
                    if (!l.IsParsed) throw new InvalidOperationException("Wrong data type!");
                    if (l.Value > max) max = l;
                }

                if (item.SubItems.Count < 4)
                    item.SubItems.Add(max.ToString());
                else
                    item.SubItems[2].Text = max.ToString();
                lvwHeader.Refresh();
            }
        }

        private void CheckForStringMax()
        {
            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;

            foreach (ListViewItem item in lvwHeader.SelectedItems)
            {
                int column = int.Parse(item.Text) - 1;
                string dataType = item.SubItems[2].Text;

                if (dataType != "string") continue;

                int max = 0;

                var lines = _csvFiles.ReadFile(path, encoding, true);
                foreach (var line in lines)
                {
                    if (line is null) continue;
                    var tokenizedLine = line.Value;

                    int l = tokenizedLine.Tokens[column].Length;
                    if (l > max) max = l;
                }

                if (item.SubItems.Count < 4)
                    item.SubItems.Add(max.ToString());
                else
                    item.SubItems[2].Text = max.ToString();
                lvwHeader.Refresh();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string p = SourcePath;
            if (p.Length > 0 && !File.Exists(p))
            {
                ShowWarning("The current path does not exist!");
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
        private List<int> SelectedColumns { get => SelectedHeaders.Select(l => int.Parse(l.Text) + 1).ToList(); }


        #region Utility functions

        void ShowInfo(string message) =>
            MessageBox.Show(message, "CSV Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Information);

        void ShowWarning(string message) =>
            MessageBox.Show(message, "CSV Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        void ShowError(string message) =>
            MessageBox.Show(message, "CSV Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Error);

        void Wait() { Cursor.Current = Cursors.WaitCursor; Application.UseWaitCursor = true; }
        void StopWaiting() { Cursor.Current = Cursors.Default; Application.UseWaitCursor = false; }


        #endregion
    }
}