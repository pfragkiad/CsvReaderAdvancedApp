using System.Diagnostics;
using System.Text;
using CsvReaderAdvanced.Interfaces;

namespace CsvReaderAdvanced;

public class CsvReader : ICsvReader
{
    //internal static void TestsFirst()
    //{
    //    string[] lines = new string[] {
    //    "1.2;keftes;test",
    //    ";1.2;keftes;;",
    //    "1;\"kdsfa;jai  kasdf, fsdasfdsfasfasf\"",
    //    "1;\"k\"\"tef\"\"sf\"",
    //    "1;\"k\";9",
    //     "1;\";k;\";",
    //     "1;\";k;"
    //    };

    //    List<TokenizedLine> lines2 = new List<TokenizedLine>();
    //    //for (int selected = 0; selected < lines.Length; selected++)
    //    for (int selected = 6; selected < 7; selected++)
    //    {
    //        string line = lines[selected];
    //        lines2.Add(GetTokenizedLine(line, selected + 1)!.Value);
    //    }

    //    Debugger.Break();
    //}

    //public void Tests()
    //{
    //    string path = "d:\\Desktop\\HAGERO_FY2022_api_address.csv"; // "D:\\repos\\pfragkiad\\glec\\CsvReaderAdvanced\\samples\\hard2.csv";


    //    //char? separator = ReadSeparator(path,Encoding.UTF8);
    //    //if (separator is null) Console.WriteLine("Could not read file");

    //    //using StreamReader reader = new StreamReader(path, Encoding.UTF8);
    //    //var lines = GetTokenizedLines(reader,separator!.Value).ToList();



    //    //bool allSameTokensCount = file.Lines.All(l => l.Value.Tokens.Count == file.Header.Value.Tokens.Count);

    //    Debugger.Break();
    //}

    public char? ReadSeparator(string path, Encoding encoding)
    {
        using StreamReader reader = new StreamReader(path, encoding);
        return ReadSeparator(reader);
    }

    public char? ReadSeparator(StreamReader reader)
    {
        string? line = reader.ReadLine();
        if (line is null) return null;

        char[] candidates = new char[] { ';', ',', '\t' };

        var stats = candidates.Select(s => (Separator: s, Count: line.Count(c => c == s))).OrderByDescending(sc => sc.Count);
        return stats.First().Separator;
    }

    public IEnumerable<TokenizedLine?> GetTokenizedLines(
        StreamReader reader,
        char separator = ';',
        char quote = '"',
        bool omitEmptyEntries = false,
        int startLineBeforeRead = 0)
    {
        int iLine = startLineBeforeRead;
        while (!reader.EndOfStream)
        {
            iLine++;
            string? line = reader.ReadLine();
            if (line == "") continue; //omit empty lines
            if (line is null) break;

            int iStartLine = iLine;
            var currentTokens = GetTokenizedLine(line, iStartLine, iStartLine, null, separator, quote, omitEmptyEntries);
            while (currentTokens!.Value.IsIncomplete && !reader.EndOfStream)
            {
                iLine++; line = reader.ReadLine();
                if (line == "") continue; //omit empty lines
                currentTokens = GetTokenizedLine(line, iStartLine, iLine, currentTokens, separator, quote, omitEmptyEntries);
            }

            yield return currentTokens!;
        }
    }

    public TokenizedLine? GetTokenizedLine(
        string? line,
        int? startLine = null,
        int? endLine = null,
        TokenizedLine? previousIncompleteTokenizedLine = null,
        char separator = ';',
        char quote = '"',
        bool omitEmptyEntries = false)
    {
        if (endLine is null) endLine = startLine;
        if (line is null) return previousIncompleteTokenizedLine;

        //https://techterms.com/definition/csv#:~:text=Since%20CSV%20files%20use%20the,double%20quote%20marks%20the%20end.

        //copy previous tokens if they are passed
        List<string> tokens = previousIncompleteTokenizedLine?.Tokens ?? new List<string>();

        int currentTokenStart = 0;
        bool isCurrentTokenQuoted = previousIncompleteTokenizedLine?.IsIncomplete ?? false;
        string quotedToken = previousIncompleteTokenizedLine?.TrailingQuotedItem ?? "";
        bool lastAddedItemWasQuoted = false;

        int len = line.Length;
        for (int i = 0; i < len; i++)
        {
            char c = line[i];

            if (c == quote)
                if (!isCurrentTokenQuoted)
                {
                    //initialize new quoted token
                    isCurrentTokenQuoted = true;
                    quotedToken = "";
                }
                else //there is already a quote open -> always retrieve next character here to decide properly 
                {
                    i++;

                    //then it is a last token quote
                    if (i == len && (quotedToken.Length > 0 || !omitEmptyEntries))
                    {
                        tokens.Add(quotedToken);
                        quotedToken = "";
                        lastAddedItemWasQuoted = true;
                        break;
                    }
                    else  //check for the next token if it is another quote of separator
                    {
                        c = line[i];
                        if (c == quote)// "" case
                            quotedToken += "\"";
                        //last quote
                        else if (c == separator && (quotedToken.Length > 0 || !omitEmptyEntries))
                        {
                            tokens.Add(quotedToken);
                            quotedToken = "";
                            isCurrentTokenQuoted = false; //reset flag (for error checking only)

                            currentTokenStart = i + 1;
                        }
                    }
                }

            else if (c == separator)
            {
                if (!isCurrentTokenQuoted)
                {
                    if (i > currentTokenStart || !omitEmptyEntries)
                        tokens.Add(line[currentTokenStart..i]);

                    currentTokenStart = i + 1;
                }
                else quotedToken += c;
            }
            else if (isCurrentTokenQuoted)
                quotedToken += c;
        }
        if (!lastAddedItemWasQuoted && !isCurrentTokenQuoted && (currentTokenStart != line.Length || !omitEmptyEntries))
            tokens.Add(line[currentTokenStart..line.Length]);

        return new TokenizedLine()
        {
            FromLine = startLine,
            ToLine = endLine,
            Tokens = tokens,
            TrailingQuotedItem = quotedToken,
            IsIncomplete = isCurrentTokenQuoted
        };
    }
}
