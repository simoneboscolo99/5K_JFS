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
    
    public Ray Fire_All_Rays(int col, int row, float uPixels = 0.5f, float vPixels = 0.5f)
    {
        //There is an error in this formula, but implement it as is anyway!
        var u = (col + uPixels) / (ImageTracer.Image.Width-1);
        var v = (row + vPixels) / (ImageTracer.Image.Height-1);
        return ImageTracer.Cam.Fire_Ray(u, v);
    }

}