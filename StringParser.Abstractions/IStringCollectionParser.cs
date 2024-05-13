namespace StringParser.Abstractions;

public interface IStringCollectionParser
{
    /// <summary>
    /// Process the given set of strings
    /// </summary>
    /// <param name="input">List of input strings</param>
    /// <returns>List of processed strings</returns>
    IEnumerable<string> Parse(IEnumerable<string> input);
}