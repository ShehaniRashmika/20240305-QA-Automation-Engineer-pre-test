using Moq;
using NUnit.Framework;
using Shouldly;
using StringParser.Abstractions;
using StringParser.Services;
using System.Text;

namespace StringParser.UnitTests;

[TestFixture]
public class StringParserTests
{
    private StringCollectionParser _stringCollectionParser;
    private Mock<IStringParser> _parserMock;

    [SetUp]
    public void Setup()
    {
        _parserMock = new Mock<IStringParser>();
        _stringCollectionParser = new StringCollectionParser(_parserMock.Object);
    }

    [Test]
    public void Parse_EmptyInput_ReturnsEmptyCollection()
    {
        // Act
        var result = _stringCollectionParser.Parse(new List<string>());

        // Assert using NUnit
        Assert.That(result, Is.Empty);

        // Assert using shoudly
        result.ShouldBeEmpty();
    }

    [Test]
    public void Parse_NullInput_ReturnsNotEmptyCollection()
    {
        // Act
        var result = _stringCollectionParser.Parse(Enumerable.Repeat<string>(null, 5));

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public void Parse_CallsStringParserForEachItem()
    {
        // Arrange
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns<string>(s => s);
        var input = new List<string> { "abc", "def", "ghi" };
        var expectedOutput = new List<string> { "abc", "def", "ghi" };

        // Act
        var result = _stringCollectionParser.Parse(input);

        // Assert
        Assert.That(result, Is.EqualTo(expectedOutput));

        for (int i = 0; i < input.Count; i++)
        {
            Assert.That(expectedOutput[i], Is.EqualTo(result.ToList()[i]));
            _parserMock.Verify(p => p.Parse(input[i]), Times.Once);
        }
    }

    [Test]
    public void Parse_InputWithVariousCharacters_ProcessesCorrectly()
    {
        // Arrange
        _parserMock.Setup(p => p.Parse(It.IsAny<string>()))
                   .Returns<string>(s => s.Replace("$", "£").Replace("_", "").Replace("4", ""));
        var input = new List<string> { "hello", "world!$", "_test_4" };
        var expectedOutput = new List<string> { "hello", "world!£", "test" };

        // Act
        var result = _stringCollectionParser.Parse(input);

        // Assert
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Parse_InputWithDuplicates_ProcessesCorrectly()
    {
        // Arrange
        _parserMock.Setup(p => p.Parse(It.IsAny<string>()))
                   .Returns<string>(s =>
                   {
                       var processed = new StringBuilder();
                       char? lastChar = null;

                       foreach (var c in s)
                       {
                           if (c != lastChar)
                           {
                               processed.Append(c);
                               lastChar = c;
                           }
                       }
                       return processed.ToString();
                   });

        var input = new List<string> { "aaabbbccc", "hello", "world!!!" };
        var expectedOutput = new List<string> { "abc", "helo", "world!" };

        // Act
        var result = _stringCollectionParser.Parse(input);

        // Assert
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Parse_InputExceedsMaxLength_TruncatesCorrectly()
    {
        // Arrange
        _parserMock.Setup(p => p.Parse(It.IsAny<string>()))
                   .Returns<string>(s => s.Substring(0, Math.Min(s.Length, 15)));
        var input = new List<string> { "abcdefghijklmnopqrstuvwxyz", "12345678901234567890" };
        var expectedOutput = new List<string> { "abcdefghijklmno", "123456789012345" };

        // Act
        var result = _stringCollectionParser.Parse(input);

        // Assert
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Parse_InputWithEmptyOrNullString_ProcessesCorrectly()
    {
        // Arrange
        _parserMock.Setup(p => p.Parse(It.IsAny<string>()))
            .Returns<string>(s =>
            {
                {
                    if (s == "" || s == null)
                    {
                        return null;
                    }
                    else
                    {
                        return s;
                    }
                }
            }
            );

        var input = new List<string> { "", null, "def", "ghi" };
        var expectedOutput = new List<string> { null, null, "def", "ghi" };

        // Act
        var result = _stringCollectionParser.Parse(input);

        // Assert
        Assert.That(result, Is.EqualTo(expectedOutput));

        for (int i = 0; i < input.Count; i++)
        {
            Assert.That(expectedOutput[i], Is.EqualTo(result.ToList()[i]));
            _parserMock.Verify(p => p.Parse(input[i]), Times.Once);
        }
    }
}

// Paths to cover
// Input collection with null or empty strings.
// Input strings with different lengths and characters.
// Testing the truncation to a maximum length of 15 characters.
// Testing the removal of contiguous duplicate characters.
// Testing the replacement of '$' with '£'.
// Testing the removal of underscores and number '4'.
// Ensuring that the returned collection is not null.