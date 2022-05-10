using Trace;

namespace _5K_JFS;

public static class Parameters
{
    public static string InputPfmFileName = "";
    public static float A = 0.2f;
    public static float Gamma = 1.0f;
    public static string OutputFileName = "";
    public static string Format = "";

    public static void Parse_Command_Line(string[] args)
    {
        if (args.Length != 4)
            throw new RuntimeException("Usage: dotnet run INPUT_PFM_FILE FACTOR_A GAMMA OUTPUT_FILE");
        InputPfmFileName = args[0];
        try
        {
            A = Convert.ToSingle(args[1]);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {args[1]}, it must be a floating-point number");
        }
        try
        {
            Gamma = Convert.ToSingle(args[2]);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {args[2]}, it must be a floating-point number");
        }

        OutputFileName = args[3];
        Format = Path.GetExtension(OutputFileName);
    }

    // DEMO
    public static int Width;
    public static int Height ;
    public static float AngleDeg;
    public static bool Orthogonal;
    public static void Parse_Command_Line_Demo(string? width, string? height, string? angle)
    {
        var w = width ?? "480";
        var h = height ?? "480";
        var a = angle ?? "0";

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
        /*if (args.Length < 2) 
            throw new RuntimeException("Usage: dotnet run WIDTH HEIGHT ANGLE_DEG ORTHOGONAL \n" +
                                       "If ANGLE_DEG and ORTHOGONAL are not specified, default values are used \n" +
                                       "Default values: \n" +
                                       "ANGLE_DEG = 0.0 \n" +
                                       "ORTHOGONAL = false \n");
        try
        {
            Width = Convert.ToInt32(args[0]);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {args[0]}, it must be an integer");
        }
        try
        {
            Height = Convert.ToInt32(args[1]);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {args[1]}, it must be an integer");
        }

        if (args.Length > 2)
        {
            try
            {
                AngleDeg = Convert.ToSingle(args[2]);
            }
            catch
            {
                throw new RuntimeException($"Invalid factor {args[2]}, it must be a floating-point number");
            }
        }

        if (args.Length == 4)
        {
            if (args[3].ToUpper() == "ORTHOGONAL") Orthogonal = true;
            else throw new RuntimeException($"Invalid factor {args[3]}, it must be the word orthogonal se si vuole usare proiezione ortogonale");
        } */
    }
}