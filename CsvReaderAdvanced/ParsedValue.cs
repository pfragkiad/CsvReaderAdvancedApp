namespace CsvReaderAdvanced;

//public struct Unparsable { public static Unparsable Default = new Unparsable(); }

public struct ParsedValue<T> //where T:IConvertible
{
    public T? Value { get; init; }

    public bool Parsed { get; init; }

    /// <summary>
    /// Returns true if a true empty value is read, i.e.an empty string in the input file.
    /// </summary>
    public bool HasNullValue => Value is null && Parsed;

    public ParsedValue(object? value) { Value = (T?)value; Parsed = true; }

    public ParsedValue() { Value = default(T?); Parsed = false; }

    public static readonly ParsedValue<T> Unparsable = new ParsedValue<T>();

    public static readonly ParsedValue<T> Null = new ParsedValue<T>(null);

    public override string ToString()
    {
        return $"{Value} ({(Parsed?"parsed":"cannot parse")})";
    }
}

//public class LineValues
//{
//    protected readonly Dictionary<string, int> _columns;
//    protected readonly TokenizedLine _line;

//    public Dictionary<string, object> Values { get; init; } = new();

//    public LineValues(Dictionary<string, int> columns, TokenizedLine line)
//    {
//        _columns = columns;
//        _line = line;
//    }
//}
