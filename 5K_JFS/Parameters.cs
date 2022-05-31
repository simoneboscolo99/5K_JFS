using Trace;

namespace _5K_JFS;

/// <summary>
/// A class that parses the command line parameters
/// </summary>
public class Parameters
{
    /// <summary>
    /// Path of the input pfm file.
    /// </summary>
    public static string InputPfmFileName = "";
    
    /// <summary>
    /// Multiplicative factor.
    /// </summary>
    public static float Factor;
    
    /// <summary>
    /// Exponent for gamma-correction.
    /// </summary>
    public static float Gamma;
    
    /// <summary>
    /// Path of the output ldr file.
    /// </summary>
    public static string OutputFileName = "";
    
    /// <summary>
    /// Format of the output ldr file.
    /// </summary>
    public static string Format = "";
    
    // =======================================================================================
    // === CONVERT === CONVERT === CONVERT === CONVERT === CONVERT === CONVERT === CONVERT === 
    // =======================================================================================

    /// <summary>
    /// Parses command line parameters in convert mode.
    /// </summary>
    /// <param name="inputFilename"> a string that contains the path of the input pfm file. If  <paramref name="inputFilename"/> is null, the default path 'Input_Pfm/memorial.pfm' is used. </param>
    /// <param name="outputFilename"> a string that contains the path of the output ldr file. If <paramref name="outputFilename"/> is null, the default path 'Images/output.png' is used. </param>
    /// <param name="gamma"> a string that contains the number corresponding to the exponent for gamma-correction. If <paramref name="gamma"/> is null, the default value of 1 is used. </param>
    /// <param name="factor"> a string that contains the number corresponding to the multiplicative factor. If <paramref name="factor"/> is null, the default value of 0,2 is used. </param>
    /// <exception cref="RuntimeException"> invalid format of the parameters. </exception>
    public static void Parse_Command_Line_Convert(string? inputFilename = null, string? outputFilename = null, string? gamma = null,
        string? factor = null)
    {
        var i = inputFilename ?? "Input_Pfm/memorial.pfm";
        var o = outputFilename ?? "Images/output.png";
        var g = gamma ?? "1";
        var f = factor ?? "0,2";

        InputPfmFileName = Convert.ToString(i);
        OutputFileName = Convert.ToString(o);
        
        try
        {
            Gamma = Convert.ToSingle(g);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {g}, it must be a floating-point number");
        }

        try
        {
            Factor = Convert.ToSingle(f);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {f}, it must be a floating-point number");
        }

        Format = Path.GetExtension(OutputFileName);
    }

    // ====================================================================================
    // === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO ===
    // ====================================================================================
    
    /// <summary>
    /// Width of the image.
    /// </summary>
    public static int Width;
    
    /// <summary>
    /// Height of the image.
    /// </summary>
    public static int Height;
    
    /// <summary>
    /// Angle of view.
    /// </summary>
    public static float AngleDeg;
    
    /// <summary>
    /// Use an orthogonal camera instead of a perspective camera.
    /// </summary>
    public static bool Orthogonal;

    /// <summary>
    /// 
    /// </summary>
    public static int SamplesPerSide;

    /// <summary>
    /// Parses command line parameters in demo mode.
    /// </summary>
    /// <param name="width"> a string that contains the number corresponding to the width of the image. If <paramref name="width"/> is null, the default value of 480 is used. </param>
    /// <param name="height"> a string that contains the number corresponding to the height of the image. If <paramref name="height"/> is null, the default value of 480 is used. </param>
    /// <param name="angle"> a string that contains the number corresponding to the angle of view. If <paramref name="angle"/> is null, the default value of 0 is used. </param>
    /// <param name="gamma"> a string that contains the number corresponding to the exponent for gamma-correction. If <paramref name="gamma"/> is null, the default value of 1 is used. </param>
    /// <param name="factor"> a string that contains the number corresponding to the multiplicative factor. If <paramref name="factor"/> is null, the default value of 0,2 is used. </param>
    /// <param name="outputFilename"> a string that contains the path of the output ldr file. If <paramref name="outputFilename"/> is null, the default path 'Demo.png' is used. </param>
    /// <param name="samplesPerPixel"></param>
    /// <exception cref="RuntimeException"> invalid format of the parameters. </exception>
    public static void Parse_Command_Line_Demo(string? width = null, string? height = null, string? angle = null, string? gamma = null,
        string? factor = null, string? outputFilename = null, string? samplesPerPixel = null)
    {
        var w = width ?? "480";
        var h = height ?? "480";
        var a = angle ?? "0";
        var g = gamma ?? "1";
        var f = factor ?? "0,2";
        var output = outputFilename ?? "Demo.png";
        var ssp = samplesPerPixel ?? "0";

        try
        {
            Width = Convert.ToInt32(w);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {w}, it must be an integer.");
        }

        try
        {
            Height = Convert.ToInt32(h);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {h}, it must be an integer.");
        }

        try
        {
            AngleDeg = Convert.ToSingle(a);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {a}, it must be a floating-point number.");
        }

        try
        {
            Gamma = Convert.ToSingle(g);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {g}, it must be a floating-point number.");
        }

        try
        {
            Factor = Convert.ToSingle(f);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {f}, it must be a floating-point number.");
        }
        
        try
        {
            var samples = Convert.ToInt32(ssp);
            SamplesPerSide = (int) Math.Sqrt(samples);
            if (SamplesPerSide * SamplesPerSide != samples) throw new RankException($"Error, the number of samples per pixel ({samplesPerPixel}) must be a perfect square");
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {samplesPerPixel}, it must be an integer.");
        }
        
        OutputFileName = Convert.ToString(output);
        Format = Path.GetExtension(OutputFileName);
    }
}