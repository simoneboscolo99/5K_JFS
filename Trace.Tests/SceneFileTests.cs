namespace Trace.Tests;
using System;
using System.IO;

public class SceneFileTests
{
    //[Fact]
    public void TestInputFile()
    {
        var stream = new InputStream(StringWriter("abc   \nd\nef"));
        //Assert.True(stream.Location.LineNum == 1);
        
    }
}