

using CsvReaderAdvanced.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CsvReaderAdvanced;

public static class DependencyInjection
{
    public static IServiceCollection AddCsvReader(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICsvReader,CsvReader>();
        services.AddTransient<ICsvFile,CsvFile>();

        //Microsoft.Extensions.Hosting must be referenced
        services.Configure<CsvSchemaOptions>(configuration.GetSection(CsvSchemaOptions.CsvSchemasSection));
        return services;

    }

    public static ICsvReader GetCsvReader(this IServiceProvider provider) =>
        provider.GetRequiredService<ICsvReader>();

    public static ICsvFile GetCsvFile(this IServiceProvider provider) =>
        provider.GetRequiredService<ICsvFile>();

    public static CsvSchemaOptions GetSchemaOptions(this IServiceProvider provider) =>
        provider.GetRequiredService<IOptionsMonitor<CsvSchemaOptions>>().CurrentValue;
}

