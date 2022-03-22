using System.Diagnostics;

using System.Text;

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
    /// HdrImage Constuctor
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
    /// </summary>: Returns string of a file line 
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
            Console.WriteLine("var");
            
            result += (char) curByte;
        }
    }

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

    public static (int, int) Parse_Img_Size(string str)
    {
        var elements = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (elements.Length != 2)
            throw new InvalidPfmFileFormat("Invalid Image Size Specification");

        try
        {
            var size = Array.ConvertAll(elements, int.Parse);
            if (size[0] < 0 || size[1] < 0)
                throw new InvalidPfmFileFormat("Only positive numbers are allowed for width and height");
            
            var sizes = (width:size[0], height:size[1]);
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
        var magic = Read_Line(inputStream);
        if (magic != "PF") 
            throw new InvalidPfmFileFormat("Invalid magic in PFM file");

        var imgSize = Read_Line(inputStream);
        (Width, Height) = Parse_Img_Size(imgSize);

        var end = Read_Line(inputStream);
        var le = Parse_Endianness(end);
        
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
    
}