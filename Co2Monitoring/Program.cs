using Microsoft.Extensions.Hosting;

using CsvReaderAdvanced;

using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using CsvReaderAdvanced.Files;

namespace Co2Monitoring;


internal class Program
{
    static void Main(string[] args)
    {
        var host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            services.AddCsvReader(context.Configuration)).Build();

        var fileFactory = host.Services.GetCsvFileFactory();

        //string path = @"D:\OneDrive - EMISIA SA\EEA\EEA DB CSV FILES\cars\2021 final cars\data.csv";
        //string targetTable = "cars_2021";

        //string path = @"D:\OneDrive - EMISIA SA\EEA\EEA DB CSV FILES\cars\2022 provisional\data.csv";
        //string targetTable = "cars_2022_p";

        //string path = @"D:\OneDrive - EMISIA SA\EEA\EEA DB CSV FILES\vans\2021 final\data.csv";
        //string targetTable = "vans_2021";

        string path = @"D:\OneDrive - EMISIA SA\EEA\EEA DB CSV FILES\vans\2022 provisional\data.csv";
        string targetTable = "vans_2022_p";



        //string[] fields = new string[] { "ID", "Country", "Ft", "Fm", "Mk", "Cn", "ec (cm3)","r" };
        string[] fields = new string[] { "ID", "Country", "Ft", "Fm", "Mk", "Cn", "ec (cm3)","m (kg)", "Mf (kg)", "r" };

        DataTable table = new DataTable();
        table.Columns.Add("ID", typeof(int));
        table.Columns.Add("Country",typeof(string));
        table.Columns.Add("Ft", typeof(string));
        table.Columns.Add("Fm", typeof(string));
        table.Columns.Add("Mk", typeof(string));
        table.Columns.Add("Cn", typeof(string));
        table.Columns.Add("ec (cm3)", typeof(int));
        table.Columns.Add("m (kg)", typeof(int)); //vans
        table.Columns.Add("Mf (kg)", typeof(int));//vans
        table.Columns.Add("r", typeof(int));


        var file = fileFactory.GetFile(path, Encoding.UTF8,true);
        var c = file.ExistingColumns;

        foreach(var l in file.Read(skipHeader:true))
        {
            if(!l.HasValue) continue;
            var lt = l.Value;
            List<object?> values = new List<object?>();
            

            table.Rows.Add(
                (int?)lt.GetInt("ID", c),
                lt.GetString("Country", c),
                lt.GetString("Ft", c),
                lt.GetString("Fm", c),
                lt.GetString("Mk", c),
                lt.GetString("Cn", c),
                (int?)lt.GetInt("ec (cm3)",c),
                (int?)lt.GetInt("m (kg)", c),//vans
                (int?)lt.GetInt("Mf (kg)", c),//vans
                (int?)lt.GetInt("r", c));
        }

        string connectionString = @"Data Source=DESKTOP-D131KNR\SERVER2019;Initial Catalog=co2;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        SqlConnection connection = new (connectionString);
        connection.Open();
        SqlBulkCopy copier = new (connection);
        copier.BulkCopyTimeout = 0;
        copier.DestinationTableName = targetTable;
        copier.WriteToServer(table);
        connection.Close();

    }
}