using System.Globalization;

namespace CsvReaderAdvanced;

public struct TokenizedLine
{
    public int? FromLine { get; init; }
    public int? ToLine { get; init; }

    public List<string> Tokens { get; init; }

    public string? TrailingQuotedItem { get; init; }

    public bool IsIncomplete { get; init; }

    #region Values

    //public Dictionary<string, string>? StringValues { get; private set; }
    ////public Dictionary<string, object>? Values { get; protected set; }

    //public void UpdateValues(Dictionary<string, int> columns)
    //{
    //    StringValues = columns.ToDictionary(c => c.Key, c => Tokens[c.Value]);
    //}

    static readonly CultureInfo _en = CultureInfo.InvariantCulture;
    //public object? GetValue<T>(string f, Dictionary<string,int> columns, string? format= null, CultureInfo? info = null) where T : struct
    //public readonly ParsedValue<T> Get<T>(string fieldName, Dictionary<string, int> columns, string? format = null, CultureInfo? info = null)
    //{
    //    info ??= _en;
    //    string sValue = Tokens[columns[fieldName]];
    //    if (sValue == "") return ParsedValue<T>.Null;

    //    if (typeof(T) == typeof(string))
    //        //return sValue;
    //        return new ParsedValue<T>(sValue);

    //    if (typeof(T) == typeof(double))
    //    {
    //        bool parsed = double.TryParse(sValue, info, out var doubleValue);
    //        return parsed ? new ParsedValue<T>(doubleValue) : ParsedValue<T>.Unparsable;
    //        //return parsed ? doubleValue : Unparsable.Default;

    //    }
    //    if (typeof(T) == typeof(int))
    //    {
    //        bool parsed = int.TryParse(sValue, info, out int intValue);
    //        return parsed ? new ParsedValue<T>(intValue) : ParsedValue<T>.Unparsable;
    //        //return parsed ? intValue : Unparsable.Default;
    //    }
    //    if (typeof(T) == typeof(float))
    //    {
    //        bool parsed = float.TryParse(sValue, info, out var floatValue);
    //        return parsed ? new ParsedValue<T>(floatValue) : ParsedValue<T>.Unparsable;
    //        //return parsed ? floatValue : Unparsable.Default;
    //    }

    //    if (typeof(T) == typeof(bool))
    //    {
    //        sValue = sValue.ToLower();
    //        bool isTrue = sValue == "yes" || sValue == "true" || sValue == "1" || sValue == "-1" || sValue == "oui";
    //        bool isFalse = sValue == "no" || sValue == "false" || sValue == "0" || sValue == "non";
    //        if (isTrue) return new ParsedValue<T>(true); //true;
    //        if (isFalse) return new ParsedValue<T>(false);// false;
    //        return ParsedValue<T>.Unparsable; //Unparsable.Default;
    //    }

    //    if (typeof(T) == typeof(DateTimeOffset))
    //    {
    //        //sValue = sValue.Replace("/", "-");
    //        DateTimeOffset dateTimeOffsetValue;
    //        bool parsed =
    //            format is not null ?
    //            DateTimeOffset.TryParseExact(sValue, format, info, DateTimeStyles.None, out dateTimeOffsetValue) :
    //            DateTimeOffset.TryParse(sValue, info, out dateTimeOffsetValue);
    //        return parsed ? new ParsedValue<T>(dateTimeOffsetValue) : ParsedValue<T>.Unparsable;
    //        //return parsed ? dateTimeOffsetValue : Unparsable.Default;
    //    }

    //    if (typeof(T) == typeof(DateTime))
    //    {
    //        DateTime dateTimeValue;
    //        bool parsed =
    //            format is not null ?
    //            DateTime.TryParseExact(sValue, format, info, DateTimeStyles.None, out dateTimeValue) :
    //            DateTime.TryParse(sValue, info, out dateTimeValue);
    //        //return parsed ? dateTimeValue : Unparsable.Default;
    //        return parsed ? new ParsedValue<T>(dateTimeValue) : ParsedValue<T>.Unparsable;
    //    }

    //    if (typeof(T) == typeof(decimal))
    //    {
    //        bool parsed = decimal.TryParse(sValue, info, out var decimalValue);
    //        return parsed ? new ParsedValue<T>(decimalValue) : ParsedValue<T>.Unparsable;
    //        //return parsed ? decimalValue : Unparsable.Default;
    //    }

    //    if (typeof(T) == typeof(long))
    //    {
    //        bool parsed = long.TryParse(sValue, info, out long longValue);
    //        return parsed ? new ParsedValue<T>(longValue) : ParsedValue<T>.Unparsable;
    //        //return parsed ? intValue : Unparsable.Default;
    //    }
    //    if (typeof(T) == typeof(byte))
    //    {
    //        bool parsed = byte.TryParse(sValue, info, out byte byteValue);
    //        return parsed ? new ParsedValue<T>(byteValue) : ParsedValue<T>.Unparsable;
    //        //return parsed ? intValue : Unparsable.Default;
    //    }
    //    return ParsedValue<T>.Unparsable;
    //}
    public readonly string? GetString(string fieldName, Dictionary<string, int> columns, bool assumeWhiteSpaceIsEmpty = true) =>
        GetString(columns[fieldName], assumeWhiteSpaceIsEmpty);

    public readonly string? GetString(int column, bool assumeWhiteSpaceIsEmpty = true)
    {
        string sValue = Tokens[column];
        if (assumeWhiteSpaceIsEmpty && string.IsNullOrWhiteSpace(sValue) || string.IsNullOrEmpty(sValue))
            return null;

        return sValue;
    }

    public readonly ParsedValue<bool> GetBool(string fieldName, Dictionary<string, int> columns)
        => GetBool(columns[fieldName]);

    public readonly ParsedValue<bool> GetBool(int column)
    {
        string sValue = Tokens[column];
        if (sValue == "") return ParsedValue<bool>.Null;

        string sValueLower = sValue.ToLower();

        bool isTrue = sValueLower == "yes" || sValueLower == "true" || sValueLower == "1" || sValueLower == "-1" || sValueLower == "oui";
        bool isFalse = sValueLower == "no" || sValueLower == "false" || sValueLower == "0" || sValueLower == "non";

        if (isTrue) return new ParsedValue<bool>(true, sValue); //true;
        if (isFalse) return new ParsedValue<bool>(false, sValue);// false;
        return ParsedValue<bool>.Unparsable(sValue); //Unparsable.Default;
    }


    public readonly ParsedValue<double> GetDouble(string fieldName, Dictionary<string, int> columns, CultureInfo? info = null) =>
        GetDouble(columns[fieldName], info);

    public readonly ParsedValue<double> GetDouble(int column, CultureInfo? info = null)
    {
        info ??= _en;
        string sValue = Tokens[column];
        if (sValue == "") return ParsedValue<double>.Null;
        bool parsed = double.TryParse(sValue, info, out var doubleValue);
        return parsed ? new ParsedValue<double>(doubleValue, sValue) : ParsedValue<double>.Unparsable(sValue);
    }

    public readonly ParsedValue<float> GetFloat(string fieldName, Dictionary<string, int> columns, CultureInfo? info = null) =>
        GetFloat(columns[fieldName], info);


    public readonly ParsedValue<float> GetFloat(int column, CultureInfo? info = null)
    {
        info ??= _en;
        string sValue = Tokens[column];
        if (sValue == "") return ParsedValue<float>.Null;
        bool parsed = float.TryParse(sValue, info, out var floatValue);
        return parsed ? new ParsedValue<float>(floatValue, sValue) : ParsedValue<float>.Unparsable(sValue);
    }

    public readonly string? GetString(string fieldName, Dictionary<string, int> columns) =>
        GetString(columns[fieldName]);

    public readonly string? GetString(int column)
    {
        string sValue = Tokens[column];
        if (sValue == "") return null;
        return sValue;
    }

    public readonly ParsedValue<int> GetInt(string fieldName, Dictionary<string, int> columns, CultureInfo? info = null)
        => GetInt(columns[fieldName], info);

    public readonly ParsedValue<int> GetInt(int column, CultureInfo? info = null)
    {
        info ??= _en;
        string sValue = Tokens[column];
        if (sValue == "") return ParsedValue<int>.Null;
        bool parsed = int.TryParse(sValue, info, out int intValue);
        return parsed ? new ParsedValue<int>(intValue, sValue) : ParsedValue<int>.Unparsable(sValue);
    }

    public readonly ParsedValue<byte> GetByte(string fieldName, Dictionary<string, int> columns, CultureInfo? info = null) =>
        GetByte(columns[fieldName], info);

    public readonly ParsedValue<byte> GetByte(int column, CultureInfo? info = null)
    {
        info ??= _en;
        string sValue = Tokens[column];
        if (sValue == "") return ParsedValue<byte>.Null;
        bool parsed = byte.TryParse(sValue, info, out byte byteValue);
        return parsed ? new ParsedValue<byte>(byteValue, sValue) : ParsedValue<byte>.Unparsable(sValue);
    }

    public readonly ParsedValue<long> GetLong(string fieldName, Dictionary<string, int> columns, CultureInfo? info = null)
        => GetLong(columns[fieldName], info);

    public readonly ParsedValue<long> GetLong(int column, CultureInfo? info = null)
    {
        info ??= _en;
        string sValue = Tokens[column];
        if (sValue == "") return ParsedValue<long>.Null;
        bool parsed = long.TryParse(sValue, info, out long longValue);
        return parsed ? new ParsedValue<long>(longValue, sValue) : ParsedValue<long>.Unparsable(sValue);
    }

    public readonly ParsedValue<decimal> GetDecimal(string fieldName, Dictionary<string, int> columns, CultureInfo? info = null) =>
        GetDecimal(columns[fieldName], info);

    public readonly ParsedValue<decimal> GetDecimal(int column, CultureInfo? info = null)
    {
        info ??= _en;
        string sValue = Tokens[column];
        if (sValue == "") return ParsedValue<decimal>.Null;
        bool parsed = decimal.TryParse(sValue, info, out decimal decimalValue);
        return parsed ? new ParsedValue<decimal>(decimalValue, sValue) : ParsedValue<decimal>.Unparsable(sValue);
    }

    public readonly ParsedValue<DateTime> GetDateTime(string fieldName, Dictionary<string, int> columns, CultureInfo? info = null, string? format = null) =>
        GetDateTime(columns[fieldName], info, format);


    public readonly ParsedValue<DateTime> GetDateTime(int column, CultureInfo? info = null, string? format = null)
    {
        info ??= _en;
        string sValue = Tokens[column];
        if (sValue == "") return ParsedValue<DateTime>.Null;

        DateTime dateTimeValue;
        bool parsed =
            format is not null ?
            DateTime.TryParseExact(sValue, format, info, DateTimeStyles.None, out dateTimeValue) :
            DateTime.TryParse(sValue, info, out dateTimeValue);
        //return parsed ? dateTimeValue : Unparsable.Default;
        return parsed ? new ParsedValue<DateTime>(dateTimeValue, sValue) : ParsedValue<DateTime>.Unparsable(sValue);
    }

    public readonly ParsedValue<DateTimeOffset> GetDateTimeOffset(string fieldName, Dictionary<string, int> columns, CultureInfo? info = null, string? format = null) =>
        GetDateTimeOffset(columns[fieldName], info, format);


    public readonly ParsedValue<DateTimeOffset> GetDateTimeOffset(int column, CultureInfo? info = null, string? format = null)
    {
        info ??= _en;
        string sValue = Tokens[column];
        if (sValue == "") return ParsedValue<DateTimeOffset>.Null;

        DateTimeOffset dateTimeValue;
        bool parsed =
            format is not null ?
            DateTimeOffset.TryParseExact(sValue, format, info, DateTimeStyles.None, out dateTimeValue) :
            DateTimeOffset.TryParse(sValue, info, out dateTimeValue);
        //return parsed ? dateTimeValue : Unparsable.Default;
        return parsed ? new ParsedValue<DateTimeOffset>(dateTimeValue, sValue) : ParsedValue<DateTimeOffset>.Unparsable(sValue);
    }
    #endregion

    public override string ToString()
    {
        string line = "";
        if (FromLine is not null && ToLine is not null)
            line = FromLine != ToLine ? $"Lines: {FromLine}-{ToLine}, " : $"Line: {FromLine}, ";
        else if (FromLine is not null)
            line = $"Line: {FromLine}, ";

        string incomplete = IsIncomplete ? ", Incomplete" : "";
        return $"{line}Tokens: {Tokens.Count}{incomplete}";
    }
}

