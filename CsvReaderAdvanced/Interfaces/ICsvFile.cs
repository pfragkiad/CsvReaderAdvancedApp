using System.Text;

namespace CsvReaderAdvanced;

public interface ICsvFile
{
    Dictionary<string, int> AllFieldColumns { get; }
    Dictionary<string, int> ExistingColumns { get; }
    List<string> ExistingFieldNames { get; }
    TokenizedLine? Header { get; }
    List<TokenizedLine?> Lines { get; }
    List<string> MissingFieldNames { get; }
    char? Separator { get; }
    List<CsvField> MissingRequiredFields { get; }

    void CheckAgainstSchema(CsvSchema schema);
    void PopulateColumns();
    void ReadFromFile(string path, Encoding encoding, bool withHeader = true);
}