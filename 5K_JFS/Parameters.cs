using Trace;
using System.Globalization;
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
            throw new RuntimeException($"Invalid factor {samplesPerPixel}, it must be a perfect square.");
        }
        
        OutputFileName = Convert.ToString(output);
        Format = Path.GetExtension(OutputFileName);
    }
    
    // ============================================================================
    // === RENDER === RENDER === RENDER === RENDER === RENDER === RENDER === RENDER
    // ============================================================================
    
    /// <summary>
    /// Path of the output hdr file
    /// </summary>
    public static string PfmOutputFilename = "";

    /// <summary>
    /// Path of the input scene file
    /// </summary>
    public static string InputSceneName = "";
    
    /// <summary>
    /// Number of rays departing from each surface point
    /// </summary>
    public static int NumOfRays;
    
    /// <summary>
    /// Maximum allowed ray depth 
    /// </summary>
    public static int MaxDepth;
    
    /// <summary>
    /// Initial seed for the random number generator (positive number)
    /// </summary>
    public static ulong InitState;
    
    /// <summary>
    /// Identifier of the sequence produced by the random number generator (positive number)
    /// </summary>
    public static ulong InitSeq;
    
    /// <summary>
    /// Declare a variable. The syntax is «--declare-float=VAR:VALUE»
    /// </summary>
    public static IDictionary<string, float> DeclareFloat = new Dictionary<string, float>();

    /// <summary>
    /// Parses command line parameters in demo mode.
    /// </summary>
    /// <param name="width"> a string that contains the number corresponding to the width of the image. If <paramref name="width"/> is null, the default value of 480 is used. </param>
    /// <param name="height"> a string that contains the number corresponding to the height of the image. If <paramref name="height"/> is null, the default value of 480 is used. </param>
    /// <param name="angle"> a string that contains the number corresponding to the angle of view. If <paramref name="angle"/> is null, the default value of 0 is used. </param>
    /// <param name="gamma"> a string that contains the number corresponding to the exponent for gamma-correction. If <paramref name="gamma"/> is null, the default value of 1 is used. </param>
    /// <param name="factor"> a string that contains the number corresponding to the multiplicative factor. If <paramref name="factor"/> is null, the default value of 0,2 is used. </param>
    /// <param name="outputFilename"> a string that contains the path of the output ldr file. If <paramref name="outputFilename"/> is null, the default path 'output.png' is used. </param>
    /// <param name="pfmoutputFilename"> a string that contains the path of the output hdr file. If <paramref name="pfmoutputFilename"/> is null, the default path 'output.pfm' is used. </param>
    /// <param name="numOfRays"> Number of rays departing from each surface point. If <paramref name="numOfRays"/> is null, the default value of 10 is used. </param>
    /// <param name="maxDepth"> Maximum allowed ray depth. If <paramref name="maxDepth"/> is null, the default value of 3 is used. </param>
    /// <param name="samplesPerPixel">Number of samples per pixel (must be a perfect square, e.g., 16) If <paramref name="samplesPerPixel"/> is null, the default value of 0 is used. </param>
    /// <param name="initState"> Initial seed for the random number generator. If <paramref name="initState"/> is null, the default value of 45 is used. </param>
    /// <param name="initSeq"> Identifier of the sequence produced by the random number generator. If <paramref name="initSeq"/> is null, the default value of 54 is used. </param>
    /// <param name="declareFloat"> Declare a variable. The syntax is «--declare-float=VAR:VALUE».  </param>
    /// <param name="input"> Name of the input file with the scene.</param>
    /// <exception cref="RuntimeException"> invalid format of the parameters. </exception>
    public static void Parse_Command_Line_Render(string? width = null, string? height = null, string? angle = null, string? gamma = null,
        string? factor = null, string? outputFilename = null, string? samplesPerPixel = null, string? initSeq = null, string? initState = null, string? maxDepth = null, string? pfmoutputFilename = null, 
        List<string>? declareFloat = null, string? numOfRays =null, string? input = null)
    {
        var w = width ?? "480";
        var h = height ?? "480";
        var a = angle ?? "0";
        var g = gamma ?? "1";
        var f = factor ?? "0.2";
        var output = outputFilename ?? "output.png";
        var pfm = pfmoutputFilename ?? "output.pfm";
        var ssp = samplesPerPixel ?? "0";
        var inSeq = initSeq ?? "54";
        var inSt = initState ?? "42";
        var max = maxDepth ?? "3";
        var num = numOfRays ?? "10";
        var dec = declareFloat ?? new List<string>();
        var inp = input ?? "Examples/demo.txt";

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
            AngleDeg = float.Parse(a , CultureInfo.InvariantCulture);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {a}, it must be a floating-point number.");
        }

        try
        {
            Gamma = float.Parse(g , CultureInfo.InvariantCulture);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {g}, it must be a floating-point number.");
        }

        try
        {
            Factor = float.Parse(f , CultureInfo.InvariantCulture);
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
        
        try
        {
            MaxDepth = Convert.ToInt32(max);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {max}, it must be an integer.");
        }
        
        try
        {
            NumOfRays = Convert.ToInt32(num);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {num}, it must be an integer.");
        }
        
        try
        {
            InitState = Convert.ToUInt64(inSt);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {inSt}, it must be an integer.");
        }
        
        try
        {
            InitSeq = Convert.ToUInt64(inSeq);
        }
        catch
        {
            throw new RuntimeException($"Invalid factor {inSeq}, it must be an integer.");
        }

        DeclareFloat = Build_Variable_Table(dec);
        
        OutputFileName = Convert.ToString(output);
        PfmOutputFilename = Convert.ToString(pfm);
        InputSceneName = Convert.ToString(inp);

        Format = Path.GetExtension(OutputFileName);
    }

    /// <summary>
    /// Parse the list of `-d` switches and return a dictionary associating variable names with their values.
    /// </summary>
    /// <param name="declare"></param>
    /// <returns></returns>
    public static IDictionary<string, float> Build_Variable_Table(List<string> declare)
    {
        var variables = new Dictionary<string, float>();
        foreach (var declaration in declare)
        {
            var parts = declaration.Split(":");
            if (parts.Length != 2)
            {
                throw new RuntimeException($"error, the definition «{declaration}» does not follow the pattern NAME:VALUE");
            }

            var name = parts[0];
            var stringValue = parts[1];
            float value;
            try
            {
                value = Convert.ToSingle(stringValue);
            }
            catch
            {
                throw new RuntimeException($"invalid floating-point value «{stringValue}» in definition «{declaration}»");
            }

            variables[name] = value;
        }

        return variables;
    }
}