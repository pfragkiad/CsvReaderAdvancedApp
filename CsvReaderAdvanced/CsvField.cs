namespace CsvReaderAdvanced;

public class CsvField
{
    public string Name { get; init; } = default!;

    public bool Required { get; init; } = false;

    public string[] Alternatives { get; init; } = Array.Empty<string>();    

    public string[] AlternativeFields { get; init; } = Array.Empty<string>();

    public string? Unit { get; init; }

    public string[] AlternativeUnits { get; init; } = Array.Empty<string>();

    public IEnumerable<string> GetCandidateNames()
    {
        var allNames = Alternatives.Concat(Alternatives.Select(a => a.Replace(" ", ""))).ToList();
        allNames.Add(Name);
        allNames = allNames.Distinct().ToList();

        var allUnits = AlternativeUnits.Concat(AlternativeUnits.Select(u => u.Replace(" ", ""))).ToList();
        if (!string.IsNullOrWhiteSpace(Unit)) allUnits.Add(Unit);
        allUnits = allUnits.Distinct().ToList();

        foreach (string n in allNames)
        {
            yield return n;
            foreach (string u in allUnits)
            {
                yield return $"{n} {u}";
                yield return $"{n} ({u})";
                yield return $"{n} [{u}]";
                yield return $"{n}_{u}";
            }
        }
    }

    public override string ToString() => Name;
}
