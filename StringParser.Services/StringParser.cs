using StringParser.Abstractions;

namespace StringParser.Services;

public class StringParser : IStringParser
{
    private const int MaxLength = 15;
    private readonly Dictionary<string, string> _replacements = new ()
    {
        { "$", "£" }
    };
    private readonly HashSet<string> _removals = new() { "4", "_" };


    public string Parse(string input)
    {
        if (string.IsNullOrEmpty(input)) return null;

        var output = string.Empty;

        char? lastChar = null;
        foreach (var c in input)
        {
            // Remove duplicates
            if (c == lastChar) continue;
            lastChar = c;

            // Remove forbidden chars
            if (_removals.Contains(c.ToString())) continue;

            // Add either the current char or the replacement char
            output += _replacements.ContainsKey(c.ToString())
                ? _replacements[c.ToString()]
                : c;
        }

        return output.Length > MaxLength
            ? output[..MaxLength]
            : output;
    }
}
