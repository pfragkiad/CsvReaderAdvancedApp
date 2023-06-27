using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CsvReaderAdvanced;

public readonly struct ReaderReport
{
    public int? Added { get; init; }

    public int? Updated { get; init; }

    public int? Valid { get => Added + Updated; }

    public int? Invalid { get; init; }

    public Dictionary<string, ReaderReport>? Subreports { get; init; }

    public ValidationResult Validation { get; init; }

    public static readonly ReaderReport DefaultValid =
        new ReaderReport() { Validation = new ValidationResult() };


    public IResult ToIResult()
    {
        if ((Valid ?? 0) > 0)
            return Results.Ok(this);
        else
            return Results.UnprocessableEntity(this);
    }
}
