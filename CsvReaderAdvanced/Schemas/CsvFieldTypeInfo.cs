using System.Globalization;
using CsvReaderAdvanced.Files;

namespace CsvReaderAdvanced.Schemas;

public class CsvFieldTypeInfo
{
    public int Column { get; set; }

    public BaseType BaseType { get; set; } = BaseType.Unknown;

    public int ValuesCount { get; set; }

    public int NullValuesCount { get; set; }

    public int UnparsedValuesCount { get; set; }

    public object? Minimum { get; set; }

    public object? Maximum { get; set; }

    private bool _statsStarted = false;

    /// <summary>
    /// Resets all fields except for the Column and the BaseType
    /// </summary>
    public void StartStats()
    {
        ValuesCount = NullValuesCount = UnparsedValuesCount = 0;
        Minimum = Maximum = null;
        stats = new PartialStats();
    }

    protected struct PartialStats
    {
        public int min = 0, max = 0;
        public long minL = 0, maxL = 0;
        public float minF = 0.0f, maxF = 0.0f;
        public double minD = 0.0, maxD = 0.0;
        public DateTimeOffset minDto = DateTimeOffset.MinValue, maxDto = DateTimeOffset.MaxValue;
        public DateTime minDt = DateTime.MinValue, maxDt = DateTime.MaxValue;
        public int minLength = 0, maxLength = 0; //for strings
        //public int iRow = 0;

        public PartialStats() { }
    }
    PartialStats stats;

    public void ProcessLineForStats(TokenizedLine t)
    {
        ValuesCount++;

        if (BaseType == BaseType.Integer)
        {
            var value = t.GetInt(Column);

            if (value.State == ParseState.Unparsable)
            {
                UnparsedValuesCount++;
                return;
            }

            if (value.State == ParseState.Null)
            {
                NullValuesCount++;
                return;
            }

            if (!_statsStarted)
            {
                stats.min = stats.max = (int)value;
                _statsStarted = true;
                return;
            }
            if ((int)value < stats.min) stats.min = (int)value;
            else if ((int)value > stats.max) stats.max = (int)value;
        }
        else if (BaseType == BaseType.String)
        {
            var value = t.GetString(Column);
            if (value is null)
            {
                NullValuesCount++;
                return;
            }

            //no case of unparsed string value
            //if (!value.IsParsed) 
            //{
            //    UnparsedValuesCount++;
            //    return;
            //}

            int length = value.Length;
            if (!_statsStarted)
            {
                stats.minLength = stats.maxLength = length;
                _statsStarted = true;
                return;
            }
            if (length < stats.minLength) stats.minLength = length;
            else if (length > stats.maxLength) stats.maxLength = length;
        }
        else if (BaseType == BaseType.Float)
        {
            var value = t.GetFloat(Column);
            if (value.State == ParseState.Unparsable)
            {
                UnparsedValuesCount++;
                return;
            }

            if (value.State == ParseState.Null)
            {
                NullValuesCount++;
                return;
            }


            if (!_statsStarted)
            {
                stats.minF = stats.maxF = (float)value;
                _statsStarted = true;
                return;
            }
            if ((float)value < stats.minF) stats.minF = (float)value;
            else if ((float)value > stats.maxF) stats.maxF = (float)value;
        }
        else if (BaseType == BaseType.Double)
        {
            var value = t.GetDouble(Column);
            if (value.State == ParseState.Unparsable)
            {
                UnparsedValuesCount++;
                return;
            }

            if (value.State == ParseState.Null)
            {
                NullValuesCount++;
                return;
            }


            if (!_statsStarted)
            {
                stats.minD = stats.maxD = (double)value;
                _statsStarted = true;
                return;
            }
            if ((double)value < stats.minD) stats.minD = (double)value;
            else if ((double)value > stats.maxD) stats.maxD = (double)value;
        }
        else if (BaseType == BaseType.Long)
        {
            var value = t.GetLong(Column);
            if (value.State == ParseState.Unparsable)
            {
                UnparsedValuesCount++;
                return;
            }

            if (value.State == ParseState.Null)
            {
                NullValuesCount++;
                return;
            }


            if (!_statsStarted)
            {
                stats.minL = stats.maxL = (long)value;
                _statsStarted = true;
                return;
            }
            if ((long)value < stats.minL) stats.minL = (long)value;
            else if ((long)value > stats.maxL) stats.maxL = (long)value;
        }

        else if (BaseType == BaseType.DateTimeOffset)
        {
            var value = t.GetDateTimeOffset(Column);
            if (value.State == ParseState.Unparsable)
            {
                UnparsedValuesCount++;
                return;
            }

            if (value.State == ParseState.Null)
            {
                NullValuesCount++;
                return;
            }


            if (!_statsStarted)
            {
                stats.minDto = stats.maxDto = (DateTimeOffset)value;
                _statsStarted = true;
                return;
            }
            if ((DateTimeOffset)value < stats.minDto) stats.minDto = (DateTimeOffset)value;
            else if ((DateTimeOffset)value > stats.maxDto) stats.maxDto = (DateTimeOffset)value;
        }
        else if (BaseType == BaseType.DateTime)
        {
            var value = t.GetDateTime(Column);
            if (value.State == ParseState.Unparsable)
            {
                UnparsedValuesCount++;
                return;
            }

            if (value.State == ParseState.Null)
            {
                NullValuesCount++;
                return;
            }


            if (!_statsStarted)
            {
                stats.minDt = stats.maxDt = (DateTime)value;
                _statsStarted = true;
                return;
            }
            if ((DateTime)value < stats.minDt) stats.minDt = (DateTime)value;
            else if ((DateTime)value > stats.maxDt) stats.maxDt = (DateTime)value;
        }
        else if (BaseType == BaseType.Boolean)
        {
            var value = t.GetBoolean(Column);

            if (value.State == ParseState.Unparsable)
            {
                UnparsedValuesCount++;
                return;
            }

            if (value.State == ParseState.Null)
            {
                NullValuesCount++;
                return;
            }

            //no further stats (no min/max)
            //if (!_statsStarted)
            //{
            //    minDt = maxDt = (DateTime)value;
            //    _statsStarted = true;
            //    return;
            //}
            //if ((DateTime)value < minDt) minDt = (DateTime)value;
            //else if ((DateTime)value > maxDt) maxDt = (DateTime)value;
        }
    }

    public void FinishStats()
    {
        if (BaseType == BaseType.Integer && _statsStarted)
        {
            Minimum = _statsStarted ? stats.min : null;
            Maximum = _statsStarted ? stats.max : null;
        }
        else if (BaseType == BaseType.Long && _statsStarted)
        {
            Minimum = _statsStarted ? stats.minL : null;
            Maximum = _statsStarted ? stats.maxL : null;
        }
        else if (BaseType == BaseType.Float && _statsStarted)
        {
            Minimum = _statsStarted ? stats.minF : null;
            Maximum = _statsStarted ? stats.maxF : null;

        }
        else if (BaseType == BaseType.Double && _statsStarted)
        {
            Minimum = _statsStarted ? stats.minD : null;
            Maximum = _statsStarted ? stats.maxD : null;
        }
        else if (BaseType == BaseType.DateTimeOffset && _statsStarted)
        {
            Minimum = _statsStarted ? stats.minDto : null;
            Maximum = _statsStarted ? stats.maxDto : null;
        }
        else if (BaseType == BaseType.DateTime && _statsStarted)
        {
            Minimum = _statsStarted ? stats.minDt : null;
            Maximum = _statsStarted ? stats.maxDt : null;
        }
        else if (BaseType == BaseType.String && _statsStarted)
        {
            Minimum = _statsStarted ? stats.minLength : null;
            Maximum = _statsStarted ? stats.maxLength : null;
        }
    }

    /// <summary>
    /// Updates the BaseType if the current value cannot be parsed by the existing type.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="dateTimeFormat"></param>
    /// <param name="dateTimeOffsetFormat"></param>
    public void ProcessLineForBaseType(
        TokenizedLine t,
        string? dateTimeFormat = null,
        string? dateTimeOffsetFormat = null)
    {
        //we do not continue
        if (BaseType == BaseType.String || t.Tokens[Column].Length == 0) return;

        //we arrive here the first time of a non-empty string
        if (BaseType == BaseType.Unknown) BaseType = BaseType.Boolean;

        //check from stricter to less stricter
        if (BaseType == BaseType.Boolean && t.GetBoolean(Column).State == ParseState.Parsed) return;

        BaseType = BaseType.Integer;
        if (BaseType == BaseType.Integer && t.GetInt(Column).State == ParseState.Parsed) return;

        BaseType = BaseType.Long;
        if (BaseType == BaseType.Long && t.GetLong(Column).State == ParseState.Parsed) return;

        BaseType = BaseType.Float;
        if (BaseType == BaseType.Float && t.GetFloat(Column, CultureInfo.InvariantCulture).State == ParseState.Parsed) return;

        BaseType = BaseType.Double;
        if (BaseType == BaseType.Double && t.GetDouble(Column, CultureInfo.InvariantCulture).State == ParseState.Parsed) return;

        //format should be passed for datetime formats
        BaseType = BaseType.DateTime;
        if (BaseType == BaseType.DateTime && t.GetDateTime(Column, CultureInfo.InvariantCulture, dateTimeFormat).State == ParseState.Parsed) return;

        //2022-01-31T00:00:00+00:00
        BaseType = BaseType.DateTimeOffset;
        if (BaseType == BaseType.DateTimeOffset && t.GetDateTimeOffset(Column, CultureInfo.InvariantCulture, dateTimeOffsetFormat).State == ParseState.Parsed) return;

        BaseType = BaseType.String; //no need to continue looping from here

    }
}
