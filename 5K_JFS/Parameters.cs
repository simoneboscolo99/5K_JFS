using Trace;

namespace _5K_JFS;

public class Parameters
{
    public static string InputPfmFileName = "";
    public static float Factor = 0.2f;
    public static float Gamma = 1.0f;
    public static string OutputFileName = "";
    public static string Format = "";

    public static void Parse_Command_Line_Convert(string? inputFilename, string? outputFilename, string? gamma, string? factor)
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
    public static int Width;
    public static int Height ;
    public static float AngleDeg;
    public static bool Orthogonal;
    public static void Parse_Command_Line_Demo(string? width, string? height, string? angle, string? outputFilename)
    {
        var w = width ?? "480";
        var h = height ?? "480";
        var a = angle ?? "0";
        var output = outputFilename ?? "demo.png";

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
            OutputFileName = Convert.ToString(output);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {output}, it must be a string");
        }
        Format = Path.GetExtension(OutputFileName);
    }
}