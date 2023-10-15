
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvReaderAdvanced.Files;

public class CsvFileFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly CsvReader _csvReader;

    public CsvFileFactory(
        ILoggerFactory loggerFactory,
        CsvReader csvReader
        )
    {
        _loggerFactory = loggerFactory;
        _csvReader = csvReader;
    }

    //TODO: Add example with CSV file factory

    /// <summary>
    /// Retrieve a CsvFile and read its header.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="encoding"></param>
    /// <param name="withHeader"></param>
    /// <returns></returns>
    public CsvFile GetFile(string path, Encoding encoding, bool withHeader)
    {
        var file = new CsvFile(_loggerFactory.CreateLogger<CsvFile>(), _csvReader, path,encoding,withHeader);
        if (withHeader) file.ReadHeader();
        return file;
    }

    /// <summary>
    /// Retrieve a CsvFile and read its contents.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="encoding"></param>
    /// <param name="withHeader"></param>
    /// <returns></returns>
    public CsvFile ReadWholeFile(string path, Encoding encoding, bool withHeader)
    {
        var file = new CsvFile(_loggerFactory.CreateLogger<CsvFile>(), _csvReader, path,encoding, withHeader);
        file.ReadWholeFile();

        return file;
    }


    public IEnumerable<TokenizedLine?> ReadFile(string path, Encoding encoding, bool skipHeader)
    {
        var file = new CsvFile(_loggerFactory.CreateLogger<CsvFile>(), _csvReader, path,encoding,skipHeader);
        foreach (var line in file.Read(skipHeader))
            yield return line;
    }


}
