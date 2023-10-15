using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CsvReaderAdvanced.Files;

public readonly struct ReaderReport
{
    public int? Added { get; init; }

    public int? Updated { get; init; }

    public int? Valid
    {
        get =>
            Added is null && Updated is null ? null : (Added ?? 0) + (Updated ?? 0);
    }

    public int? Invalid { get; init; }

    public Dictionary<string, ReaderReport>? Subreports { get; init; }

    public ValidationResult Validation { get; init; }

    public static readonly ReaderReport DefaultValid =
        new ReaderReport() { Validation = new ValidationResult() };

    public static ReaderReport CreateSingle(string propertyName, string message) =>
        new ReaderReport() { Validation = new(new ValidationFailure[] { new ValidationFailure(propertyName, message) }) };

    public IResult ToIResult()
    {
        //if there is at least one valid entry we consider the result as successful

        int totalValid = Subreports?.Sum(r => r.Value.Valid ?? 0) ?? 0;
        totalValid += Valid ?? 0;

        if (totalValid > 0)
            return Results.Ok(this);
        else
            return Results.UnprocessableEntity(this);
    }
}
