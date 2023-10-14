using CsvReaderAdvanced;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlServerExplorerLib;
using System.Globalization;

namespace CsvWinAnalyzer;

internal static class Program
{
    public static IHost? Provider { get; private set; }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Provider = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services
                .AddSingleton<frmMain>()
                .AddSingleton<frmDatabase>()
                .AddCsvReader(context.Configuration)
                .AddSingleton<SqlServerExplorer>()

                ;
            })
            .Build();


        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        Application.Run(Provider.Services.GetRequiredService<frmMain>());
    }


}