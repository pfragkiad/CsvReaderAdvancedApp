using System.ComponentModel.DataAnnotations.Schema;

namespace CsvReaderAdvanced;

public class Limit
{
    //[Column("ID")]
    //public int Id { get; init; }

    public string TableName { get; init; } = default!;

    public string FieldName { get; init; } = default!;

    public double? Warning { get; init; }

    public double? Maximum { get; init; }

    /// <summary>
    /// Used only for strings. The length the of the fields should be lower than or equal to MaximumLength.
    /// </summary>
    public int? MaximumLength { get; init; }

    public double? Minimum { get; init; }

    public override string ToString()
    {
        string s = $"{TableName}/{FieldName} ";

        return s += MaximumLength.HasValue ? $"[{MaximumLength}]" : $"[{Minimum}|{Warning}|{Maximum}]";
    }
}
