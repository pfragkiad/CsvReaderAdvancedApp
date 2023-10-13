using CsvReaderAdvanced;
using CsvReaderAdvanced.Interfaces;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CsvWinAnalyzer
{
    public partial class frmMain : Form
    {
        private readonly IServiceProvider _provider;

        public frmMain(IServiceProvider provider)
        {
            InitializeComponent();

            _provider = provider;

            //initialize base on settings
            txtFilePath.Text = FormSettings.Default.Filepath;
            ReadHeader();

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
            var file = _provider.GetCsvFile();
            file.ReadHeader(txtFilePath.Text, Encoding.UTF8);

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

        private void mnuFindDataType_Click(object sender, EventArgs e)
        {
            if (lvwHeader.SelectedItems.Count == 0) return;

            string path = txtFilePath.Text;
            var encoding = Encoding.UTF8;

            var file = _provider.GetCsvFile();
            file.ReadHeader(path, encoding);

            int maxRows = 100;

            lvwHeader.SuspendLayout();

            foreach (ListViewItem item in lvwHeader.SelectedItems)
            {
                int column = int.Parse(item.Text) - 1;

                file.Reset(); //reset in order to start reading from start

                int scenario = -1, iRow = 0;
                foreach (var line in file.Read(path, encoding, true))
                {
                    if (line is null) continue;
                    var tokenizedLine = line.Value;

                    if (tokenizedLine.Tokens[column].Length > 0 && scenario == -1)
                        scenario = 0;

                    iRow++;
                    if (iRow > maxRows) break;

                    //check from stricter (int) to less stricter (string)
                    //check for int
                    if (scenario == 0) //int
                    {
                        if (tokenizedLine.GetInt(column).IsParsed) continue;
                        scenario++;
                    }

                    if (scenario == 1) //float
                    {
                        if (tokenizedLine.GetFloat(column, CultureInfo.InvariantCulture).IsParsed) continue;
                        scenario++;
                    }

                    if (scenario == 2) //date
                    {
                        if (tokenizedLine.GetDateTime(column, CultureInfo.InvariantCulture, "yyyy-MM-dd").IsParsed) continue;
                        scenario++;
                    }

                    //else if (scenario==4) //string
                    //    if (tokenizedLine.GetDateTime(column, CultureInfo.InvariantCulture,"yyyy-MM-dd").IsParsed) continue;


                }

                string dataType = scenario switch
                {
                    -1 => "<unknown>",
                    0 => "int",
                    1 => "float",
                    2 => "date",
                    _ => "string"
                };


                if (item.SubItems.Count < 3)
                    item.SubItems.Add(dataType);
                else
                    item.SubItems[2].Text = dataType;
            }

            lvwHeader.ResumeLayout();

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

            foreach (ListViewItem item in lvwHeader.SelectedItems)
            {
                int column = int.Parse(item.Text) - 1;
                string dataType = item.SubItems[2].Text;

                if (dataType != "string") continue;

                string path = txtFilePath.Text;
                var encoding = Encoding.UTF8;

                int max = 0;

                var file = _provider.GetCsvFile();
                foreach (var line in file.Read(path, encoding, true))
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







            //int
            foreach (ListViewItem item in lvwHeader.SelectedItems)
            {
                int column = int.Parse(item.Text) - 1;
                string dataType = item.SubItems[2].Text;

                if (dataType != "int") continue;

                string path = txtFilePath.Text;
                var encoding = Encoding.UTF8;

                int max = 0;
                var file = _provider.GetCsvFile();
                foreach (var line in file.Read(path, encoding, true))
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
            tstStatus.Text = "OK";
            Cursor.Current = Cursors.Default;
        }



        //TODO: Ensure data type (run all lines)






    }
}