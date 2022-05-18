using Trace;

namespace _5K_JFS;

/// <summary>
/// 
/// </summary>
public class Parameters
{
    /// <summary>
    /// Path of the input pfm file
    /// </summary>
    public static string InputPfmFileName = "";
    /// <summary>
    /// Multiplicative factor
    /// </summary>
    public static float Factor;
    /// <summary>
    /// Exponent for gamma-correction
    /// </summary>
    public static float Gamma;
    /// <summary>
    /// Path of the output ldr file
    /// </summary>
    public static string OutputFileName = "";
    /// <summary>
    /// Format of the output ldr file
    /// </summary>
    public static string Format = "";

    /// <summary>
    /// Control of parameters given by user to convert 
    /// </summary>
    /// <param name="inputFilename"></param>
    /// <param name="outputFilename"></param>
    /// <param name="gamma"></param>
    /// <param name="factor"></param>
    /// <exception cref="RuntimeException"></exception>
    public static void Parse_Command_Line_Convert(string? inputFilename, string? outputFilename, string? gamma,
        string? factor)
    {
        var i = inputFilename ?? "Input_Pfm/memorial.pfm";
        var o = outputFilename ?? "Images/output.png";
        var g = gamma ?? "1";
        var f = factor ?? "0,2";

        try
        {
            InputPfmFileName = Convert.ToString(i);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {i}, it must be a string");
        }

        try
        {
            OutputFileName = Convert.ToString(o);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {o}, it must be a string");
        }

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

    // DEMO
    
    /// <summary>
    /// Width of the image
    /// </summary>
    public static int Width;
    /// <summary>
    /// Height of the image
    /// </summary>
    public static int Height;
    /// <summary>
    /// Angle of view
    /// </summary>
    public static float AngleDeg;
    /// <summary>
    /// Use an orthogonal camera instead of a perspective camera
    /// </summary>
    public static bool Orthogonal;

    /// <summary>
    /// Control over the Demo's parameters
    /// </summary>
    /// <param name="width"> width of the image </param>
    /// <param name="height"> height of the image </param>
    /// <param name="angle"> angle of view </param>
    /// <param name="gamma"> exponent for gamma-correction </param>
    /// <param name="factor"> multiplicative factor </param>
    /// <param name="outputFilename"> path of the output ldr file </param>
    /// <exception cref="RuntimeException"></exception>
    public static void Parse_Command_Line_Demo(string? width, string? height, string? angle, string? gamma,
        string? factor, string? outputFilename)
    {
        var w = width ?? "480";
        var h = height ?? "480";
        var a = angle ?? "0";
        var g = gamma ?? "1";
        var f = factor ?? "0,2";
        var output = outputFilename ?? "Demo.png";

        try
        {
            Width = Convert.ToInt32(w);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {w}, it must be an integer");
        }

        try
        {
            Height = Convert.ToInt32(h);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {h}, it must be an integer");
        }

        try
        {
            AngleDeg = Convert.ToSingle(a);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {a}, it must be a floating-point number");
        }

        try
        {
            Gamma = Convert.ToSingle(g);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {g}, it must be a float");
        }

        try
        {
            Factor = Convert.ToSingle(f);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {f}, it must be a float");
        }

        try
        {
            OutputFileName = Convert.ToString(output);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {output}, it must be a string");
        }

        Format = Path.GetExtension(OutputFileName);
    }
}
