using CsvReaderAdvanced.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvReaderAdvanced;

public class CsvFileFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ICsvReader _csvReader;

    public CsvFileFactory(
        ILoggerFactory loggerFactory,
        ICsvReader csvReader
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
    public ICsvFile GetFile(string path, Encoding encoding, bool withHeader)
    {
        var file = new CsvFile(_loggerFactory.CreateLogger<CsvFile>(),_csvReader);
        if (withHeader) file.ReadHeader(path, encoding);

        return file;
    }

    /// <summary>
    /// Retrieve a CsvFile and read its contents.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="encoding"></param>
    /// <param name="withHeader"></param>
    /// <returns></returns>
    public ICsvFile ReadWholeFile(string path, Encoding encoding, bool withHeader)
    {
        var file = new CsvFile(_loggerFactory.CreateLogger<CsvFile>(),_csvReader);
        file.ReadFromFile(path, encoding, withHeader);

        return file;
    }


    public IEnumerable<TokenizedLine?> ReadFile(string path, Encoding encoding, bool skipHeader)
    {
        var file = new CsvFile(_loggerFactory.CreateLogger<CsvFile>(), _csvReader);

        foreach (var line in file.Read(path, encoding, skipHeader))
            yield return line;
    }


}
