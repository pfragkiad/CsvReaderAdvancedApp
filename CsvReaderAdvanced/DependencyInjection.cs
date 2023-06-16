

using CsvReaderAdvanced.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Configuration;

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
}

