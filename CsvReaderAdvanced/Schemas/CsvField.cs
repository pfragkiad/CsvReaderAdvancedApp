namespace CsvReaderAdvanced.Schemas;


public class CsvFieldStats
{
    public BaseType BaseType { get; set; }

    public int ValuesCount { get; set; }
    public int NullValuesCount { get; set; }
    public int UnparsedValuesCount { get; set; }

    public object? Minimum { get; set; }

    public object? Maximum { get; set; }
}


public class CsvField
{
    public string Name { get; init; } = default!;

    public bool Required { get; init; } = false;

    public string[] Alternatives { get; init; } = Array.Empty<string>();

    public string[] AlternativeFields { get; init; } = Array.Empty<string>();

    public string? Unit { get; init; }

    public string[] AlternativeUnits { get; init; } = Array.Empty<string>();

    public HashSet<string> GetCandidateNames(bool ignoreAlternativeUnits)
    {
        var allNames = Alternatives.Concat(Alternatives.Select(a => a.Replace(" ", ""))).ToList();
        allNames.Add(Name);
        allNames = allNames.Distinct().ToList();

        var allUnits = AlternativeUnits.Concat(AlternativeUnits.Select(u => u.Replace(" ", ""))).ToList();
        if (!string.IsNullOrWhiteSpace(Unit)) allUnits.Add(Unit);
        allUnits = allUnits.Distinct().ToList();

        HashSet<string> candidates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string n in allNames)
        {
            candidates.Add(n);
            if (ignoreAlternativeUnits) continue;

            foreach (string u in allUnits)
            {
                candidates.Add($"{n} {u}");
                candidates.Add($"{n} ({u})");
                candidates.Add($"{n} [{u}]");
                candidates.Add($"{n}_{u}");
            }
        }

        return candidates;
    }

    public override string ToString() => Name;
}
