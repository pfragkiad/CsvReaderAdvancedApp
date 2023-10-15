


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using CsvReaderAdvanced.Schemas;
using CsvReaderAdvanced.Files;

namespace CsvReaderAdvanced;

public static class DependencyInjection
{
    public static IServiceCollection AddCsvReader(this IServiceCollection services, IConfiguration configuration)
    {
        services
        .AddScoped<CsvReader>()
        .AddScoped<CsvFileFactory>()
        ;

        //Microsoft.Extensions.Hosting must be referenced
        services.Configure<CsvSchemaOptions>(configuration.GetSection(CsvSchemaOptions.CsvSchemasSection));
        return services;

    }

    public static CsvReader GetCsvReader(this IServiceProvider provider) =>
        provider.GetRequiredService<CsvReader>();

    public static CsvFileFactory GetCsvFileFactory(this IServiceProvider provider) =>
        provider.GetRequiredService<CsvFileFactory>();

    public static CsvSchemaOptions GetSchemaOptions(this IServiceProvider provider) =>
        provider.GetRequiredService<IOptionsMonitor<CsvSchemaOptions>>().CurrentValue;
}

