using FluentValidation.Results;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace CsvReaderAdvanced;

public readonly struct ReaderReport
{
    public int Added { get; init; }

    public int Updated { get; init; }

    public int Valid { get => Added + Updated; }

    public int Invalid { get; init; }

    public ValidationResult Validation {get;init;}

    public static readonly ReaderReport DefaultValid =
        new ReaderReport() { Validation = new ValidationResult()};
}
