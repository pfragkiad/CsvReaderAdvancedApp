using CsvReaderAdvanced.Schemas;

namespace CsvReaderAdvanced;

public class CsvSchemaOptions
{
    public const string CsvSchemasSection = "csvSchemas";
    public List<CsvSchema>? Schemas { get; set; }
}
