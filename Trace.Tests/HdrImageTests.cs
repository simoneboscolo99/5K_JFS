using System;
using System.Globalization;
using System.IO;
using System.Linq;
//using System.IO.Compression;
using System.Text;
using Xunit;


namespace Trace.Tests;

public class HdrImageTests
{
    HdrImage image = new(7, 4);

    [Fact]
    public void TestCoord()
    {
        Assert.True(image.Valid_Coordinates(0, 0), "Test 1");
        Assert.True(image.Valid_Coordinates(6, 3), "Test 2");
        Assert.False(image.Valid_Coordinates(-1, 0), "Test 3");
        Assert.False(image.Valid_Coordinates(0, -1), "Test 4");
        Assert.False(image.Valid_Coordinates(0, 4), "Test 5");
        Assert.False(image.Valid_Coordinates(7, 0), "Test 6");
    }

    [Fact]
    public void TestPos()
    {
        Assert.True(image.Pixel_Offset(3, 2) == 17, "Test 1");
        Assert.True(image.Pixel_Offset(4, 3) == 25, "Test 2");
        Assert.True(image.Pixel_Offset(6, 3) == 7 * 4 - 1, "Test 3");
    }

    [Fact]
    public void TestGetSetPixel()
    {
        Color a = new Color(2.0f, 3.0f, 5.0f);
        image.Set_Pixel(3, 2, a);
        Assert.True(image.Get_Pixel(3, 2).Is_Close(a), "Test 1");
    }

    [Fact]
    public void TestPfmReadLine()
    {
        var line = Encoding.ASCII.GetBytes($"hello\nworld");
        Stream stream = new MemoryStream(line);
        Assert.True(HdrImage.Read_Line(stream) == "hello", "Test 1");
        Assert.True(HdrImage.Read_Line(stream) == "world", "Test 2");
        Assert.True(HdrImage.Read_Line(stream) == "", "Test 3");
    }

    [Fact]
    public void TestPfmParseEndianness()
    {
        Assert.False(HdrImage.Parse_Endianness("1.0"), "Test BE 1");
        Assert.False(HdrImage.Parse_Endianness("4.2"), "Test BE 2");
        Assert.True(HdrImage.Parse_Endianness("-1.0"), "Test LE");
        var ex1 = Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.Parse_Endianness("0.0"));
        Assert.Contains("Invalid endianness specification: it cannot be zero", ex1.Message);
        var ex2 = Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.Parse_Endianness("a"));
        Assert.Contains("Invalid endianness specification: expected number", ex2.Message);
    }

    [Fact]
    public void TestPfmParseImgSize()
    {
        Assert.True(HdrImage.Parse_Img_Size("  3   2  ") == (3, 2));
        Assert.True(HdrImage.Parse_Img_Size("  33   22  ") == (33, 22));
        var ex1 = Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.Parse_Img_Size(" -1 3 "));
        Assert.Contains("Only positive numbers are allowed for width and height", ex1.Message);
        var ex2 = Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.Parse_Img_Size(" 1 -3 "));
        Assert.Contains("Only positive numbers are allowed for width and height", ex2.Message);
        var ex3 = Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.Parse_Img_Size("3 2 1"));
        Assert.Contains("Invalid Image Size Specification", ex3.Message);
        var ex4 = Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.Parse_Img_Size(" 3 "));
        Assert.Contains("Invalid Image Size Specification", ex4.Message);
        var ex5 = Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.Parse_Img_Size("a b"));
        Assert.Contains("Only integer numbers are allowed for width and height", ex5.Message);
        var ex6 = Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.Parse_Img_Size("3.1 4"));
        Assert.Contains("Only integer numbers are allowed for width and height", ex6.Message);
        var ex7 = Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.Parse_Img_Size("3 a"));
        Assert.Contains("Only integer numbers are allowed for width and height", ex7.Message);
    }

    [Fact]
    public void TestPfmRead()
    {
        byte[] leReferenceBytes =
        {
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
            0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
            0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
            0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
            0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
            0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
            0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42
        };

        byte[] beReferenceBytes =
        {
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x31, 0x2e, 0x30, 0x0a, 0x42,
            0xc8, 0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00, 0x43,
            0xc8, 0x00, 0x00, 0x43, 0xfa, 0x00, 0x00, 0x44, 0x16, 0x00, 0x00, 0x44,
            0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61, 0x00, 0x00, 0x41,
            0x20, 0x00, 0x00, 0x41, 0xa0, 0x00, 0x00, 0x41, 0xf0, 0x00, 0x00, 0x42,
            0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70, 0x00, 0x00, 0x42,
            0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00
        };

        Stream streamLe = new MemoryStream(leReferenceBytes);
        var img1 = new HdrImage(streamLe);

        Assert.True(img1.Width == 3, "LE - Test image width");
        Assert.True(img1.Height == 2, "LE - Test image height");
        Assert.True(img1.Get_Pixel(0, 0).Is_Close(new Color(1e1f, 2e1f, 3e1f)), "LE - Test Color 1");
        Assert.True(img1.Get_Pixel(1, 0).Is_Close(new Color(4e1f, 5e1f, 6e1f)), "LE - Test Color 2");
        Assert.True(img1.Get_Pixel(2, 0).Is_Close(new Color(7e1f, 8e1f, 9e1f)), "LE - Test Color 3");
        Assert.True(img1.Get_Pixel(0, 1).Is_Close(new Color(1e2f, 2e2f, 3e2f)), "LE - Test Color 4");
        Assert.True(img1.Get_Pixel(1, 1).Is_Close(new Color(4e2f, 5e2f, 6e2f)), "LE - Test Color 5");
        Assert.True(img1.Get_Pixel(2, 1).Is_Close(new Color(7e2f, 8e2f, 9e2f)), "LE - Test Color 6");

        Stream streamBe = new MemoryStream(beReferenceBytes);
        var img2 = new HdrImage(streamBe);

        Assert.True(img2.Width == 3, "BE - Test image width");
        Assert.True(img2.Height == 2, "BE - Test image height");
        Assert.True(img2.Get_Pixel(0, 0).Is_Close(new Color(1e1f, 2e1f, 3e1f)), "BE - Test Color 1");
        Assert.True(img2.Get_Pixel(1, 0).Is_Close(new Color(4e1f, 5e1f, 6e1f)), "BE - Test Color 2");
        Assert.True(img2.Get_Pixel(2, 0).Is_Close(new Color(7e1f, 8e1f, 9e1f)), "BE - Test Color 3");
        Assert.True(img2.Get_Pixel(0, 1).Is_Close(new Color(1e2f, 2e2f, 3e2f)), "BE - Test Color 4");
        Assert.True(img2.Get_Pixel(1, 1).Is_Close(new Color(4e2f, 5e2f, 6e2f)), "BE - Test Color 5");
        Assert.True(img2.Get_Pixel(2, 1).Is_Close(new Color(7e2f, 8e2f, 9e2f)), "BE - Test Color 6");
    }

    [Fact]
    public void TestPfmReadWrongFile()
    {
        var line = Encoding.ASCII.GetBytes($"PF\n3 2\n-1.0\nstop");
        Stream stream = new MemoryStream(line);

        var ex1 = Assert.Throws<InvalidPfmFileFormat>(() => new HdrImage(stream));
        Assert.Contains("Impossible to read binary data from the file", ex1.Message);

        // one missing byte 
        byte[] leReferenceBytes =
        {
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
            0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
            0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
            0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
            0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
            0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
            0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4
        };

        Stream streamLe = new MemoryStream(leReferenceBytes);

        var ex2 = Assert.Throws<InvalidPfmFileFormat>(() => new HdrImage(streamLe));
        Assert.Contains("Impossible to read binary data from the file", ex2.Message);
    }

    [Fact]

    public void TestWriteFloat()
    {
        var test = BitConverter.GetBytes(34.3f);
        MemoryStream strOut = new MemoryStream();
        HdrImage.Write_Float(strOut, 34.3f);
        var arrayOut = strOut.ToArray();
        Assert.True(test.SequenceEqual(arrayOut), "Verify this bool");
    }


   /* [Fact]

       public void TestWritePfm()
    {
        HdrImage img = new(3, 2);

        img.Set_Pixel(1, 0, new Color(4.0e1f, 5.0e1f, 6.0e1f));
        img.Set_Pixel(2, 0, new Color(7.0e1f, 8.0e1f, 9.0e1f));
        img.Set_Pixel(0, 1, new Color(1.0e2f, 2.0e2f, 3.0e2f));
        img.Set_Pixel(1, 1, new Color(4.0e2f, 5.0e2f, 6.0e2f));
        img.Set_Pixel(2, 1, new Color(7.0e2f, 8.0e2f, 9.0e2f));

        byte[] referenceBytes = {0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
            0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
            0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
            0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
            0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
            0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
            0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42};

        MemoryStream streamOut = new MemoryStream();
        img.Write_pfm(streamOut, -1);
        var strOut = streamOut.GetBuffer();
        Assert.True( strOut.Equals(referenceBytes), "Memory test");

       using (Stream streamOut = File.OpenWrite("file.pfm"))
           {
               img.Write_pfm(streamOut, -1);
   
               using (Stream referenceBytes = File.OpenRead("Trace.Tests/reference_le.pfm"))
               {
                   Assert.True(streamOut.Equals(referenceBytes), "Test 1");
               }
           }*/
}    
    
