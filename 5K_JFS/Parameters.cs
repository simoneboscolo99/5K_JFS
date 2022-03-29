using Trace;

namespace _5K_JFS;

public class Parameters
{
    public static string InputPfmFileName = "";
    public static float a = 0.2f;
    public static float Gamma = 1.0f;
    public static string OutputPngFileName = "";
    
    public static void Parse_Command_Line(string[] args)
    {
        if (args.Length != 4)
            throw new RuntimeException("Usage: dotnet run INPUT_PFM_FILE FACTOR_A GAMMA OUTPUT_FILE");
        InputPfmFileName = args[0];
        try
        {
            a = Convert.ToSingle(args[1]);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {args[1]}, it must be a floating-point number ");
        }
        try
        {
            Gamma = Convert.ToSingle(args[2]);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {args[2]}, it must be a floating-point number ");
        }

        OutputPngFileName = args[3];
    }
}