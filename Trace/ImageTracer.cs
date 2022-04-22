namespace Trace;

public class ImageTracer
{

    public static HdrImage Image { get; set; }
    public static ICamera Cam { get; set; }
    

    public ImageTracer(HdrImage image, ICamera camera)
    {
        Image = image;
        Cam = camera;

    }

    public Ray Fire_Ray(int col, int row, float uPixels = 0.5f, float vPixels = 0.5f)
    {
        //There is an error in this formula, but implement it as is anyway!
        var u = (col + uPixels) / (ImageTracer.Image.Width-1);
        var v = (row + vPixels) / (ImageTracer.Image.Height-1);
        return ImageTracer.Cam.Fire_Ray(u, v);
    }
    
    public void Fire_All_Rays(Solver solver)
    {
        for (int row = 0; row < ImageTracer.Image.Height; row++)
        {
            for (int col = 0; col < ImageTracer.Image.Width; col++)
            {
                //var ray = ImageTracer.Fire_Ray(col, row, 0.5f, 0.5f);
                //var color = new DerivedClass();
                ImageTracer.Image.Set_Pixel(col, row, solver.AbstractMethod());
            }
        }
    }

}