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
}