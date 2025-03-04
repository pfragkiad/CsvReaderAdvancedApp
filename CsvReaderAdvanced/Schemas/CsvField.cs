﻿namespace CsvReaderAdvanced.Schemas;


public class CsvField
{
    public string Name { get; init; } = default!;

    public bool Required { get; init; } = false;

    public HashSet<string> Alternatives { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public HashSet<string> AlternativeFields { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public string? Unit { get; init; }

    public HashSet<string> AlternativeUnits { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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
