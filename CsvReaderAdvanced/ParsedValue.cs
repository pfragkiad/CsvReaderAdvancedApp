namespace CsvReaderAdvanced;

//public struct Unparsable { public static Unparsable Default = new Unparsable(); }

public readonly struct ParsedValue<T>
{
    //T? is not allowed as a value here
    public T Value { get; init; }

    public string StringValue { get; init; }

    public bool IsParsed { get; init; }

    public bool IsNull { get; init; }

    public ParsedValue(T value, string stringValue)
    {
        Value = value; IsParsed = true; IsNull = false;
        StringValue = stringValue;
    }

    public static ParsedValue<T> Unparsable(string stringValue) => new ParsedValue<T>() { IsNull = false, IsParsed = false, Value = default , StringValue = stringValue};

    public static readonly ParsedValue<T> Null = new ParsedValue<T>() { IsNull = true, IsParsed = true, Value = default, StringValue="" };

    public override string ToString()
    {
        return $"'{StringValue}' -> {Value} ({(IsParsed ? "parsed" : "cannot parse")})";
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
