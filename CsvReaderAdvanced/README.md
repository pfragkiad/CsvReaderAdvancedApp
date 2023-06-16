# CsvReaderAdvanced

The faster and most modern CSV reader adapted to DI principles.

## How to use

First add the service to the ServiceCollection.
```cs
 builder.ConfigureServices((context, services) =>
        {
            services.AddCsvReader(context.Configuration);
        ...
```

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
        "name": "shipments",
        "fields": [
          {
            "name": "ClientShipmentID",
            "alternatives": [ "Client Shipment ID", "Client shipmentid" ],
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
            "alternativeUnits": [ "m3", "m^3", "m³" ]
...
```

We assume that we get the options via DI like the following example:

```cs
public MyImporter(
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
```

