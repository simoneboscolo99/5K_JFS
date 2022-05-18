// See https://aka.ms/new-console-template for more information
//not yet :) .... This program is an "image order" RayTracer

using System.Drawing;
using _5K_JFS;
using Microsoft.Extensions.CommandLineUtils;
using Trace;
using Color = Trace.Color;

// References for CLI (Command Line Interface)
// https://github.com/anthonyreilly/ConsoleArgs/blob/master/Program.cs
// https://www.areilly.com/2017/04/21/command-line-argument-parsing-in-net-core-with-microsoft-extensions-commandlineutils/

// Instantiate the command line app
var app = new CommandLineApplication
{
    Name = "dotnet run",
    Description = ".NET Core console app with argument parsing.",
    ExtendedHelpText = "\nThis is the extended help text for the application.\n"
};

// Set the arguments to display the description and help text
app.HelpOption("-?|-h|--help");

// ===========================================================================
// === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO ===
// ===========================================================================

// This is a command with no arguments - it just does default action.
app.Command("demo", (command) =>
{
    // This is a command that has it's own options
    // description and help text of the command
    command.Description = "This is the description for demo.";
    command.ExtendedHelpText = "\nThis is the extended help text for demo.\n";

    // OPTIONS
    // There are 3 possible option types:
    // NoValue
    // SingleValue
    // MultipleValue (not use in our case)
    
    // MultipleValue options can be supplied as one or multiple arguments
    // e.g. -m valueOne -m valueTwo -m valueThree
    
    // SingleValue: A basic Option with a single value
    // e.g. -s sampleValue
    // Option: it starts with a pipe-delimited list of option flags/names to use
    // Optionally, It is then followed by a space and a short description of the value to specify.
    // e.g. here we could also just use "-o|--option"
    var width = command.Option("--width <INTEGER>", "Width of the image", CommandOptionType.SingleValue);
    var height = command.Option("--height <INTEGER>", "Height of the image", CommandOptionType.SingleValue);
    var angleDeg = command.Option("-a|--angle-deg <FLOAT>", "Angle of view", CommandOptionType.SingleValue);
    var gamma = command.Option("-g|--gamma <FLOAT>", "Gamma parameter", CommandOptionType.SingleValue);
    var factor = command.Option("-f|--factor <FLOAT>", "Factor parameter", CommandOptionType.SingleValue);
    var outputFilename = command.Option("--output <OUTPUT_FILENAME>", "Path of the output file", CommandOptionType.SingleValue);
    // NoValue are basically booleans: true if supplied, false otherwise
    var orthogonal = command.Option("-o|--orthogonal", "Use an orthogonal camera instead of a perspective camera", CommandOptionType.NoValue);
    var algorithm = command.Option("-alg|--algorithm", "Use a specific solver to create the image", CommandOptionType.NoValue);
    command.HelpOption("-?|-h|--help");
    
    command.OnExecute(() =>
    {
        // Do the command's work here, or via another object/method  
        
        Console.WriteLine("Executing demo");
        
        // Grab the values of the various options. when not specified, they will be null.

        // The NoValue type has no Value property, just the HasValue() method.
        Parameters.Orthogonal = orthogonal.HasValue();
        Parameters.Algorithm = algorithm.HasValue();
        
        // Check if the various options have values and display them.
        // Here we're checking HasValue() to see if there is a value before displaying the output.
        // Alternatively, you could just handle nulls from the Value properties
        
        // SingleValue returns a single string
        var w = width.Value();
        var h = height.Value();
        var angle = angleDeg.Value();
        var g = gamma.Value();
        var f = factor.Value();
        var output = outputFilename.Value();

        try
        {
            Parameters.Parse_Command_Line_Demo(w,h,angle,g,f,output);
            Console.WriteLine("Parameters: \n" + $"Width: {Parameters.Width} \n" + $"Height: {Parameters.Height} \n"
                        + $"Angle_Deg: {Parameters.AngleDeg} \n" + $"Gamma: {Parameters.Gamma} \n"
                        + $"A: {Parameters.Factor} \n" + $"Orthogonal: {Parameters.Orthogonal} \n" + $"On/Off Algorithm : {Parameters.Algorithm} \n");
            Console.WriteLine($"Generating a {Parameters.Width}x{Parameters.Height} image");
            
            var obsRot = Transformation.Rotation_Z(Parameters.AngleDeg);
            var aspectRatio = (float) Parameters.Width / Parameters.Height;
    
            var image = new HdrImage(Parameters.Width, Parameters.Height);
            

            // Creating the scene
            var world = new World();
            var scale = Transformation.Scale(new Vec(0.1f, 0.1f, 0.1f));
            var red = new Color(0.9f, 0.15f, 0.33f);

            var cube = new List<float> {-0.5f, 0.5f};
            foreach (var x in cube)
                foreach (var y in cube)
                    foreach (var z in cube)
                        world.Add(new Sphere(Transformation.Translation(new Vec(x, y, z)) * scale));

            // Asymmetrical spheres
            world.Add(new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, -0.5f)) * scale));
            world.Add(new Sphere(Transformation.Translation(new Vec(0.0f, 0.5f, 0.0f)) * scale));
            
            // Creating the camera
            ICamera camera;
            if (Parameters.Orthogonal) camera = new OrthogonalCamera(aspectRatio: aspectRatio, t: obsRot * Transformation.Rotation_Y(10.0f) * Transformation.Translation(new Vec(-2.0f, -0.0f, 0.0f)));
            else camera = new PerspectiveCamera(aspectRatio: aspectRatio, t:  obsRot * Transformation.Translation(new Vec(-1.0f, 0.0f, 0.0f)));

            var tracer = new ImageTracer(image, camera);
            
            // Rendering
            if (Parameters.Algorithm)
            {
                Console.WriteLine("Using on/off renderer");
                tracer.Fire_All_Rays(new OnOffTracing(world, Color.Black));
            }

            else if (!Parameters.Algorithm)
            {
                Console.WriteLine("Using Flat renderer");
                tracer.Fire_All_Rays(new FlatTracing(world, Color.Black));
            }
           

            Console.WriteLine("Rendering completed");
            
            // Save pfm file
            const string pfmDemoPath = "Demo.pfm";
            using FileStream outputPfm = File.OpenWrite(pfmDemoPath);
            image.Write_pfm(outputPfm);
            Console.WriteLine($"HDR demo image written to {pfmDemoPath}");
            
            // Convert to Ldr
            // Tone mapping
            image.Luminosity_Norm(Parameters.Factor);
            image.Clamp_Image();

            //using Stream fileStream = File.OpenWrite(Parameters.OutputFileName);
            //const string pngDemoPath = "Demo.png";
            //Parameters.Format = Path.GetExtension(pngDemoPath);
            image.Write_Ldr_Image(Parameters.OutputFileName, Parameters.Format, Parameters.Gamma);
            Console.WriteLine($"PNG demo image written to {Parameters.OutputFileName}");
        }
        
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        return 0; // return 0 on a successful execution
    });
});
// ===========================================================================
// === END === END === END === END === END === END === END === END === END ==
// ===========================================================================


// ===========================================================================
// === CONVERT === CONVERT === CONVERT === CONVERT === CONVERT === CONVERT === 
// ===========================================================================

// Arguments are basic arguments, that are parsed in the order they are given
// e.g ConsoleArgs "first value" "second value"
// This is OK for really simple tasks, but generally you're better off using Options
// since they avoid confusion
app.Command("convert", (command) =>
{
    command.Description = "This is the description for convert.";
    command.ExtendedHelpText = "\nThis is the extended help text for convert.\n";


    var inputFilename = command.Option("-i|--inputFilename <INPUT_FILENAME>", "Path of the input file", CommandOptionType.SingleValue);
    var outputFilename = command.Option("-o|--outputFilename <OUTPUT_FILENAME>", "Path of the output file", CommandOptionType.SingleValue);
    var gamma = command.Option("-g|--gamma <GAMMA>", "Exponent for gamma-correction", CommandOptionType.SingleValue);
    var factor = command.Option("-f|--factor <FACTOR>", "Multiplicative factor", CommandOptionType.SingleValue);
    
    command.HelpOption("-?|-h|--help");

    command.OnExecute(() =>
    {
        Console.WriteLine("Executing convert");

        var i = inputFilename.Value();
        var o = outputFilename.Value();
        var g = gamma.Value();
        var f = factor.Value();
        try
        {
            Parameters.Parse_Command_Line_Convert(i,o,g,f);
            var image = new HdrImage(Parameters.InputPfmFileName);
            // Tone mapping
            image.Luminosity_Norm(Parameters.Factor);
            image.Clamp_Image();

            //using Stream fileStream = File.OpenWrite(Parameters.OutputFileName);
            image.Write_Ldr_Image(Parameters.OutputFileName, Parameters.Format, Parameters.Gamma);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }   
        return 0; // return 0 on a successful execution
    });
});
// ===========================================================================
// === END === END === END === END === END === END === END === END === END ==
// ===========================================================================



// When no commands are specified, this block will execute.
// This is the main "command"
app.OnExecute(() =>
{
    app.ShowHelp();
    return 0;
});

try
{
    // This begins the actual execution of the application
    Console.WriteLine("Executing... \n\n\n");
    app.Execute(args);
}
catch (CommandParsingException ex)
{
    // You'll always want to catch this exception, otherwise it will generate a messy and confusing error for the end user.
    // the message will usually be something like:
    // "Unrecognized command or argument '<invalid-command>'"
    Console.WriteLine(ex.Message);
    app.ShowHelp();
}
catch (Exception ex)
{
    Console.WriteLine("Unable to execute application: {0}", ex.Message);
}