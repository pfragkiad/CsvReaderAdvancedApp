using System.Text;
using CsvReaderAdvanced.Schemas;

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
        bool ConfirmAssumedType(int column, string path, Encoding encoding, BaseType assumedType, bool hasHeader = true, int maxRows = int.MaxValue);
        void Dispose();
        BaseType GetBaseType(int column, string path, Encoding encoding, BaseType assumedType = BaseType.Unknown, bool hasHeader = true, int maxRows = int.MaxValue);
        void PopulateColumns();
        IEnumerable<TokenizedLine?> Read(string path, Encoding encoding, bool skipHeader);
        void ReadFromFile(string path, Encoding encoding, bool withHeader);
        void ReadHeader(string path, Encoding encoding);
        void Reset();
        Task SavePartialAs(string sourcePath, string targetPath, char targetSeparator, Encoding encoding, params int[] columns);
        Task SavePartialAs(string sourcePath, string targetPath, char targetSeparator, Encoding encoding, params string[] columnNames);
    }
}