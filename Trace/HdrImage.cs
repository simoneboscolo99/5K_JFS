using System.Diagnostics;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Trace;

public class HdrImage
{
    //CONSTRUCTORS-------------------------------------------------------------------------------
    public int Height { get; set; }
    public int Width { get; set; }
    public List<Color> Image { get; set; }

    /// <summary>
    /// HdrImage constructor
    /// </summary> Matrix of elements Color
    /// <param name="width"> image width </param>
    /// <param name="height"> image height </param>
    public HdrImage(int width, int height)
    {
        Height = height;
        Width = width;
        Image = new List<Color>(Height * Width);
        for (int i = 0; i < Height * Width; i++) Image.Add(new Color());
    }

    /// <summary>
    /// HdrImage Constructor
    /// </summary>: Creates an image reading dates from a stream
    /// <param name="inputStream"></param>
    public HdrImage(Stream inputStream)
    {
        Read_Pfm(inputStream);
    }


    /// <summary>
    /// HdrImage Constructor
    /// </summary>: Creates an HdrImage from an input file-name 
    /// <param name="fileName"></param>

    public HdrImage(string fileName)
    {
        using (Stream fileStream = File.OpenRead(fileName))
        {
            Read_Pfm(fileStream);
        }
    }

    //END OF CONSTRUCTORS------------------------------------------------------------------------

    /// <summary>
    /// Valid_Coordinates
    /// </summary>: returns true if (x,y) is valid, else false
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool Valid_Coordinates(int col, int row)
        => col >= 0 && col < Width && row < Height && row >= 0;

    /// <summary>
    /// Pixel_Offset
    /// </summary>: Returns ordinal position of pixel in (x,y)
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public int Pixel_Offset(int col, int row)
        => row * Width + col;

    /// <summary>
    /// Get_Pixel
    /// </summary>: Returns the Color of indexes (row,col)
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public Color Get_Pixel(int col, int row)
    {
        Debug.Assert(Valid_Coordinates(col, row), "Invalid coordinates");
        var pos = Pixel_Offset(col, row);
        return Image[pos];
    }

    /// <summary>
    /// Set_Pixel
    /// </summary>: Assigns to a color of indexes x,y the variable Color 'a' given 
    /// <param name="col"> int </param>
    /// <param name="row"> int </param>
    /// <param name="a"> Color type </param>
    public void Set_Pixel(int col, int row, Color a)
    {
        Debug.Assert(Valid_Coordinates(col, row), "Invalid coordinates");
        var pos = Pixel_Offset(col, row);
        Image[pos] = a;
    }

    /// <summary>
    /// Write_Float
    /// </summary>: Writes on the output stream bytes that represent the float given, using hexadecimal and endianness of your system.
    /// <param name="outputStream"></param>
    /// <param name="val"></param>
    public static void Write_Float(Stream outputStream, float val)
    {
        var seq = BitConverter.GetBytes(val);
        outputStream.Write(seq, 0, seq.Length);
    }

    /// <summary>
    /// Write_pfm
    /// </summary>: Writes an HdrImage on a stream, in PFM format.
    /// <param name="outputStream"></param>
    public void Write_pfm(Stream outputStream)
    {
        var end = BitConverter.IsLittleEndian ? "-1.0" : "1.0";
        var header = Encoding.ASCII.GetBytes($"PF\n{Width} {Height}\n{end}\n");
        outputStream.Write(header);

        for (int y = Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                Color c = Get_Pixel(x, y);
                Write_Float(outputStream, c.R);
                Write_Float(outputStream, c.G);
                Write_Float(outputStream, c.B);
            }
        }
    }

    /// <summary>
    /// Read_Line
    /// </summary>: Returns a file line in its string representation 
    /// <param name="inputStream"></param>
    /// <returns></returns>
    public static string Read_Line(Stream inputStream)
    {
        var result = "";
        while (true)
        {
            var curByte = inputStream.ReadByte();
            if (curByte is -1 or '\n')
                return result;

            result += (char) curByte;
        }
    }

    /// <summary>
    /// Read_Float
    /// </summary>: Reads 4 bytes of a file and converts them into a single precision floating variable
    /// <param name="inputStream"></param>
    /// <param name="le"> "little endian?" </param>
    /// <returns></returns>
    /// <exception cref="InvalidPfmFileFormat"></exception>
    private static float Read_Float(Stream inputStream, bool le)
    {
        var reader = new BinaryReader(inputStream);
        var floatBytes = reader.ReadBytes(4);
        if (le != BitConverter.IsLittleEndian) Array.Reverse(floatBytes);
        try
        {
            var val = BitConverter.ToSingle(floatBytes);
            return val;
        }
        catch
        {
            throw new InvalidPfmFileFormat("Impossible to read binary data from the file");
        }
    }

    /// <summary>
    /// Parse_Endianness
    /// </summary>: Converts a string to its "IsLittleEndian?"-value
    /// <param name="str"></param>
    /// <returns></returns>
    /// <exception cref="InvalidPfmFileFormat"></exception>
    public static bool Parse_Endianness(string str)
    {
        try
        {
            var value = float.Parse(str);
            return value switch
            {
                < 0 => true,
                > 0 => false,
                _ => throw new InvalidPfmFileFormat("Invalid endianness specification: it cannot be zero")
            };
        }
        catch (FormatException)
        {
            throw new InvalidPfmFileFormat("Invalid endianness specification: expected number");
        }
    }


    /// <summary>
    /// Parse_Img_Size
    /// </summary>: returns image size (width,height)
    /// <param name="str"></param>
    /// <returns></returns>
    /// <exception cref="InvalidPfmFileFormat"></exception>
    public static (int, int) Parse_Img_Size(string str) //returns a tuple made of 2 ints
    {
        //elements is an array containing decomposed line
        var elements = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (elements.Length != 2)
            throw new InvalidPfmFileFormat("Invalid Image Size Specification");

        try
        {
            var size = Array.ConvertAll(elements, int.Parse);
            if (size[0] < 0 || size[1] < 0)
                throw new InvalidPfmFileFormat("Only positive numbers are allowed for width and height");

            var sizes = (width: size[0], height: size[1]);
            return sizes;
        }
        catch (FormatException)
        {
            throw new InvalidPfmFileFormat("Only integer numbers are allowed for width and height");
        }
        catch (OverflowException)
        {
            throw new InvalidPfmFileFormat("Only integer numbers are allowed for width and height");
        }
        catch (ArgumentNullException)
        {
            throw new InvalidPfmFileFormat("Only integer numbers are allowed for width and height");
        }
    }

    private void Read_Pfm(Stream inputStream)
    {
        //first row
        var magic = Read_Line(inputStream);
        if (magic != "PF")
            throw new InvalidPfmFileFormat("Invalid magic in PFM file");

        //second row
        var imgSize = Read_Line(inputStream);
        (Width, Height) = Parse_Img_Size(imgSize);

        //third row
        var endianness = Read_Line(inputStream);
        var le = Parse_Endianness(endianness);

        Image = new List<Color>(Height * Width);
        for (int i = 0; i < Height * Width; i++) Image.Add(new Color());

        // bottom up, left to right
        for (int row = Height - 1; row >= 0; row--)
        {
            for (int col = 0; col < Width; col++)
            {
                var r = Read_Float(inputStream, le);
                var g = Read_Float(inputStream, le);
                var b = Read_Float(inputStream, le);
                Set_Pixel(col, row, new Color(r, g, b));
            }
        }
    }

    public float Luminosity_Ave(float delta = 1e-10f)
    {
        var sum = 0.0d;
        foreach (var color in Image)
            sum += Math.Log10(delta + color.Luminosity());
        return (float) Math.Pow(10, sum / Image.Count);
    }

    public void Luminosity_Norm(float a, float? luminosity = null)
    {
        //if luminosity == Null => lum = Lum_Ave, else lum = luminosity
        var lum = luminosity ?? Luminosity_Ave();
        for (int i = 0; i < Image.Count; i++)
            Image[i] = (a / lum) * Image[i];
    }

    public void Clamp_Image()
    {
        for (int i = 0; i < Image.Count; i++)
            Image[i] = new Color(Functions.Clamp(Image[i].R), Functions.Clamp(Image[i].G), Functions.Clamp(Image[i].B));
    }

    
    /// <summary>
    /// 
    /// </summary>: Write an image with a desired format
    /// <param name="outputStream"></param>
    /// <param name="format"></param>
    /// <param name="gamma"></param>
    public void Write_Ldr_Image(Stream outputStream, string? format = null, float? gamma = null)
    {
        var g = gamma ?? 1.0f;
        var f = format ?? "Png";
        var bitmap = new Image<Rgb24>(Configuration.Default, Width, Height);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var aColor = Get_Pixel(x, y);
                bitmap[x, y] = new Rgb24((byte) (int)(255 * Math.Pow(aColor.R,1/g)),(byte) (int)(255 * Math.Pow(aColor.G,1/g)), (byte) (int)(255 *Math.Pow(aColor.B,1/g)));
            }
        }
        
        var extension = f.ToUpper();
        switch (extension)
        {
            case "PNG":
            { 
                using Stream fileStream = File.OpenWrite("output.png");
                bitmap.Save(fileStream, new PngEncoder());
                break;
            }
            case "JPEG":
            {
                using Stream fileStream = File.OpenWrite("output.jpg");
                bitmap.Save(fileStream, new JpegEncoder());
                break;
            }
            case "BMP":
            {
                using Stream fileStream = File.OpenWrite("output.bmp");
                bitmap.Save(fileStream, new BmpEncoder());
                break;
            }
            case "GIF":
            {
                using Stream fileStream = File.OpenWrite("output.gif");
                bitmap.Save(fileStream, new GifEncoder());
                break;
            }
            case "PBM":
            {
                using Stream fileStream = File.OpenWrite("output.pbm");
                bitmap.Save(fileStream, new PbmEncoder());
                break;
            }
        }
    }
}