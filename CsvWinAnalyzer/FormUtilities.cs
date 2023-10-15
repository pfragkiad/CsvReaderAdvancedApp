global using static CsvWinAnalyzer.FormUtilities;

namespace CsvWinAnalyzer;

public static class FormUtilities
{
    public static void ShowInfo(string message) =>
          MessageBox.Show(message, "CSV Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static void ShowWarning(string message) =>
        MessageBox.Show(message, "CSV Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    public static DialogResult ShowWarningAndAsk(string message) =>
        MessageBox.Show(message, "CSV Analyzer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

    public static void ShowError(string message) =>
        MessageBox.Show(message, "CSV Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void Wait() { Cursor.Current = Cursors.WaitCursor; Application.UseWaitCursor = true; }
    public static void StopWaiting() { Cursor.Current = Cursors.Default; Application.UseWaitCursor = false; }
}
