using CsvReaderAdvanced.Interfaces;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace CsvReaderAdvanced.Interfaces;

public class CsvFile : ICsvFile
{
    private readonly ILogger<CsvFile> _logger;
    private readonly ICsvReader _reader;

    public CsvFile(ILogger<CsvFile> logger, ICsvReader reader)
    {
        _logger = logger;
        _reader = reader;
    }

    #region Initialization

    public char? Separator { get; private set; }

    public TokenizedLine? Header { get; private set; }

    public List<TokenizedLine?> Lines { get; private set; } = new();

    public void ReadFromFile(string path, Encoding encoding, bool withHeader = true)
    {
        Separator = _reader.ReadSeparator(path, encoding);
        if (Separator is null)
        {
            _logger.LogError("Cannot identify separator. Cannot not read file {path}.", path);
            return;
        }

        using StreamReader reader = new StreamReader(path, Encoding.UTF8);
        if (withHeader)
            Header = _reader.GetTokenizedLine(reader.ReadLine(), 1, 1, null, Separator!.Value);

        Lines = _reader.GetTokenizedLines(reader, Separator!.Value, startLineBeforeRead: withHeader ? 1 : 0).ToList();

        if (withHeader)
            PopulateColumns();
    }

    #endregion


    #region Columns and fields dictionaries and methods

    /// <summary>
    /// The dictionary is filled based on all existing headers.
    /// </summary>
    public Dictionary<string, int> ExistingColumns { get; private set; } = new();

    /// <summary>
    /// The dictionary is filled against a specific list of CsvFields via the PopulateFieldColumns method.
    /// </summary>
    public Dictionary<string, int> AllFieldColumns { get; private set; } = new();

    /// <summary>
    /// The dictionary is filled against a specific list of CsvFields via the PopulateFieldColumns method. It contains only the missing fields.
    /// </summary>
    public List<string> MissingFields => AllFieldColumns.Where(e => e.Value == -1).Select(e => e.Key).ToList();

    public List<string> ExistingFields => AllFieldColumns.Where(e => e.Value >= 0).Select(e => e.Key).ToList();


    public void PopulateColumns()
    {
        _logger.LogDebug("Populating columns from header...");

        ExistingColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (Header.HasValue)
        {
            ExistingColumns = Enumerable.Range(0, Header.Value.Tokens.Count).ToDictionary(i => Header.Value.Tokens[i], i => i, StringComparer.OrdinalIgnoreCase);

            foreach (var e in ExistingColumns)
                _logger.LogDebug("Column: {c}, Position {i}", e.Key, e.Value);
        }
    }

    public void CheckAgainstSchema(CsvSchema schema)
    {
        //attempt to fill the columns
        if (!ExistingColumns.Any()) PopulateColumns();

        if (schema.Fields is null || !schema.Fields.Any())
        {
            _logger.LogError("Schema {name} has no fields.", schema.Name);
            return;
        }

        AllFieldColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in schema.Fields)
        {
            bool found = false;
            foreach (string n in f.GetCandidateNames())
            {
                if (ExistingColumns.ContainsKey(n))
                {
                    AllFieldColumns.Add(f.Name, ExistingColumns[n]);
                    found = true; break;
                }
            }
            if (!found) AllFieldColumns.Add(f.Name, -1);
        }

    }

    #endregion



}
