namespace StringParser.Abstractions;

public interface IStringParser
{
    /// <summary>
    /// Process the given string
    /// </summary>
    /// <param name="input">Single input string</param>
    /// <returns>Processed string</returns>
    string Parse(string input);
}