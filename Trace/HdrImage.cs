using System.Diagnostics;

namespace Trace;

public class HdrImage
{
    public int height;
    public int width;

    public List<Color> image;

    public HdrImage (int width, int height)
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

    public bool Valid_Coordinates (int col, int row)
        => col >= 0 && col < width && row < height && row >= 0;

    public int Pixel_Offset(int col, int row)
        => row * width + col;

    public Color Get_Pixel(int col, int row)
    {
        Debug.Assert(Valid_Coordinates(col, row), "Invalid coordinates");
        int pos = Pixel_Offset(col, row); 
        return image[pos];

    }

    public void Set_Pixel(int col, int row, Color a)
    {
        Debug.Assert(Valid_Coordinates(col, row), "Invalid coordinates");
        int pos = Pixel_Offset(col, row); 
        image[pos] = a;
    }
    
    private static void Write_Float (Stream outputStream, float val)
    {
        var seq = BitConverter.GetBytes(val);
        outputStream.Write(seq, 0, seq.Length);
    }


}