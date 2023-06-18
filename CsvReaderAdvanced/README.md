# CsvReaderAdvanced

*The faster and most modern CSV reader adapted to DI principles.*

Combine the power of the configuration JSON files with customized CSV reading. 

## How to install

Via tha Package Manager:
```powershell
Install-Package CsvReaderAdvanced -Version 1.0.23
```

Via the .NET CLI
```bat
dotnet add package CsvReaderAdvanced --version 1.0.23
```

## How to use

First add the service to the ServiceCollection.
```cs
 builder.ConfigureServices((context, services) =>
        {
            services.AddCsvReader(context.Configuration);
        ...
```


## Csv schemas via appsettings.json

To understand exactly what the method does, it assumes that the current configuration file contains a `csvSchemas` section, typically in the `appsettings.json` file:

```cs
public static IServiceCollection AddCsvReader(this IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton<ICsvReader,CsvReader>();
    services.AddTransient<ICsvFile,CsvFile>();

    //Microsoft.Extensions.Hosting must be referenced
    services.Configure<CsvSchemaOptions>(configuration.GetSection(CsvSchemaOptions.CsvSchemasSection));
    return services;
}
```

The schema in the appsettings.json file typically contains a property named `csvSchemas`:

```json
"csvSchemas": {
    "schemas": [
      {
        "name": "products",
        "fields": [
          {
            "name": "ProductID",
            "alternatives": [ "Product ID" ],
            "required": true
          },
          {
            "name": "Weight",
            "unit": "t",
            "alternativeFields": [ "Volume", "TEU" ],
            "required": true
          },
          {
            "name": "Volume",
            "unit": "m^3",
            "alternativeUnits": [ "m3", "m^3" ]
...
```

We assume that we get the options via DI like the following example:

```cs
public Importer(
    IUnitOfWork context,
    IMapper mapper,
    IServiceProvider provider,
    ILogger logger,
    IOptions<CsvSchemaOptions> options)
{
    _context = context;
    _mapper = mapper;
    _provider = provider;
    _logger = logger;
    _options = options.Value;
}

protected readonly IUnitOfWork _context;
protected readonly IMapper _mapper;
protected readonly IServiceProvider _provider;
protected readonly ILogger _logger;
protected readonly CsvSchemaOptions _options;

public CsvSchema? GetSchema(string name) =>
    _options?.Schemas?.FirstOrDefault(s => s.Name == name);

public ValidationResult CheckForSchema(string name)
{
    if (_options?.Schemas is null || !_options.Schemas.Any())
    {
        _logger.LogError("Could not retrieve csv schemas from settings");
        return new ValidationResult(
            new ValidationFailure[] { new ValidationFailure("CsvSchemas", "Cannot retrieve csv schemas from settings") });
    }

    var schema = GetSchema(name);

    if (schema is null)
    {
        _logger.LogError("Could not retrieve '{schemaName}' schema from settings",name);
        return new ValidationResult(
            new ValidationFailure[] { new ValidationFailure(name, $"Cannot retrieve '{name}' schema from settings") });
    }
    return new ValidationResult();

}
```

## Read the file

We instantiate a `CsvFile` in order to read the file. Note that the aforementioned `CsvSchema` is not needed if we do not have a header and/or do not want to validate the existence of fields.
For the example below, we assume that a `CsvSchema` is checked.

```cs
//We assume that _provider is an IServiceProvider which is injected via DI
var file = _provider.GetCsvFile();
file.ReadFromFile(path, Encoding.UTF8, withHeader:true);

//the line above is equivalent to the 2 commands:
file.ReadFromFile(path, Encoding.UTF8);
file.PopulateColumns();
```

The `PopulateColumns()` method updates the internal `ExistingColumns` dictionary. The `ExistingColumns` dictionary is case insensitive and stores the index location for each column. The index location is zero-based.
To check the existence of fields against a schema we should call the `CheckAgainstSchema()` method as shown below:

```cs
CsvScema schema = _options.Schemas.FirstOrDefault(s => s.Name == "products");
file.CheckAgainstSchema(schema);
```

The `CheckAgainstSchema()` method also calls the `PopulateColumns()` method if the `ExistingColumns` property is not populated. It then updates the `ExistingFieldColumns` dictionary, which is a dictionary of the column index location based on the field name.
Additional properties (Hashsets) are populated: `MissingFields`, `MissingRequiredFields`.

## Lines and ParsedValue

The most important updated property after the `ReadFromFile()` call is the `Lines` property, whic is a List of `TokenizedLine?` objects.
The `TokenizedLine` struct contains the `Tokens` property which is a List of `string` objects. The power of this library is that each `TokenizedLine` may potentially span more than 1 lines. This can occur in the case of quoted strings which may span to the next line. In general all cases where quoted strings are met, are cases where a simple `string.Split()` cannot work.
That's why the properties `FromLine` to `ToLine` exist. The latter are important for debugging purposes.
The `GetDouble`/`GetFloat`/`GetString`/`GetInt`/`GetByte`/`GetLong`/`GetDateTime`/`GetDateTimeOffset` methods return a `ParsedValue<T>` struct. The `ParsedValue` is a useful wrapper the contains a `Value`, a `IsParsed` and a `IsNull` property.

```cs
var c = file.ExistingFieldColumns;

//we can use the following instead, in case we want to use the original field names within the header the CSV file
//var c = file.ExistingColumns;

foreach (var line in file.Lines)
{
    TokenizedLine l = line.Value;
    
    //for strings we can immediately retrieve the token based on the field name
    string name = l.Tokens[c["ProductName"]];

    var weightValue = l.GetDouble("Weight", c);
    if (!weightValue.Parsed)
        _logger.LogError("Cannot parse Weight {value} at line {line}.", weightValue.Value, l.FromLine);
    else
    {
        double weight = weightValue.Value;
    ...
...
```

> IS THAT ALL? Of course not. More examples are pending. The library is more powerful than it seems!
