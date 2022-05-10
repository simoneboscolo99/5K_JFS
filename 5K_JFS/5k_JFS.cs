// See https://aka.ms/new-console-template for more information
//not yet :) .... This program is an "image order" RayTracer

using _5K_JFS;
using Trace;
using Microsoft.Extensions.CommandLineUtils;

var app = new CommandLineApplication();
app.Name = "dotnet run";
app.Description = ".NET Core console app with argument parsing.";

app.HelpOption("-?|-h|--help");

// ===========================================================================
// === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO === DEMO ===
// ===========================================================================

// This is a command with no arguments - it just does default action.
app.Command("demo", (command) =>
{
    command.Description = "This is the description for demo.";

    // OPTIONS
    // There are 3 possible option types:
    // NoValue
    // SingleValue
    // MultipleValue (not use in our case)
    
    // MultipleValue options can be supplied as one or multiple arguments
    // e.g. -m valueOne -m valueTwo -m valueThree
    
    // SingleValue: A basic Option with a single value
    // e.g. -s sampleValue
    var width = command.Option("--width <WIDTH>", "Width of the image", CommandOptionType.SingleValue);
    var height = command.Option("--height <HEIGHT>", "Height of the image", CommandOptionType.SingleValue);
    var angleDeg = command.Option("-a|--angle-deg <ANGLE_DEG>", "Angle of view", CommandOptionType.SingleValue);

    // NoValue are basically booleans: true if supplied, false otherwise
    var orthogonal = command.Option("-o|--orthogonal", "Use an orthogonal camera instead of a perspective camera", CommandOptionType.NoValue);
    
    command.HelpOption("-?|-h|--help");

    command.OnExecute(() =>
    {
        // Do the command's work here, or via another object/method  
        
        Console.WriteLine("Executing demo");
        
        // Grab the values of the various options. when not specified, they will be null.

        // The NoValue type has no Value property, just the HasValue() method.
        Parameters.Orthogonal = orthogonal.HasValue();
        
        // SingleValue returns a single string
        var w = width.Value();
        var h = height.Value();
        var angle = angleDeg.Value();
        
        try
        {
            Parameters.Parse_Command_Line_Demo(w, h, angle);
            Console.WriteLine("Parameters: \n" + $"Width: {Parameters.Width} \n" + $"Height: {Parameters.Height} \n"
                        + $"Angle_Deg: {Parameters.AngleDeg} \n" + $"Gamma: {Parameters.Gamma} \n"
                        + $"A: {Parameters.A} \n" + $"Orthogonal: {Parameters.Orthogonal} \n");
            Console.WriteLine($"Generating a {Parameters.Width}x{Parameters.Height} image");
            var obsRot = Transformation.Rotation_Z(Parameters.AngleDeg);
            var aspectRatio = (float) Parameters.Width / Parameters.Height;
    
            var image = new HdrImage(Parameters.Width, Parameters.Height);

            // Creating the scene
            var world = new World();
            var scale = Transformation.Scale(new Vec(0.1f, 0.1f, 0.1f));

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
            if (Parameters.Orthogonal) camera = new OrthogonalCamera(aspectRatio: aspectRatio, t: obsRot * Transformation.Rotation_Y(10.0f) * Transformation.Translation(new Vec(-2.0f, 0.0f, 0.0f)));
            else camera = new PerspectiveCamera(aspectRatio: aspectRatio, t:  obsRot * Transformation.Translation(new Vec(-1.0f, 0.0f, 0.0f)));

            var tracer = new ImageTracer(image, camera);
            
            // Rendering
            Console.WriteLine("Using on/off renderer");
            tracer.Fire_All_Rays(new OnOffTracing(world));
    
            Console.WriteLine("Rendering completed");
            
            // Save pfm file
            const string pfmDemoPath = "Demo.pfm";
            using FileStream outputPfm = File.OpenWrite(pfmDemoPath);
            image.Write_pfm(outputPfm);
            Console.WriteLine($"HDR demo image written to {pfmDemoPath}");
            
            // Convert to Ldr
            // Tone mapping
            image.Luminosity_Norm(Parameters.A);
            image.Clamp_Image();

            //using Stream fileStream = File.OpenWrite(Parameters.OutputFileName);
            const string pngDemoPath = "Demo.png";
            Parameters.Format = Path.GetExtension(pngDemoPath);
            image.Write_Ldr_Image(pngDemoPath, Parameters.Format, Parameters.Gamma);
            Console.WriteLine($"PNG demo image written to {pngDemoPath}");
        }
        
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        return 0; // return 0 on a successful execution
    });
});

// Executed when no commands are specified
app.OnExecute(() =>
{
    app.ShowHelp();
    return 0;
});

try
{
    app.Execute(args);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    app.ShowHelp();
}


// ===========================================================================
// === PFM2PNG === PFM2PNG === PFM2PNG === PFM2PNG === PFM2PNG === PFM2PNG === 
// ===========================================================================

/*HdrImage image;
try
{
    Parameters.Parse_Command_Line(args);
    image = new HdrImage(Parameters.InputPfmFileName);
    // Tone mapping
    image.Luminosity_Norm(Parameters.A);
    image.Clamp_Image();

    //using Stream fileStream = File.OpenWrite(Parameters.OutputFileName);
    image.Write_Ldr_Image(Parameters.OutputFileName, Parameters.Format, Parameters.Gamma);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}*/