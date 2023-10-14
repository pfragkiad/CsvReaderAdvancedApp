﻿using CsvReaderAdvanced.Interfaces;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

namespace CsvReaderAdvanced;

public class CsvFile : ICsvFile, IDisposable
{
    private readonly ILogger<CsvFile> _logger;
    private readonly ICsvReader _reader;

    public CsvFile(ILogger<CsvFile> logger, ICsvReader reader)
    {
        _logger = logger;
        _reader = reader;
    }

    #region Initialization

    public void Reset ()
    {
        Separator = null;
        Header = null;
        Lines = null;
        ExistingColumns = new();
        ExistingFieldColumns = new();
        MissingFields = new();
        MissingRequiredFields = new();
    }

    public char? Separator { get; private set; }

    public TokenizedLine? Header { get; private set; }


    public void ReadHeader(string path, Encoding encoding)
    {

        Separator = _reader.ReadSeparator(path, encoding);
        if (Separator is null)
        {
            _logger.LogError("Cannot identify separator. Cannot not read file {path}.", path);
            return;
        }

        using StreamReader reader = new StreamReader(path, encoding);
        Header = _reader.GetTokenizedLine(reader.ReadLine(), 1, 1, null, Separator!.Value);
        PopulateColumns();
    }


    public List<TokenizedLine?>? Lines { get; private set; }

    /// <summary>
    /// Reads the Lines from the file. If withHeader is true then the PopulateColumns() is called internally.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="encoding"></param>
    /// <param name="withHeader"></param>
    public void ReadFromFile(string path, Encoding encoding, bool withHeader)
    {
        if (withHeader && Header is null) ReadHeader(path, encoding);

        if (!withHeader || Separator is null)
        {
            Separator = _reader.ReadSeparator(path, encoding);
            if (Separator is null)
            {
                _logger.LogError("Cannot identify separator. Cannot not read file {path}.", path);
                return;
            }
        }

        using StreamReader reader = new StreamReader(path, encoding);
        if (withHeader) _reader.GetTokenizedLine(reader.ReadLine(), 1, 1, null, Separator!.Value);

        Lines = _reader.GetTokenizedLines(reader, Separator!.Value, startLineBeforeRead: withHeader ? 1 : 0).ToList();
    }

    public IEnumerable<TokenizedLine?> Read(string path, Encoding encoding, bool skipHeader)
    {
        Separator = _reader.ReadSeparator(path, encoding);
        if (Separator is null)
        {
            _logger.LogError("Cannot identify separator. Cannot not read file {path}.", path);
            yield break;
        }

        var _streamReader = new StreamReader(path, encoding);
        if (skipHeader) _reader.GetTokenizedLine(_streamReader.ReadLine(), 1, 1, null, Separator!.Value);

        foreach (var line in _reader.GetTokenizedLines(_streamReader, Separator!.Value))
            yield return line;
    }

    #endregion


    #region Columns and fields dictionaries and methods

    /// <summary>
    /// The dictionary is filled based on all existing headers.
    /// </summary>
    public Dictionary<string, int> ExistingColumns { get; private set; } = new();


    /// <summary>
    /// Uses the field name as a key. Includes only the schema fields that exist in the file.
    /// </summary>
    public Dictionary<string, int> ExistingFieldColumns { get; private set; } = new();

    public HashSet<CsvField> MissingFields { get; private set; } = new();

    public HashSet<CsvField> MissingRequiredFields { get; private set; } = new();

    ///// <summary>
    ///// The dictionary is filled against a specific list of CsvFields via the PopulateFieldColumns method. It contains only the missing fields.
    ///// </summary>
    //public List<string> MissingFieldNames => AllFieldColumns.Where(e => e.Value == -1).Select(e => e.Key).ToList();

    //public List<string> ExistingFieldNames => AllFieldColumns.Where(e => e.Value >= 0).Select(e => e.Key).ToList();

    /// <summary>
    /// Updates the ExistingColumns dictionary.
    /// </summary>
    public void PopulateColumns()
    {
        _logger.LogDebug("Populating columns from header...");

        ExistingColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (Header is not null)
        {
            ExistingColumns = Enumerable.Range(0, Header.Value.Tokens.Count).ToDictionary(i => Header.Value.Tokens[i], i => i, StringComparer.OrdinalIgnoreCase);

            foreach (var e in ExistingColumns)
                _logger.LogDebug("Column: {c}, Position {i}", e.Key, e.Value);
        }
    }

    /// <summary>
    /// Updates the ExistingFieldColumns, MissingFields and MissingRequiredFieds properties.
    /// </summary>
    /// <param name="schema"></param>
    public void CheckAgainstSchema(CsvSchema schema)
    {
        //attempt to fill the columns
        if (!ExistingColumns.Any()) PopulateColumns();

        if (schema.Fields is null || !schema.Fields.Any())
        {
            _logger.LogError("Schema {name} has no fields.", schema.Name);
            return;
        }

        //AllFieldColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        ExistingFieldColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        MissingFields = new HashSet<CsvField>();
        MissingRequiredFields = new HashSet<CsvField>();
        foreach (var f in schema.Fields)
        {
            bool found = false;
            var candidatesNames = f.GetCandidateNames(false);
            foreach (string n in candidatesNames)
            {
                if (ExistingColumns.ContainsKey(n))
                {
                    //AllFieldColumns.Add(f.Name, ExistingColumns[n]);
                    ExistingFieldColumns.Add(f.Name, ExistingColumns[n]);
                    found = true; break;
                }
            }
            if (!found)
            {
                MissingFields.Add(f);
                if (f.Required) MissingRequiredFields.Add(f);

                // AllFieldColumns.Add(f.Name, -1);
            }
        }


    }

    public void Dispose() => Reset();

    #endregion

    #region Save to file

    public async Task SavePartialAs(
        string sourcePath,
        string targetPath,
        char targetSeparator,
        Encoding encoding,
        params string[] columnNames)
    {
        //populate existing columns
        if (Header is null) ReadHeader(sourcePath, encoding);
        bool useFields = ExistingFieldColumns.Count>0;
        
        //get the source columns
        int[] columns = useFields ?
            columnNames.Select(c => ExistingFieldColumns[c]).ToArray() :
            columnNames.Select(c => ExistingColumns[c]).ToArray();

        await SavePartialAs(sourcePath, targetPath, targetSeparator, encoding, columns);
    }

    //TODO: Add sample SavePartialAs.
    //TODO: Add data type in schema.
    public async Task SavePartialAs(string sourcePath,
        string targetPath,
        char targetSeparator,
        Encoding encoding,
        params int[] columns)
    {
        using StreamWriter writer = new StreamWriter(targetPath, false, encoding);
        foreach (var line in Read(sourcePath, encoding, skipHeader: false))
            await writer.WriteLineAsync(string.Join(targetSeparator, columns.Select(c => line!.Value.Tokens[c])));
    }

    #endregion

}
