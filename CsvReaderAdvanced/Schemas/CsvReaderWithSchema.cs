using CsvReaderAdvanced.Interfaces;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace CsvReaderAdvanced.Schemas;

public abstract class CsvReaderWithSchema : ICsvReaderWithSchema
{
    protected readonly ICsvFile _file;
    protected readonly ILogger _logger;
    protected readonly CsvSchemaOptions _options;

    public CsvReaderWithSchema(
        ICsvFile file,
        ILogger logger,
        IOptions<CsvSchemaOptions> options)
    {
        _file = file;
        _logger = logger;
        _options = options.Value;

        CsvSchema = GetSchema(SchemaName);
    }

    public abstract string SchemaName { get; }

    public CsvSchema? CsvSchema { get; init; }

    protected CsvSchema? GetSchema(string name) =>
        _options?.Schemas?.FirstOrDefault(s => s.Name == name);

    public ValidationResult CheckSchema(string schemaName)
    {
        if (_options?.Schemas is null || !_options.Schemas.Any())
        {
            _logger.LogError("Could not retrieve csv schemas from settings");
            return new ValidationResult(
                new ValidationFailure[] {
                    new ValidationFailure("CsvSchemas",
                    "Cannot retrieve csv schemas from settings") });
        }

        var schema = GetSchema(schemaName);

        if (schema is null)
        {
            _logger.LogError("Could not retrieve '{schemaName}' schema from settings", schemaName);
            return new ValidationResult(
                new ValidationFailure[] {
                    new ValidationFailure(schemaName,
                    $"Cannot retrieve '{schemaName}' schema from settings") });
        }
        return new ValidationResult();
    }

    protected static async Task<string> ReadUtf8FileContent(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }

    public async Task<ReaderReport> Import(HttpRequest req)
    {
        string content = await ReadUtf8FileContent(req);

        string tempFilePath = Path.GetTempFileName();
        File.WriteAllText(tempFilePath, content);

        var result = await Import(tempFilePath);

        File.Delete(tempFilePath);

        return result;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public virtual async Task<ReaderReport> Import(string filePath)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        if (CsvSchema is null)
        {
            var schemaValidation = CheckSchema(SchemaName);

            //without schema information the processing cannot continue
            if (!schemaValidation.IsValid)
                return new ReaderReport
                {
                    Validation = schemaValidation
                };
        }

        _file.ReadFromFile(filePath, Encoding.UTF8, withHeader: true);

        //schema is needed for reporting missing columns
        _file.CheckAgainstSchema(CsvSchema!);

        //if any of the required fields are missing then the importing cannot continue
        if (_file.MissingRequiredFields.Any())
            return new ReaderReport
            {
                Validation = new ValidationResult(
                    _file.MissingRequiredFields.Select(m =>
                    {

                        string candidateNames = string.Join(", ", m.GetCandidateNames(true).Select(n => $"'{n}'"));
                        string message = $"{m.Name} is missing from the csv file. Consider using one of the following headers: {candidateNames}";
                        ValidationFailure failure = new ValidationFailure()
                        {
                            ErrorMessage = message,
                            PropertyName = "CsvFile"
                        };
                        return failure;
                    }))
            };

        return ReaderReport.DefaultValid;
    }

    #region Check functions

    protected static ParsedValue<int> CheckIntId<T>(
        string fieldName,
        Dictionary<string, int> c,
        TokenizedLine line,
        List<ValidationFailure> lineFailures,
        Dictionary<int, T> idCollection,
        bool allowNull)
    {
        if (!c.ContainsKey(fieldName))
        {
            if (!allowNull)
                lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} field is missing. Line: {line.FromLine}." });

            return ParsedValue<int>.Null;
        }


        ParsedValue<int> valueToken = line.GetInt(fieldName, c);
        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });

        //we allow null if we arrive here
        if (!valueToken.IsNull && !idCollection.ContainsKey(valueToken))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} '{valueToken.Value}' was not found. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }

    protected static ParsedValue<int> CheckIntId<T>(
        string fieldName, string sValue, int recordNumber,
        List<ValidationFailure> lineFailures,
        Dictionary<int, T> idCollection,
        bool allowNull)
    {
        ParsedValue<int> valueToken = GetInt(sValue);
        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });

        //we allow null if we arrive here
        if (!valueToken.IsNull && !idCollection.ContainsKey(valueToken))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} '{valueToken.Value}' was not found. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }

    protected static void CheckIntId<T>(
     string fieldName, int? value, int recordNumber,
     List<ValidationFailure> lineFailures,
     Dictionary<int, T> idCollection,
     bool allowNull)
    {
        if (value is null && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = value });

        //we allow null if we arrive here
        if (value.HasValue && !idCollection.ContainsKey(value.Value))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} '{value}' was not found. Record: {recordNumber}.", AttemptedValue = value });
    }

    public static ParsedValue<int> GetInt(string sValue, CultureInfo? info = null)
    {
        info ??= _en;
        if (sValue == "") return ParsedValue<int>.Null;
        bool parsed = int.TryParse(sValue, info, out int intValue);
        return parsed ? new ParsedValue<int>(intValue, sValue) : ParsedValue<int>.Unparsable(sValue);
    }

    protected static ParsedValue<bool> CheckBool(
        string fieldName,
        Dictionary<string, int> c,
        TokenizedLine line,
        List<ValidationFailure> lineFailures,
        bool allowNull)
    {
        if (!c.ContainsKey(fieldName)) return ParsedValue<bool>.Null;

        var valueToken = line.GetBool(fieldName, c);

        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }

    protected static ParsedValue<bool> CheckBool(
        string fieldName, string sValue, int recordNumber,
        List<ValidationFailure> lineFailures,
        bool allowNull)
    {
        var valueToken = GetBool(sValue);

        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }

    protected static void CheckBool(
     string fieldName, bool? value, int recordNumber,
     List<ValidationFailure> lineFailures,
     bool allowNull)
    {
        if (value is null && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = value });
    }

    public static ParsedValue<bool> GetBool(string sValue)
    {
        if (sValue == "") return ParsedValue<bool>.Null;

        string sValueLower = sValue.ToLower();

        bool isTrue = sValueLower == "yes" || sValueLower == "true" || sValueLower == "1" || sValueLower == "-1" || sValueLower == "oui";
        bool isFalse = sValueLower == "no" || sValueLower == "false" || sValueLower == "0" || sValueLower == "non";

        if (isTrue) return new ParsedValue<bool>(true, sValue); //true;
        if (isFalse) return new ParsedValue<bool>(false, sValue);// false;
        return ParsedValue<bool>.Unparsable(sValue); //Unparsable.Default;
    }

    protected static ParsedValue<DateTimeOffset> CheckDateTimeOffset(
        string fieldName,
        Dictionary<string, int> c,
        TokenizedLine line,
        List<ValidationFailure> lineFailures,
        bool allowNull)
    {
        if (!c.ContainsKey(fieldName)) return ParsedValue<DateTimeOffset>.Null;

        var valueToken = line.GetDateTimeOffset(fieldName, c);

        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }
    protected static ParsedValue<DateTimeOffset> CheckDateTimeOffset(
         string fieldName, string sValue, int recordNumber,
         List<ValidationFailure> lineFailures,
         bool allowNull)
    {
        var valueToken = GetDateTimeOffset(sValue);

        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }
    protected static void CheckDateTimeOffset(
           string fieldName, DateTimeOffset? value, int recordNumber,
           List<ValidationFailure> lineFailures,
           bool allowNull)
    {

        if (value is null && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = value });
    }
    public static ParsedValue<DateTimeOffset> GetDateTimeOffset(string sValue, CultureInfo? info = null, string? format = null)
    {
        info ??= _en;
        if (sValue == "") return ParsedValue<DateTimeOffset>.Null;

        DateTimeOffset dateTimeValue;
        bool parsed =
            format is not null ?
            DateTimeOffset.TryParseExact(sValue, format, info, DateTimeStyles.None, out dateTimeValue) :
            DateTimeOffset.TryParse(sValue, info, out dateTimeValue);
        //return parsed ? dateTimeValue : Unparsable.Default;
        return parsed ? new ParsedValue<DateTimeOffset>(dateTimeValue, sValue) : ParsedValue<DateTimeOffset>.Unparsable(sValue);
    }

    protected static string? CheckString(
        string fieldName,
        Dictionary<string, int> c,
        TokenizedLine line,
        List<ValidationFailure> lineFailures,
        bool allowNull,
        Dictionary<string, Limit>? limitsByName = null,
        string? limitsFieldName = null)
    {
        if (!c.ContainsKey(fieldName))
        {
            if (!allowNull)
                lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} field is missing. Line: {line.FromLine}." });

            return null;
        }

        limitsFieldName ??= fieldName;

        string value = line.Tokens[c[fieldName]];
        int? maximumLength = limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].MaximumLength : null
            : null;

        if (string.IsNullOrWhiteSpace(value) && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Line: {line.FromLine}.", AttemptedValue = value });
        else if (maximumLength.HasValue && value.Length > maximumLength)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has {value.Length} characters (maximum allowed: {maximumLength}). Line: {line.FromLine}.", AttemptedValue = value });

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    protected static string? CheckString(
    string fieldName, string? value, int recordNumber,
    List<ValidationFailure> lineFailures,
    bool allowNull,
    Dictionary<string, Limit>? limitsByName = null,
    string? limitsFieldName = null)
    {
        limitsFieldName ??= fieldName;

        int? maximumLength = limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].MaximumLength : null
            : null;

        if (string.IsNullOrWhiteSpace(value) && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = value });
        else if (maximumLength.HasValue && value is not null && value!.Length > maximumLength)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has {value.Length} characters (maximum allowed: {maximumLength}). Record: {recordNumber}.", AttemptedValue = value });

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    protected static ParsedValue<double> CheckDouble(
        string fieldName,
        Dictionary<string, int> c,
        TokenizedLine line,
        List<ValidationFailure> lineFailures,
        bool allowNull,
        Dictionary<string, Limit>? limitsByName = null,
        string? limitsFieldName = null)
    {
        if (!c.ContainsKey(fieldName))
        {
            if (!allowNull)
                lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} field is missing. Line: {line.FromLine}." });

            return ParsedValue<double>.Null;

        }

        limitsFieldName ??= fieldName;
        double? minimumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Minimum : null
            : null;
        double? maximumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Maximum : null
            : null;
        //var valueToken = line.GetDouble(fieldName, c);
        ParsedValue<double> valueToken = line.GetDouble(fieldName, c);
        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (
            minimumLimit.HasValue && maximumLimit.HasValue &&
            (valueToken.Value < minimumLimit || valueToken.Value > maximumLimit))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is out of range (range allowed: {minimumLimit} to {maximumLimit}). Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (
            minimumLimit.HasValue && valueToken.Value < minimumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is lower than {minimumLimit}. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (
            maximumLimit.HasValue && valueToken.Value > maximumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is greater than {maximumLimit}). Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }


    protected static ParsedValue<int> CheckInt(
    string fieldName,
    Dictionary<string, int> c,
    TokenizedLine line,
    List<ValidationFailure> lineFailures,
    bool allowNull,
    Dictionary<string, Limit>? limitsByName = null,
    string? limitsFieldName = null)
    {
        if (!c.ContainsKey(fieldName))
        {
            if (!allowNull)
                lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} field is missing. Line: {line.FromLine}." });

            return ParsedValue<int>.Null;

        }

        limitsFieldName ??= fieldName;
        double? minimumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Minimum : null
            : null;
        double? maximumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Maximum : null
            : null;
        //var valueToken = line.GetDouble(fieldName, c);
        ParsedValue<int> valueToken = line.GetInt(fieldName, c);
        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (
            minimumLimit.HasValue && maximumLimit.HasValue &&
            (valueToken.Value < minimumLimit || valueToken.Value > maximumLimit))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is out of range (range allowed: {minimumLimit} to {maximumLimit}). Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (
            minimumLimit.HasValue && valueToken.Value < minimumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is lower than {minimumLimit}. Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });
        else if (
            maximumLimit.HasValue && valueToken.Value > maximumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is greater than {maximumLimit}). Line: {line.FromLine}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }


    protected static ParsedValue<double> CheckDouble(
    string fieldName, string sValue, int recordNumber,
    List<ValidationFailure> lineFailures,
    bool allowNull,
    Dictionary<string, Limit>? limitsByName = null,
    string? limitsFieldName = null)
    {

        limitsFieldName ??= fieldName;
        double? minimumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Minimum : null
            : null;
        double? maximumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Maximum : null
            : null;
        //var valueToken = line.GetDouble(fieldName, c);
        ParsedValue<double> valueToken = GetDouble(sValue);
        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (
            minimumLimit.HasValue && maximumLimit.HasValue &&
            (valueToken.Value < minimumLimit || valueToken.Value > maximumLimit))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is out of range (range allowed: {minimumLimit} to {maximumLimit}). Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (
            minimumLimit.HasValue && valueToken.Value < minimumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is lower than {minimumLimit}. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (
            maximumLimit.HasValue && valueToken.Value > maximumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is greater than {maximumLimit}). Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }

    protected static void CheckDouble(
    string fieldName, double? value, int recordNumber,
    List<ValidationFailure> lineFailures,
    bool allowNull,
    Dictionary<string, Limit>? limitsByName = null,
    string? limitsFieldName = null)
    {

        limitsFieldName ??= fieldName;
        double? minimumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Minimum : null
            : null;
        double? maximumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Maximum : null
            : null;
        //var valueToken = line.GetDouble(fieldName, c);
        if (value is null && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = value });
        else if (
            minimumLimit.HasValue && maximumLimit.HasValue &&
            (value < minimumLimit || value > maximumLimit))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is out of range (range allowed: {minimumLimit} to {maximumLimit}). Record: {recordNumber}.", AttemptedValue = value });
        else if (
            minimumLimit.HasValue && value < minimumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is lower than {minimumLimit}. Record: {recordNumber}.", AttemptedValue = value });
        else if (
            maximumLimit.HasValue && value > maximumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is greater than {maximumLimit}). Record: {recordNumber}.", AttemptedValue = value });

    }


    protected static ParsedValue<int> CheckInt(
    string fieldName, string sValue, int recordNumber,
    List<ValidationFailure> lineFailures,
    bool allowNull,
    Dictionary<string, Limit>? limitsByName = null,
    string? limitsFieldName = null)
    {
        limitsFieldName ??= fieldName;
        double? minimumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Minimum : null
            : null;
        double? maximumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Maximum : null
            : null;
        //var valueToken = line.GetDouble(fieldName, c);
        ParsedValue<int> valueToken = GetInt(sValue);
        if (!valueToken.IsParsed)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} has bad format. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (valueToken.IsNull && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (
            minimumLimit.HasValue && maximumLimit.HasValue &&
            (valueToken.Value < minimumLimit || valueToken.Value > maximumLimit))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is out of range (range allowed: {minimumLimit} to {maximumLimit}). Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (
            minimumLimit.HasValue && valueToken.Value < minimumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is lower than {minimumLimit}. Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });
        else if (
            maximumLimit.HasValue && valueToken.Value > maximumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is greater than {maximumLimit}). Record: {recordNumber}.", AttemptedValue = valueToken.StringValue });

        return valueToken;
    }


    protected static void CheckInt(
        string fieldName, int? value, int recordNumber,
        List<ValidationFailure> lineFailures,
        bool allowNull,
        Dictionary<string, Limit>? limitsByName = null,
        string? limitsFieldName = null)
    {

        limitsFieldName ??= fieldName;
        double? minimumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Minimum : null
            : null;
        double? maximumLimit =
            limitsByName is not null ?
            limitsByName.ContainsKey(limitsFieldName) ? limitsByName[limitsFieldName].Maximum : null
            : null;
        //var valueToken = line.GetDouble(fieldName, c);
        if (value is null && !allowNull)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is empty. Record: {recordNumber}.", AttemptedValue = value });
        else if (
            minimumLimit.HasValue && maximumLimit.HasValue &&
            (value < minimumLimit || value > maximumLimit))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is out of range (range allowed: {minimumLimit} to {maximumLimit}). Record: {recordNumber}.", AttemptedValue = value });
        else if (
            minimumLimit.HasValue && value < minimumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is lower than {minimumLimit}. Record: {recordNumber}.", AttemptedValue = value });
        else if (
            maximumLimit.HasValue && value > maximumLimit)
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} is greater than {maximumLimit}). Record: {recordNumber}.", AttemptedValue = value });

    }


    static readonly CultureInfo _en = CultureInfo.InvariantCulture;

    public static ParsedValue<double> GetDouble(string sValue, CultureInfo? info = null)
    {
        info ??= _en;
        if (sValue == "") return ParsedValue<double>.Null;
        bool parsed = double.TryParse(sValue, info, out var doubleValue);
        return parsed ? new ParsedValue<double>(doubleValue, sValue) : ParsedValue<double>.Unparsable(sValue);
    }


    protected static string? CheckStringWithId<T>(
        string fieldName,
        Dictionary<string, int> c,
        TokenizedLine line,
        List<ValidationFailure> lineFailures,
        Dictionary<string, T> collection,
        bool allowNull,
        Dictionary<string, Limit>? limitsByName = null,
        string? limitsFieldName = null)
    {
        string? value = CheckString(fieldName, c, line, lineFailures, allowNull, limitsByName, limitsFieldName);
        if (value is null) return null;

        //if we arrive here the value is not null and an error can still occur if the value is not contained in the dictionary
        if (!collection.ContainsKey(value))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} '{value}' was not found. Line: {line.FromLine}.", AttemptedValue = value });

        return value;
    }

    protected static string? CheckStringWithId<T>(
     string fieldName, string? sValue, int recordNumber,
     List<ValidationFailure> lineFailures,
     Dictionary<string, T> collection,
     bool allowNull,
     Dictionary<string, Limit>? limitsByName = null,
     string? limitsFieldName = null)
    {
        string? value = CheckString(fieldName, sValue, recordNumber, lineFailures, allowNull, limitsByName, limitsFieldName);
        if (value is null) return null;

        //if we arrive here the value is not null and an error can still occur if the value is not contained in the dictionary
        if (!collection.ContainsKey(value))
            lineFailures.Add(new ValidationFailure() { PropertyName = fieldName, ErrorMessage = $"{fieldName} '{value}' was not found. Record: {recordNumber}.", AttemptedValue = value });

        return value;
    }

    protected static int? CheckStringOrIntId<T>(
        string stringFieldName,
        string intFieldName,
        Dictionary<string, int> c,
        TokenizedLine line,
        List<ValidationFailure> lineFailures,
        Dictionary<string, T> nameCollection,
        Dictionary<int, T> idCollection,
        bool allowNull)
    {
        if (!c.ContainsKey(stringFieldName) && !c.ContainsKey(intFieldName))
        {
            if (!allowNull)
                lineFailures.Add(new ValidationFailure() { PropertyName = stringFieldName, ErrorMessage = $"{stringFieldName} field is missing. Line: {line.FromLine}." });

            return null;
        }

        string? name = null;
        ParsedValue<int> idToken = ParsedValue<int>.Null;
        if (c.ContainsKey(intFieldName)) idToken = CheckIntId(intFieldName, c, line, lineFailures, idCollection, allowNull: true);

        //a failure will be added if the passed name is non-empty and is not included in the collection
        if (c.ContainsKey(stringFieldName)) name = CheckStringWithId(stringFieldName, c, line, lineFailures, nameCollection, allowNull: true);
        if (name is not null && !nameCollection.ContainsKey(name)) return null;

        if (name is null && idToken.IsNull)
        {
            if (!allowNull)
                lineFailures.Add(new ValidationFailure() { PropertyName = stringFieldName, ErrorMessage = $"The {stringFieldName} must not be empty. Line: {line.FromLine}.", AttemptedValue = name });

            return null;
        }

        //one of two is not null, so we return the value
        return !idToken.IsNull ? idToken.Value : (nameCollection[name!] as dynamic)!.Id;
    }


    protected static int? GetIdByName<T>(
      string fieldName,
      Dictionary<string, int> c,
      TokenizedLine l,
      List<ValidationFailure> lineFailures,
      Dictionary<string, T> collectionByName,
      bool allowNull)
    {
        if (!c.ContainsKey(fieldName)) return null;
        string? name = CheckStringWithId(fieldName, c, l, lineFailures, collectionByName, allowNull);
        if (name is null) return null;
        return collectionByName.ContainsKey(name!) ? (collectionByName[name!] as dynamic)!.Id : null;
    }

    protected static int? GetIdByName<T>(
     string fieldName, string? value, int recordNumber,
     List<ValidationFailure> lineFailures,
     Dictionary<string, T> collectionByName,
     bool allowNull)
    {
        string? name = CheckStringWithId(fieldName, value, recordNumber, lineFailures, collectionByName, allowNull);
        if (name is null) return null;
        return collectionByName.ContainsKey(name!) ? (collectionByName[name!] as dynamic)!.Id : null;
    }

    #endregion

}
