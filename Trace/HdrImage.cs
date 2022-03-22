using System.Diagnostics;

using System.Text;

namespace Trace;

public class HdrImage
{
    public int height;
    public int width;

    public List<Color> image;

    public HdrImage(int width, int height)
    {
        this.height = height;
        this.width = width;
        image = new List<Color>(this.height * this.width);
        for (int i = 0; i < this.height; i++)
        {
            for (int j = 0; j < this.width; j++)
            {
                image.Add(new Color());
            }
        }
    }

    public bool Valid_Coordinates(int col, int row)
        => col >= 0 && col < width && row < height && row >= 0;

    public int Pixel_Offset(int col, int row)
        => row * width + col;

    public Color Get_Pixel(int col, int row)
    {
        Debug.Assert(Valid_Coordinates(col, row), "Invalid coordinates");
        var pos = Pixel_Offset(col, row);
        return image[pos];

    }

    public void Set_Pixel(int col, int row, Color a)
    {
        Debug.Assert(Valid_Coordinates(col, row), "Invalid coordinates");
        var pos = Pixel_Offset(col, row);
        image[pos] = a;
    }

    //Write_Float(outstream, float) Writes on the output stream the hexadecimal bytes that represent the
    //float given, using endianness of your system.
    private void Write_Float(Stream outputStream, float val)
    {
        var seq = BitConverter.GetBytes(val);
        outputStream.Write(seq, 0, seq.Length);
    }
    
    //Writes an HdrImage on a stream, in PFM format. 
    public void Write_pfm(HdrImage a, Stream outputStream, float endiannessValue)
    {
        var header = Encoding.ASCII.GetBytes($"PF\n{width} {height}\n{endiannessValue}\n");
        outputStream.Write(header);
        //fill matrix image, after writing (byte-coded) the header
        for (int y = a.height - 1; y >= 0; y--)
        {
            for (int x = 0; x <= a.width; x++)
            {
                Color c = a.Get_Pixel(x, y);
                a.Write_Float(outputStream, c.R);
                a.Write_Float(outputStream, c.G);
                a.Write_Float(outputStream, c.B);
            }
        }
    }

    public static string Read_Line(Stream inputStream)
    {
        var result = "";
        while (true)
        {
            var curByte = inputStream.ReadByte();
            if (curByte is -1 or '\n')
            {
                return result;
            }

            result += (char) curByte;
        }
    }

}
