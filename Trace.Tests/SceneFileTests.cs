using System.IO;
using System.Text;
using Xunit;

namespace Trace.Tests;

public class SceneFileTests
{
    [Fact]
    public void TestInputFile()
    {
        var line = Encoding.ASCII.GetBytes("abc   \nd\nef");
        Stream streamline = new MemoryStream(line);
        var stream = new InputStream(streamline);
        
        Assert.True(stream.Location.LineNum == 1, "Test line 0");
        Assert.True(stream.Location.ColumnNum == 1, "Test col 0");
        
        Assert.True(stream.ReadChar() == "a", "Test read 1");
        Assert.True(stream.Location.LineNum == 1, "Test line 1");
        Assert.True(stream.Location.ColumnNum == 2, "Test col 1");
        
        stream.UnreadChar("A");
        Assert.True(stream.ReadChar() == "A", "Test unread + read 2");
        Assert.True(stream.Location.LineNum == 1, "Test line 2");
        Assert.True(stream.Location.ColumnNum == 2, "Test col 2");
        
        Assert.True(stream.ReadChar() == "b", "Test read 3");
        Assert.True(stream.Location.LineNum == 1, "Test line 3");
        Assert.True(stream.Location.ColumnNum == 3, "Test col 3");
        
        Assert.True(stream.ReadChar() == "c", "Test read 4");
        Assert.True(stream.Location.LineNum == 1, "Test line 4");
        Assert.True(stream.Location.ColumnNum == 4, "Test col 4");
        
        stream.SkipWhitespacesAndComments();
        Assert.True(stream.ReadChar() == "d", "Test read 5");
        Assert.True(stream.Location.LineNum == 2, "Test line 5");
        Assert.True(stream.Location.ColumnNum == 2, "Test col 5");
        
        Assert.True(stream.ReadChar() == "\n", "Test read 6");
        Assert.True(stream.Location.LineNum == 3, "Test line 6");
        Assert.True(stream.Location.ColumnNum == 1, "Test col 6");
        
        Assert.True(stream.ReadChar() == "e", "Test read 7");
        Assert.True(stream.Location.LineNum == 3, "Test line 7");
        Assert.True(stream.Location.ColumnNum == 2, "Test col 7");
        
        Assert.True(stream.ReadChar() == "f", "Test read 8");
        Assert.True(stream.Location.LineNum == 3, "Test line 8");
        Assert.True(stream.Location.ColumnNum == 3, "Test col 8");
        
        Assert.True(stream.ReadChar() == "", "Test end of file");
    }

    [Fact]
    public void TestLexer()
    {
        var line = Encoding.ASCII.GetBytes(@"
# This is a comment
# This is another comment
new material sky_material(
diffuse(image(""my file.pfm"")),
) # Comment at the end of the line");
        //To enable C # keywords to be used as identifiers.
        // The @ character precedes a code element that the compiler
        // must be an identifier rather than a C # keyword 
        Stream streamline = new MemoryStream(line);
        var stream = new InputStream(streamline);

        AssertToken.AssertIsKeyword(stream.ReadToken(), KeywordEnum.New);
        AssertToken.AssertIsKeyword(stream.ReadToken(), KeywordEnum.Material);
        AssertToken.AssertIsIdentifier(stream.ReadToken(), "sky_material");
        AssertToken.AssertIsSymbol(stream.ReadToken(), "(");
        AssertToken.AssertIsKeyword(stream.ReadToken(), KeywordEnum.Diffuse);
        AssertToken.AssertIsSymbol(stream.ReadToken(), "(");
        AssertToken.AssertIsKeyword(stream.ReadToken(), KeywordEnum.Image);
        AssertToken.AssertIsSymbol(stream.ReadToken(), "(");
        AssertToken.AssertIsString(stream.ReadToken(), "my file.pfm");
        AssertToken.AssertIsSymbol(stream.ReadToken(), ")");
        AssertToken.AssertIsSymbol(stream.ReadToken(), ")");
        AssertToken.AssertIsSymbol(stream.ReadToken(), ",");
        AssertToken.AssertIsSymbol(stream.ReadToken(), "<");
        AssertToken.AssertIsNumber(stream.ReadToken(), 5.0f);
        AssertToken.AssertIsSymbol(stream.ReadToken(), ",");
        AssertToken.AssertIsNumber(stream.ReadToken(), 500.0f);
        AssertToken.AssertIsSymbol(stream.ReadToken(), ",");
        AssertToken.AssertIsNumber(stream.ReadToken(), 300.0f);
        AssertToken.AssertIsSymbol(stream.ReadToken(), ">");
        AssertToken.AssertIsSymbol(stream.ReadToken(), ")");
        Assert.IsType<StopToken>(stream.ReadToken());
    }
}

public class AssertToken
{
    public static void AssertIsKeyword(Token token, KeywordEnum keyword)
    {
        Assert.IsType<KeywordToken>(token);
        Assert.True(
            ((KeywordToken) token).Keyword == keyword, 
            $"Token {token} is not equal to keyword {keyword}"
            );
    }

    public static void AssertIsIdentifier(Token token, string identifier)
    {
        Assert.IsType<IdentifierToken>(token);
        Assert.True(
            ((IdentifierToken) token).Identifier == identifier, 
            $"Expecting identifier {identifier} instead of {token}" 
            );
    }

    public static void AssertIsSymbol(Token token, string symbol)
    {
        Assert.IsType<SymbolToken>(token);
        Assert.True(
            ((SymbolToken) token).Symbol == symbol,
            $"Expecting symbol {symbol} instead of {token}"
            );
    }

    public static void AssertIsString(Token token, string s)
    {
        Assert.IsType<StringToken>(token);
        Assert.True(
            ((StringToken) token).Str == s,
            $"Token {token} is not equal to string {s}"
            );
    }

    public static void AssertIsNumber(Token token, float number)
    {
        Assert.IsType<LiteralNumberToken>(token);
        Assert.True(
            Functions.Are_Close(((LiteralNumberToken) token).Value, number),
            $"Token {token} is not equal to number {number}"
        );
    }
}