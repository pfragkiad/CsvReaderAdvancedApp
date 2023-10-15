using CsvReaderAdvanced;
using CsvReaderAdvanced.Files;
using CsvReaderAdvanced.Schemas;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text;

namespace TestCsv;

internal class Program
{
    static void Main(string[] args)
    {
        //build the app
        var host = Host.CreateDefaultBuilder(args).ConfigureServices((c, s) => s.AddCsvReader(c.Configuration));
        var app = host.Build();

        //read the file
        string path = @"D:\repos\pfragkiad\CsvReaderAdvancedApp\CsvReaderAdvanced\samples\hard.csv";
        var factory = app.Services.GetCsvFileFactory();
        var file = factory.ReadWholeFile(path, Encoding.UTF8, withHeader: true) ;

        //file = app.Services.GetCsvFileFactory().GetFile(path,Encoding.UTF8, withHeader: true) ;

        //build a schema (without appsettings.json)
        CsvSchema schema = new()
        {
            Name = "test",
            Fields = new List<CsvField>()
            {
                new CsvField(){ Name = "FullName"},
                new CsvField(){ Name = "DoubleValue"},
                new CsvField(){ Name = "IntValue"}
            }
        };

        file.CheckAgainstSchema(schema);
        Dictionary<string, int> c = file.ExistingFieldColumns;

        //var c = file.ExistingColumns;

        foreach (TokenizedLine? l in file.Lines!)
        {
            if (!l.HasValue) return;
            List<string> t = l.Value.Tokens;

            string? v1 = l.Value.GetString("FullName", c);
            double? v2 = l.Value.GetDouble("DoubleValue", c);
            int? v3 = l.Value.GetInt("IntValue", c);
        }

    }
}