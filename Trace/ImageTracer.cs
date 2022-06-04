using ShellProgressBar;
//using System.Threading;

namespace Trace;

/// <summary>
/// Trace an image by shooting light rays through each of its pixels.
/// </summary>
public class ImageTracer
{
    /// <summary>
    /// 
    /// </summary>
    public HdrImage Image { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public ICamera Cam { get; }

    /// <summary>
    /// 
    /// </summary>
    public int SamplesPerSide;

    /// <summary>
    /// 
    /// </summary>
    public Pcg Pcg;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    /// <param name="camera"></param>
    /// <param name="samplesPerSide"></param>
    /// <param name="pcg"></param>
    public ImageTracer(HdrImage image, ICamera camera, int samplesPerSide = 0, Pcg? pcg = null)
    {
        Image = image;
        Cam = camera;
        SamplesPerSide = samplesPerSide;
        Pcg = pcg ?? new Pcg();
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
        var maxTicks = Image.Height;
        var options = new ProgressBarOptions {
            //ProgressCharacter = '-',
            //ProgressBarOnBottom = true,
            ForegroundColor = ConsoleColor.Yellow,
            ForegroundColorDone = ConsoleColor.White,
            BackgroundColor = ConsoleColor.DarkGray,
            BackgroundCharacter = '\u2593'
        };
        using (var pbar = new ProgressBar(maxTicks, "Starting", options))
        {
            DateTime mDataOraStart = DateTime.Now;
            for (int row = 0; row < Image.Height; row++)
            {
                for (int col = 0; col < Image.Width; col++)
                {
                    //var ray = Fire_Ray(col, row, 0.5f, 0.5f);
                    //var color = new DerivedClass();
                    //Image.Set_Pixel(col, row, solver.Tracing(ray));

                    var cumColor = new Color();
                    if (SamplesPerSide > 0)
                    {
                        // Run stratified sampling over the pixel's surface
                        for (int interpixelrow = 0; interpixelrow < SamplesPerSide; interpixelrow++)
                        {
                            for (int interpixelcol = 0; interpixelcol < SamplesPerSide; interpixelcol++)
                            {
                                var uPixel = (interpixelcol + Pcg.Random_Float()) / SamplesPerSide;
                                var vPixel = (interpixelrow + Pcg.Random_Float()) / SamplesPerSide;
                                var ray = Fire_Ray(col, row, uPixel, vPixel);
                                cumColor += solver.Tracing(ray);
                            }
                        }

                        Image.Set_Pixel(col, row, cumColor * (1.0f / SamplesPerSide * SamplesPerSide));
                    }

                    else
                    {
                        var ray = Fire_Ray(col, row);
                        Image.Set_Pixel(col, row, solver.Tracing(ray));
                    }
                }
                pbar.Tick(row == Image.Height - 1 ? "Rendering completed" : "Rendering...");
            }
        }
    }
    
    /*private static string CalcolaEta(DateTime dataOraInizio, int tickTot, int tickAttuale)
    {
        if (tickAttuale <= 0)
            return "-- %";

        TimeSpan tsDiff=DateTime.Now.Subtract(dataOraInizio);
        int secAllaFine = (int)((tsDiff.TotalSeconds / tickAttuale) * tickTot);
        TimeSpan t = TimeSpan.FromSeconds(secAllaFine);

        return string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
            t.Hours,
            t.Minutes,
            t.Seconds);
    }*/
}