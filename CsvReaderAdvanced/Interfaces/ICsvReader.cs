using System.Text;

namespace CsvReaderAdvanced.Interfaces;

public interface ICsvReader
{
    TokenizedLine? GetTokenizedLine(string? line, int? startLine = null, int? endLine = null, TokenizedLine? previousIncompleteTokenizedLine = null, char separator = ';', char quote = '"', bool omitEmptyEntries = false);
    IEnumerable<TokenizedLine?> GetTokenizedLines(StreamReader reader, char separator = ';', char quote = '"', bool omitEmptyEntries = false, int startLineBeforeRead = 0);
    char? ReadSeparator(StreamReader reader);
    char? ReadSeparator(string path, Encoding encoding);
}