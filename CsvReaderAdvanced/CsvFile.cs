using CsvReaderAdvanced.Interfaces;
using CsvReaderAdvanced.Schemas;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

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

    public void Reset()
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

    /// <summary>
    /// Starts reading the file synchronously.Use a for each structure to read the file line by line.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="encoding"></param>
    /// <param name="skipHeader"></param>
    /// <returns></returns>
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

    #region Data types

    public BaseType GetBaseType(
        int column,
        string path,
        Encoding encoding,
        BaseType assumedType = BaseType.Unknown,
        bool hasHeader = true,
        int maxRows = int.MaxValue,
        string? dateTimeFormat = null,
        string? dateTimeOffsetFormat = null)
    {
        int iRow = 0;

        var lines = Read(path, encoding, hasHeader);
        foreach (var line in lines)
        {
            if (line is null) continue;
            var tokenizedLine = line.Value;

            iRow++;
            if (iRow > maxRows) break;

            if (tokenizedLine.Tokens[column].Length == 0) continue;

            //we arrive here the first time of a non-empty string
            if (assumedType == BaseType.Unknown) assumedType = BaseType.Boolean;

            //check from stricter to less stricter
            if (assumedType == BaseType.Boolean && tokenizedLine.GetBoolean(column).IsParsed) continue;

            assumedType = BaseType.Integer;
            if (assumedType == BaseType.Integer && tokenizedLine.GetInt(column).IsParsed) continue;

            assumedType = BaseType.Long;
            if (assumedType == BaseType.Long && tokenizedLine.GetLong(column).IsParsed) continue;

            assumedType = BaseType.Float;
            if (assumedType == BaseType.Float && tokenizedLine.GetFloat(column, CultureInfo.InvariantCulture).IsParsed) continue;

            assumedType = BaseType.Double;
            if (assumedType == BaseType.Double && tokenizedLine.GetDouble(column, CultureInfo.InvariantCulture).IsParsed) continue;

            //format should be passed for datetime formats
            assumedType = BaseType.DateTime;
            if (assumedType == BaseType.DateTime && tokenizedLine.GetDateTime(column, CultureInfo.InvariantCulture, dateTimeFormat).IsParsed) continue;

            //2022-01-31T00:00:00+00:00
            assumedType = BaseType.DateTimeOffset;
            if (assumedType == BaseType.DateTimeOffset && tokenizedLine.GetDateTimeOffset(column, CultureInfo.InvariantCulture, dateTimeOffsetFormat).IsParsed) continue;

            //assumedType = BaseType.String;
            return BaseType.String; //no need to continue looping from here
        }

        return assumedType;
    }

    public bool ConfirmAssumedType(
        int column,
        string path,
        Encoding encoding,
        BaseType assumedType,
        bool hasHeader = true,
        int maxRows = int.MaxValue)
    {
        if (assumedType == BaseType.Unknown) return true;

        int iRow = 0;

        var lines = Read(path, encoding, hasHeader);
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


    public CsvFieldStats GetFieldStats(int column,
        string path,
        Encoding encoding,
        BaseType assumedType,
        bool hasHeader = true,
        int maxRows = int.MaxValue)
    {

        bool initialized = false;
        int min = 0, max = 0;
        long minL = 0, maxL = 0;
        float minF = 0.0f, maxF = 0.0f;
        double minD = 0.0, maxD = 0.0;
        DateTimeOffset minDto = DateTimeOffset.MinValue, maxDto = DateTimeOffset.MaxValue;
        DateTime minDt = DateTime.MinValue, maxDt = DateTime.MaxValue;
        int minLength = 0, maxLength = 0; //for strings


        int nullValues = 0, allValues = 0, unparsedValues = 0;

        int iRow = 0;
        foreach (var line in Read(path, encoding, hasHeader))
        {
            if (!line.HasValue) continue;
            var t = line.Value;

            iRow++; if (iRow > maxRows) break;

            allValues++;

            if (assumedType == BaseType.Integer)
            {
                var value = t.GetInt(column);
                if (!value.IsParsed)
                {
                    unparsedValues++;
                    continue;
                }

                if (value.IsNull)
                {
                    nullValues++;
                    continue;
                }


                if (!initialized)
                {
                    min = max = (int)value;
                    initialized = true;
                    continue;
                }
                if ((int)value < min) min = (int)value;
                else if ((int)value > max) max = (int)value;
            }
            else if (assumedType == BaseType.String)
            {
                var value = t.GetString(column);
                if (value is null)
                {
                    nullValues++;
                    continue;
                }

                //no case of unparsed string value
                //if (!value.IsParsed) 
                //{
                //    unparsedValues++;
                //    continue;
                //}

                int length = value.Length;
                if (!initialized)
                {
                    minLength = maxLength = length;
                    initialized = true;
                    continue;
                }
                if (length < minLength) minLength = length;
                else if (length > maxLength) maxLength = length;
            }
            else if (assumedType == BaseType.Float)
            {
                var value = t.GetFloat(column);
                if (!value.IsParsed)
                {
                    unparsedValues++;
                    continue;
                }
             
                if (value.IsNull)
                {
                    nullValues++;
                    continue;
                }


                if (!initialized)
                {
                    minF = maxF = (float)value;
                    initialized = true;
                    continue;
                }
                if ((float)value < minF) minF = (float)value;
                else if ((float)value > maxF) maxF = (float)value;
            }
            else if (assumedType == BaseType.Double)
            {
                var value = t.GetDouble(column);
                 if (!value.IsParsed)
                {
                    unparsedValues++;
                    continue;
                }

               if (value.IsNull)
                {
                    nullValues++;
                    continue;
                }


                if (!initialized)
                {
                    minD = maxD = (double)value;
                    initialized = true;
                    continue;
                }
                if ((double)value < minD) minD = (double)value;
                else if ((double)value > maxD) maxD = (double)value;
            }
            else if (assumedType == BaseType.Long)
            {
                var value = t.GetLong(column);
                 if (!value.IsParsed)
                {
                    unparsedValues++;
                    continue;
                }

               if (value.IsNull)
                {
                    nullValues++;
                    continue;
                }


                if (!initialized)
                {
                    minL = maxL = (long)value;
                    initialized = true;
                    continue;
                }
                if ((long)value < minL) minL = (long)value;
                else if ((long)value > maxL) maxL = (long)value;
            }

            else if (assumedType == BaseType.DateTimeOffset)
            {
                var value = t.GetDateTimeOffset(column);
                if (!value.IsParsed)
                {
                    unparsedValues++;
                    continue;
                }

                if (value.IsNull)
                {
                    nullValues++;
                    continue;
                }


                if (!initialized)
                {
                    minDto = maxDto = (DateTimeOffset)value;
                    initialized = true;
                    continue;
                }
                if ((DateTimeOffset)value < minDto) minDto = (DateTimeOffset)value;
                else if ((DateTimeOffset)value > maxDto) maxDto = (DateTimeOffset)value;
            }
            else if (assumedType == BaseType.DateTime)
            {
                var value = t.GetDateTime(column);
                 if (!value.IsParsed)
                {
                    unparsedValues++;
                    continue;
                }
               if (value.IsNull)
                {
                    nullValues++;
                    continue;
                }


                if (!initialized)
                {
                    minDt = maxDt = (DateTime)value;
                    initialized = true;
                    continue;
                }
                if ((DateTime)value < minDt) minDt = (DateTime)value;
                else if ((DateTime)value > maxDt) maxDt = (DateTime)value;
            }
            else if (assumedType == BaseType.Boolean)
            {
                var value = t.GetBoolean(column);

                if (!value.IsParsed)
                {
                    unparsedValues++;
                    continue;
                }

                if (value.IsNull)
                {
                    nullValues++;
                    continue;
                }

                //no further stats (no min/max)
                //if (!initialized)
                //{
                //    minDt = maxDt = (DateTime)value;
                //    initialized = true;
                //    continue;
                //}
                //if ((DateTime)value < minDt) minDt = (DateTime)value;
                //else if ((DateTime)value > maxDt) maxDt = (DateTime)value;
            }

        }

        var stats = new CsvFieldStats()
        {
            BaseType = assumedType,
            ValuesCount = allValues,
            NullValuesCount = nullValues,
            UnparsedValuesCount = unparsedValues
        };
        if (assumedType == BaseType.Integer && initialized)
        {
            stats.Minimum = initialized ? min : null;
            stats.Maximum = initialized ? max : null;
        }
        else if (assumedType == BaseType.Long && initialized)
        {
            stats.Minimum = initialized ? minL : null;
            stats.Maximum = initialized ? maxL : null;
        }
        else if (assumedType == BaseType.Float && initialized)
        {
            stats.Minimum = initialized ? minF : null;
            stats.Maximum = initialized ? maxF : null;

        }
        else if (assumedType == BaseType.Double && initialized)
        {
            stats.Minimum = initialized ? minD : null;
            stats.Maximum = initialized ? maxD : null;
        }
        else if (assumedType == BaseType.DateTimeOffset && initialized)
        {
            stats.Minimum = initialized ? minDto : null;
            stats.Maximum = initialized ? maxDto : null;
        }
        else if (assumedType == BaseType.DateTime && initialized)
        {
            stats.Minimum = initialized ? minDt : null;
            stats.Maximum = initialized ? maxDt : null;
        }
        else if (assumedType == BaseType.String && initialized)
        {
            stats.Minimum = initialized ? minLength : null;
            stats.Maximum = initialized ? maxLength : null;
        }

        //else if (assumedType == BaseType.Boolean && initialized)
        //{
        //    stats.Minimum = initialized ? minDt : null;
        //    stats.Maximum = initialized ? maxDt : null;
        //}

        return stats;
    }

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
        bool useFields = ExistingFieldColumns.Count > 0;

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
            await writer.WriteLineAsync(string.Join(targetSeparator, columns.Select(c =>
            {
                string token = line!.Value.Tokens[c];
                if (token.Contains(targetSeparator)) token = $"\"{token}\"";
                return token;
            })));
    }

    #endregion

}
