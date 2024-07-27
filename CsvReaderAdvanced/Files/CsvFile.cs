
using CsvReaderAdvanced.Schemas;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Globalization;
using System.Text;

namespace CsvReaderAdvanced.Files;

public class CsvFile : IDisposable
{
    private readonly ILogger<CsvFile> _logger;
    private readonly CsvReader _reader;

    private readonly string _path;
    private readonly Encoding _encoding;
    private readonly bool _hasHeader;

    public CsvFile(ILogger<CsvFile> logger, CsvReader reader, string path, Encoding encoding, bool withHeader)
    {
        _logger = logger;
        _reader = reader;

        _path = path;
        _encoding = encoding;
        _hasHeader = withHeader;
    }

    #region Initialization

    public void Reset()
    {
        Separator = null;
        Header = null;
        Lines = null;
        ExistingColumns = new();
        ExistingFieldColumns = new();
        MissingFields = new();
        MissingRequiredFields = new();
        ExistingFieldTypeInfos = new();
    }

    public char? Separator { get; private set; }

    public TokenizedLine? Header { get; private set; }


    public void ReadHeader()
    {
        Separator = _reader.ReadSeparator(_path, _encoding);
        if (Separator is null)
        {
            _logger.LogError("Cannot identify separator. Cannot not read file {path}.", _path);
            return;
        }

        using StreamReader reader = new StreamReader(_path, _encoding);
        Header = _reader.GetTokenizedLine(reader.ReadLine(), 1, 1, null, Separator!.Value);
        PopulateColumns();
    }


    public List<TokenizedLine?>? Lines { get; private set; }

    /// <summary>
    /// Reads the Lines from the file. If _hasHeader is true then the PopulateColumns() is called internally.
    /// </summary>
    public void ReadWholeFile()
    {
        if (_hasHeader && Header is null) ReadHeader();

        if (!_hasHeader || Separator is null)
        {
            Separator = _reader.ReadSeparator(_path, _encoding);
            if (Separator is null)
            {
                _logger.LogError("Cannot identify separator. Cannot not read file {path}.", _path);
                return;
            }
        }

        using StreamReader reader = new StreamReader(_path, _encoding);
        if (_hasHeader) _reader.GetTokenizedLine(reader.ReadLine(), 1, 1, null, Separator!.Value);

        Lines = _reader.GetTokenizedLines(reader, Separator!.Value, startLineBeforeRead: _hasHeader ? 1 : 0).ToList();
    }

    /// <summary>
    /// Starts reading the file synchronously.Use a for each structure to read the file line by line.
    /// </summary>
    /// <param name="skipHeader"></param>
    /// <returns></returns>
    public IEnumerable<TokenizedLine?> Read(bool skipHeader)
    {
        Separator = _reader.ReadSeparator(_path, _encoding);
        if (Separator is null)
        {
            _logger.LogError("Cannot identify separator. Cannot not read file {path}.", _path);
            yield break;
        }

        using StreamReader _streamReader = new StreamReader(_path, _encoding);
        if (skipHeader && _hasHeader) _reader.GetTokenizedLine(_streamReader.ReadLine(), 1, 1, null, Separator!.Value);

        foreach (var line in _reader.GetTokenizedLines(_streamReader, Separator!.Value))
            yield return line;

        //_streamReader.Close();
    }

    public IEnumerable<TokenizedLine?> ReadFast(bool skipHeader)
    {
        Separator = _reader.ReadSeparator(_path, _encoding);
        if (Separator is null)
        {
            _logger.LogError("Cannot identify separator. Cannot not read file {path}.", _path);
            yield break;
        }

        using StreamReader _streamReader = new StreamReader(_path, _encoding);
        if (skipHeader && _hasHeader)
        {
            //skip whitespace before the header
            //string? line;
            //do
            //{
            //    line = _streamReader.ReadLine();
            //} while (string.IsNullOrWhiteSpace(line));

            _reader.GetTokenizedLineFast(_streamReader, Separator!.Value, true);
        }

        int lineCounter = 1;
        foreach (var line in _reader.GetTokenizedLinesFast(_streamReader, Separator!.Value, true, ++lineCounter))
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

    public List<CsvFieldTypeInfo> ExistingFieldTypeInfos { get; private set; } = new();

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

    #region Data types



    public void UpdateFieldBaseTypes(
        int maxRows = int.MaxValue,
        string? dateTimeFormat = null,
        string? dateTimeOffsetFormat = null)
    {
        //read header and populate columns
        if (Header is null) ReadHeader();

        //initialize fieldtypeinfos based on all the existing fields
        if (ExistingFieldTypeInfos.Count == 0)
            ExistingFieldTypeInfos = ExistingColumns.Select(e => new CsvFieldTypeInfo() { Column = e.Value, BaseType = BaseType.Unknown }).ToList();

        UpdateFieldBaseTypes(ExistingFieldTypeInfos, maxRows, dateTimeFormat, dateTimeOffsetFormat);
    }


    public void UpdateFieldBaseTypes( //one pass for all fields
        IEnumerable<CsvFieldTypeInfo> fields,
        int maxRows = int.MaxValue,
        string? dateTimeFormat = null,
        string? dateTimeOffsetFormat = null)
    {

        var lines = Read(_hasHeader);
        int iRow = 0;
        foreach (var line in lines)
        {
            if (line is null) continue;
            iRow++; if (iRow > maxRows) break;

            var t = line.Value;


            foreach (var f in fields)
            //Parallel.ForEach(fields, f =>
            {
                if (f.BaseType == BaseType.String ||
                    t.Tokens.Count - 1 <= f.Column ||
                    t.Tokens[f.Column].Length == 0)
                    //return;
                    continue;

                f.ProcessLineForBaseType(t, dateTimeFormat, dateTimeOffsetFormat);
            }
            //);
        }
    }

    private BaseType GetBaseType(int column, BaseType assumedType, int maxRows, string? dateTimeFormat, string? dateTimeOffsetFormat, IEnumerable<TokenizedLine?> lines)
    {
        CsvFieldTypeInfo stats = new CsvFieldTypeInfo() { Column = column, BaseType = assumedType };

        int iRow = 0;
        foreach (var line in lines)
        {
            if (line is null) continue;
            iRow++;
            if (iRow > maxRows) break;

            var t = line.Value;
            if (t.Tokens[column].Length == 0) continue;

            stats.ProcessLineForBaseType(t, dateTimeFormat, dateTimeOffsetFormat);
        }
        return stats.BaseType;
    }

    public BaseType GetBaseType(
        int column,
        BaseType assumedType = BaseType.Unknown,
        int maxRows = int.MaxValue,
        string? dateTimeFormat = null,
        string? dateTimeOffsetFormat = null)
    {
        var lines = Lines is not null ? Lines : Read(_hasHeader);
        return GetBaseType(column, assumedType, maxRows, dateTimeFormat, dateTimeOffsetFormat, lines);
    }

    public bool ConfirmAssumedType(
        int column,
        BaseType assumedType,
        int maxRows = int.MaxValue)
    {
        if (assumedType == BaseType.Unknown) return true;

        int iRow = 0;

        var lines = Lines is not null ? Lines : Read(_hasHeader);
        foreach (var line in lines)
        {
            if (line is null) continue;
            var tokenizedLine = line.Value;

            if (tokenizedLine.Tokens[column].Length > 0 && assumedType == BaseType.Unknown)
                assumedType = BaseType.Integer;

            iRow++;
            if (iRow > maxRows) return true;

            if (
                assumedType == BaseType.Integer && !tokenizedLine.GetInt(column).IsParsed ||
                assumedType == BaseType.Long && !tokenizedLine.GetLong(column).IsParsed ||
                assumedType == BaseType.Float && !tokenizedLine.GetFloat(column, CultureInfo.InvariantCulture).IsParsed ||
                assumedType == BaseType.Double && !tokenizedLine.GetDouble(column, CultureInfo.InvariantCulture).IsParsed ||
                assumedType == BaseType.DateTime && !tokenizedLine.GetDateTime(column, CultureInfo.InvariantCulture, "yyyy-MM-dd").IsParsed)
                return false;
        }

        return true;
    }


    #region Field stats

    public void UpdateFieldStats(int maxRows = int.MaxValue)
    {
        UpdateFieldStats(this.ExistingFieldTypeInfos, maxRows);
    }

    private CsvFieldTypeInfo GetFieldStats(
        int column, //when lines are pre-loaded
        BaseType assumedType,
        int maxRows,
        IEnumerable<TokenizedLine?> lines)
    {
        CsvFieldTypeInfo stats = new CsvFieldTypeInfo() { BaseType = assumedType, Column = column };

        stats.StartStats();

        int iRow = 0;
        foreach (var line in lines)
        {
            if (!line.HasValue) continue;
            iRow++; if (iRow > maxRows) break;

            var t = line.Value;
            stats.ProcessLineForStats(t);
        }
        stats.FinishStats();

        return stats;
    }

    public CsvFieldTypeInfo GetFieldStats(int column, //prefer this version
        BaseType assumedType,
        int maxRows = int.MaxValue)
    {
        var lines = Lines is not null ? Lines : Read(_hasHeader);
        return GetFieldStats(column, assumedType, maxRows, lines);
    }

    //best of all (single pass for all fields
    public void UpdateFieldStats(
        IEnumerable<CsvFieldTypeInfo> fields,
        int maxRows = int.MaxValue)
    {
        //initialize stats
        foreach (var f in fields)
            f.StartStats();

        int iRow = 0;
        var lines = Lines is not null ? Lines : Read(_hasHeader);
        foreach (var line in lines)
        {
            if (!line.HasValue) continue;
            iRow++; if (iRow > maxRows) break;

            var t = line.Value;
            foreach (var f in fields)
                //Parallel.ForEach(fields,f =>
                f.ProcessLineForStats(t);
            //);
        }

        foreach (var f in fields)
            f.FinishStats();
    }
    #endregion


    #endregion

    #region Save to file

    public async Task SavePartialAs(
        string targetPath,
        char targetSeparator,
        params string[] columnNames)
    {
        //populate existing columns
        if (Header is null) ReadHeader();
        bool useFields = ExistingFieldColumns.Count > 0;

        //get the source columns
        int[] columns = useFields ?
            columnNames.Select(c => ExistingFieldColumns[c]).ToArray() :
            columnNames.Select(c => ExistingColumns[c]).ToArray();

        await SavePartialAs(targetPath, targetSeparator, columns);
    }

    //TODO: Add sample SavePartialAs.
    //TODO: Add data type in schema.
    public async Task SavePartialAs(
        string targetPath,
        char targetSeparator,
        params int[] columns)
    {
        using StreamWriter writer = new StreamWriter(targetPath, false, _encoding);
        foreach (var line in Read(skipHeader: false))
            await writer.WriteLineAsync(string.Join(targetSeparator, columns.Select(c =>
            {
                string token = line!.Value.Tokens[c];
                if (token.Contains(targetSeparator)) token = $"\"{token}\"";
                return token;
            })));
    }

    #endregion

}
