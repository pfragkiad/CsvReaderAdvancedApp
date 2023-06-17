using System.Text;

namespace CsvReaderAdvanced.Interfaces
{
    public interface ICsvFile
    {
        Dictionary<string, int> ExistingColumns { get; }
        Dictionary<string, int> ExistingFieldColumns { get; }
        TokenizedLine? Header { get; }
        List<TokenizedLine?>? Lines { get; }
        HashSet<CsvField> MissingFields { get; }
        HashSet<CsvField> MissingRequiredFields { get; }
        char? Separator { get; }

        void CheckAgainstSchema(CsvSchema schema);
        void PopulateColumns();
        void ReadFromFile(string path, Encoding encoding, bool withHeader = true);
    }
}