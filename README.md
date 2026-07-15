# CsvReaderAdvanced

A CSV parsing library for .NET with DI-friendly setup, schema-aware header mapping, lazy/full reading modes, and typed value parsing.

## Install

Package Manager:

```powershell
Install-Package CsvReaderAdvanced
```

.NET CLI:

```bash
dotnet add package CsvReaderAdvanced
```

## Quick start (DI)

```csharp
using CsvReaderAdvanced;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddCsvReader(context.Configuration);
    })
    .Build();

var fileFactory = host.Services.GetCsvFileFactory();
```

`AddCsvReader` registers:

- `CsvReader`
- `CsvFileFactory`
- `CsvSchemaOptions` from the `csvSchemas` config section

## Configure schemas in `appsettings.json`

```json
{
  "csvSchemas": {
    "schemas": [
      {
        "name": "products",
        "fields": [
          {
            "name": "ProductID",
            "alternatives": ["Product ID"],
            "required": true
          },
          {
            "name": "Weight",
            "unit": "t",
            "alternativeFields": ["Volume", "TEU"],
            "required": true
          },
          {
            "name": "Volume",
            "unit": "m^3",
            "alternativeUnits": ["m3", "m^3"]
          }
        ]
      }
    ]
  }
}
```

## Read a file

Use `CsvFileFactory` to create a `CsvFile`:

```csharp
var file = fileFactory.ReadWholeFile(path, Encoding.UTF8, withHeader: true);
// or:
var lazyFile = fileFactory.GetFile(path, Encoding.UTF8, withHeader: true);
```

- `ReadWholeFile` loads all rows into `file.Lines`
- `GetFile` reads header metadata and lets you stream rows with `Read(...)`

When `withHeader: true`, `ReadHeader()` populates:

- `Header`
- `ExistingColumns` (`Dictionary<string,int>`, case-insensitive)

## Validate header against a schema

```csharp
var options = host.Services.GetSchemaOptions();
var schema = options.Schemas?.FirstOrDefault(s => s.Name == "products");

if (schema is null)
    throw new InvalidOperationException("Schema 'products' was not found.");

var file = fileFactory.GetFile(path, Encoding.UTF8, withHeader: true);
file.CheckAgainstSchema(schema);

if (file.MissingRequiredFields.Any())
{
    foreach (var missing in file.MissingRequiredFields)
        Console.WriteLine($"Missing required field: {missing.Name}");
}

// schema field name -> column index
var columns = file.ExistingFieldColumns;
```

`CheckAgainstSchema` updates:

- `ExistingFieldColumns`
- `MissingFields`
- `MissingRequiredFields`

## Parsed values and line information

Each parsed row is a `TokenizedLine` with:

- `Tokens`
- `FromLine` / `ToLine` (helpful when quoted values span multiple physical lines)

Typed getters such as `GetDouble`, `GetInt`, `GetDateTime`, `GetBoolean`, etc. return `ParsedValue<T>` with:

- `Value`
- `State` (`Parsed`, `Null`, `Unparsable`, `NaN`)
- `IsParsed`, `IsNull`, `IsNaN`
- `StringValue`

```csharp
var file = fileFactory.ReadWholeFile(path, Encoding.UTF8, withHeader: true);
var c = file.ExistingColumns;

foreach (var line in file.Lines!.Where(l => l.HasValue).Select(l => l!.Value))
{
    var productName = line.GetString("ProductName", c);
    var weight = line.GetDouble("Weight", c);

    if (!weight.IsParsed)
    {
        Console.WriteLine($"Invalid weight '{weight.StringValue}' at line {line.FromLine}");
        continue;
    }

    double value = weight;   // implicit conversion
    double? nullable = weight;
}
```

## Example: lazy-read for low memory usage

```csharp
var file = fileFactory.GetFile(path, Encoding.UTF8, withHeader: true);
var c = file.ExistingColumns;

foreach (var line in file.Read(skipHeader: true))
{
    if (!line.HasValue) continue;

    var t = line.Value;
    var id = t.GetInt("ProductID", c);
    var qty = t.GetLong("Quantity", c);

    if (id.IsParsed && qty.IsParsed)
    {
        // process row
    }
}
```

## Example: detect types and compute stats

```csharp
var file = fileFactory.GetFile(path, Encoding.UTF8, withHeader: true);

// Infer base types from data
file.UpdateFieldBaseTypes(maxRows: 5000);

// Then compute stats for inferred types
file.UpdateFieldStats(maxRows: 5000);

foreach (var f in file.ExistingFieldTypeInfos)
{
    Console.WriteLine($"Column {f.Column}: {f.BaseType}, Nulls={f.NullValuesCount}, Unparsed={f.UnparsedValuesCount}, Min={f.Minimum}, Max={f.Maximum}");
}
```

## Example: export selected columns

```csharp
var file = fileFactory.GetFile(path, Encoding.UTF8, withHeader: true);

await file.SavePartialAs(
    targetPath: @".\\samples\\export.csv",
    targetSeparator: ';',
    columnNames: new[] { "ProductID", "ProductName", "Weight" });
```

You can also export by index:

```csharp
await file.SavePartialAs(@".\\samples\\export.csv", ';', 0, 2, 5);
```

## Notes

- Separator detection supports `;`, `,`, and tab.
- `ReadFast(...)` / fast tokenization APIs are available for simpler inputs where quoted multiline handling is not needed.
