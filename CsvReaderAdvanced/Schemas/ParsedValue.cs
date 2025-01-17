using Microsoft.Extensions.Primitives;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace CsvReaderAdvanced.Schemas;

//public struct Unparsable { public static Unparsable Default = new Unparsable(); }
public enum ParseState { Unknown, Parsed, Unparsable, NaN, Null }


public readonly struct ParsedValue<T> where T : struct
{
    //T? is not allowed as a value here
    public T? Value { get; init; } = null;

    public ParseState State { get; init; } = ParseState.Unknown;

    public bool IsParsed => State == ParseState.Parsed;
    public bool IsNull => State == ParseState.Null;

    public bool IsNaN => State == ParseState.NaN;

    public string StringValue { get; init; } = default!;

    //public bool IsParsed { get; init; }

    //public bool IsNull { get; init; }

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
        State = ParseState.Parsed;
        Value = value;
        StringValue = stringValue;
    }

    public static ParsedValue<T> Unparsable(string stringValue) => new() { State = ParseState.Unparsable, StringValue = stringValue  };
    public static ParsedValue<T> NaN(string stringValue) => new(){ State = ParseState.NaN, StringValue   = stringValue };

    public static readonly ParsedValue<T> Null = new() { State = ParseState.Null };


    public override string ToString()
    {
        return $"'{StringValue}' -> {Value} (State: {State})";
    }
}


