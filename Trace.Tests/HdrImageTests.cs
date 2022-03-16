using System.IO;
using System.IO.Compression;
using System.Text;
using Xunit;

namespace Trace.Tests;

public class HdrImageTests
{
    HdrImage image = new(7, 4);
    
    [Fact]
    public void TestCoord()
    {
        Assert.True(image.Valid_Coordinates(0, 0));
        Assert.True(image.Valid_Coordinates(6, 3));
        Assert.False(image.Valid_Coordinates(-1, 0));
        Assert.False(image.Valid_Coordinates(0, -1));
        Assert.False(image.Valid_Coordinates(0, 4));
        Assert.False(image.Valid_Coordinates(7, 0));
    }

    [Fact]
    public void TestPos()
    {
        Assert.True(image.Pixel_Offset(3, 2) == 17);
        Assert.True(image.Pixel_Offset(4, 3) == 25);
        Assert.True(image.Pixel_Offset(6, 3) == 7*4-1);
    }

    [Fact]
    public void TestGetSetPixel()
    {
        Color a = new Color(2.0f, 3.0f, 5.0f);
        image.Set_Pixel(3, 2, a);
        Assert.True(image.Get_Pixel(3, 2).Is_Close(a));
    }

    [Fact]
    public void TestPfmReadLine()
    {
        var line = Encoding.ASCII.GetBytes($"hello\nworld");
        Stream stream = new MemoryStream(line);
        Assert.True(HdrImage.Read_Line(stream) == "hello", "TestPfmReadLine failed");
        Assert.True(HdrImage.Read_Line(stream) == "world", "TestPfmReadLine failed");
        Assert.True(HdrImage.Read_Line(stream) == "", "TestPfmReadLine failed");
    }
}