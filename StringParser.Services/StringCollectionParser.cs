using StringParser.Abstractions;

namespace StringParser.Services;

public class StringCollectionParser : IStringCollectionParser
{
    private readonly IStringParser _parser;

    public StringCollectionParser(IStringParser parser)
    {
        _parser = parser;
    }

    public IEnumerable<string> Parse(IEnumerable<string> input)
    {
        var result = new List<string>();
        foreach (var item in input)
            result.Add(_parser.Parse(item));

        return result;
    }
}
