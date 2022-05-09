namespace Trace;

public class ImageTracer
{

    public HdrImage Image { get; set; }
    public ICamera Cam { get; set; }
    

    public ImageTracer(HdrImage image, ICamera camera)
    {
        Image = image;
        Cam = camera;
    }

    public Ray Fire_Ray(int col, int row, float uPixels = 0.5f, float vPixels = 0.5f)
    {
        //There is an error in this formula, but implement it as is anyway!
        var u = (col + uPixels)/Image.Width;
        var v = 1.0f - (row + vPixels)/Image.Height;
        return Cam.Fire_Ray(u, v);
    }
    
    /// <summary>
    /// Shoot several light rays crossing each of the pixels in the image.
    /// For each pixel in the :class:`.HdrImage` object fire one ray, and pass it to the function `func`, which
    /// must accept a :class:`.Ray` as its only parameter and must return a :class:`.Color` instance telling the
    /// color to assign to that pixel in the image.
    /// </summary>
    /// <param name="solver"></param>
    public void Fire_All_Rays(Solver solver)
    {
        for (int row = 0; row < Image.Height; row++)
        {
            for (int col = 0; col < Image.Width; col++)
            {
                var ray = Fire_Ray(col, row, 0.5f, 0.5f);
                //var color = new DerivedClass();
                Image.Set_Pixel(col, row, solver.Tracing(ray));
            }
        }
    }

}