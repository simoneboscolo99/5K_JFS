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
        
        Assert.True(stream.Location.LineNum == 1);
        Assert.True(stream.Location.ColumnNum == 1);
    }
}