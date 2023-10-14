using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace CsvReaderAdvanced;

//public struct Unparsable { public static Unparsable Default = new Unparsable(); }

public readonly struct ParsedValue<T> where T : struct
{
    //T? is not allowed as a value here
    public T? Value { get; init; } = null;

    public string StringValue { get; init; } = default!;

    public bool IsParsed { get; init; }

    public bool IsNull => Value is null && !IsParsed; 

    public static implicit operator T?(ParsedValue<T> v)
    {
        return v.Value;
    }

    public static implicit operator T(ParsedValue<T> v)
    {
        return v.Value ?? default;
    }

    public ParsedValue(T? value, string stringValue)
    {
        Value = value;
        IsParsed = true;// IsNull = false;
        StringValue = stringValue;
    }

    public static ParsedValue<T> Unparsable(string stringValue) => new ParsedValue<T>() { IsParsed = false, StringValue = stringValue };

    public static readonly ParsedValue<T> Null = new ParsedValue<T>() { IsParsed = true };

    public override string ToString()
    {
        return $"'{StringValue}' -> {Value} ({(IsParsed ? "parsed" : "cannot parse")})";
    }
}


