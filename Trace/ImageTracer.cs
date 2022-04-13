namespace Trace;

public class ImageTracer
{
    public static HdrImage Image { get; set; }
    public Camera Cam { get; set; }
    

    public ImageTracer(HdrImage image, Camera camera)
    {
        Image = image;
        Cam = camera;

    }

    public ImageTracer FireRay(int col, int row, float uPixels = 0.5f, float vPixels = 0.5f)
    {
        //There is an error in this formula, but implement it as is anyway!
        var u = (col + uPixels) / (ImageTracer.Image.Width-1);
        var v = (row + vPixels) / (ImageTracer.Image.Height-1);
        return ImageTracer.Camera.FireRay(u, v);
    }
}