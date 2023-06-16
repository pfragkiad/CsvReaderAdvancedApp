namespace CsvReaderAdvanced;

public class CsvSchema
{
    public string Name { get; set; } = default!;
    public List<CsvField>? Fields { get; set; }

    public override string ToString() => Name;
}
