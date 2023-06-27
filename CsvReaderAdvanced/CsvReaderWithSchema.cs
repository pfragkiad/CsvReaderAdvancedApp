using CsvReaderAdvanced.Interfaces;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvReaderAdvanced;

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

                        string candidateNames = string.Join(", ",m.GetCandidateNames(true).Select(n=>$"'{n}'"));
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
        if (c.ContainsKey(stringFieldName)) name = CheckStringWithId(stringFieldName, c, line, lineFailures, nameCollection, allowNull: true);
        if ((name is null || name is not null && !nameCollection.ContainsKey(name)) && idToken.IsNull)
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

    #endregion

}
