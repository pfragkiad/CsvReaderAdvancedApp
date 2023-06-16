namespace CsvReaderAdvanced;

public struct TokenizedLine
{
    public int? FromLine { get; init; }
    public int? ToLine { get; init; }

    public List<string> Tokens { get; init; }

    public string? TrailingQuotedItem { get; init; }

    public bool IsIncomplete { get; init; }

    public override string ToString()
    {
        string line = "";
        if (FromLine is not null && ToLine is not null)
            line = FromLine != ToLine ? $"Lines: {FromLine}-{ToLine}, " : $"Line: {FromLine}, ";
        else if (FromLine is not null)
            line = $"Line: {FromLine}, ";

        string incomplete = IsIncomplete ? ", Incomplete" :"";
        return $"{line}Tokens: {Tokens.Count}{incomplete}";
    }
}
