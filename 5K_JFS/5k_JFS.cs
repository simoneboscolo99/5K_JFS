// See https://aka.ms/new-console-template for more information
//not yet :) .... This program is an "image order" RayTracer
using Microsoft.Extensions.CommandLineUtils;
using Trace;
using _5K_JFS;

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
app.Command("demo", command =>
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
    var width = command.Option("--width <INTEGER>", "Width of the image. \t\t Default: 480", CommandOptionType.SingleValue);
    var height = command.Option("--height <INTEGER>", "Height of the image. \t\t Default: 480", CommandOptionType.SingleValue);
    var angleDeg = command.Option("-a|--angle-deg <FLOAT>", "Angle of view. \t\t\t Default: 0", CommandOptionType.SingleValue);
    var outputFilename = command.Option("--output <OUTPUT_FILENAME>", "Path of the output ldr file. \t Default: Demo.png", CommandOptionType.SingleValue);
    var algorithm = command.Option("--algorithm <ALGORITHM>", "Algorithm of rendering. \t\t Default: pathtracing", CommandOptionType.SingleValue);
    var gamma = command.Option("-g|--gamma <FLOAT>", "Exponent for gamma-correction. \t Default: 1", CommandOptionType.SingleValue);
    var factor = command.Option("-f|--factor <FLOAT>", "Multiplicative factor. \t\t Default: 0,2", CommandOptionType.SingleValue);
    var samplesPerPixel = command.Option("-spp|--samples-per-pixel <SAMPLES_PER_PIXEL>", "Number of samples per pixel (must be a perfect square, e.g., 16).. \t\t Default: 0", CommandOptionType.SingleValue);

    // Opzioni che si possono aggiungere:
    // init_state, init_seq, (pcg)
    // num_of_rays, max_depth, russian_roulette_limit? (path tracing)
    
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
        var ssp = samplesPerPixel.Value();

        try
        {
            Parameters.Parse_Command_Line_Demo(w, h, angle, g, f, output, ssp);
            Console.WriteLine("Parameters: \n" 
                              + $"Width: {Parameters.Width} \n" 
                              + $"Height: {Parameters.Height} \n"
                              + $"Angle_Deg: {Parameters.AngleDeg} \n" 
                              + $"Gamma: {Parameters.Gamma} \n"
                              + $"A: {Parameters.Factor} \n" 
                              + $"Orthogonal: {Parameters.Orthogonal} \n" 
                              + $"Samples per side: {Parameters.SamplesPerSide} \n");
            
            Console.WriteLine($"Generating a {Parameters.Width}x{Parameters.Height} image");
            
            var obsRot = Transformation.Rotation_Z(Parameters.AngleDeg);
            var aspectRatio = (float) Parameters.Width / Parameters.Height;
    
            var image = new HdrImage(Parameters.Width, Parameters.Height);
            
            var world = new World();

            // Creating the scene
            
            // Code for the closed cylinder
            /*var cylinderMaterial = new Material(
                new DiffuseBrdf(new CheckeredPigment(
                    new Color(0.3f, 0.5f, 0.1f), 
                    new Color(0.1f, 0.2f, 0.5f),
                    6))
                );
            
            var cylinder = new Cylinder(m: cylinderMaterial);
            var bottomDisk = new Disk(m: cylinderMaterial);
            var upDisk = new Disk(T: Transformation.Translation(new Vec(0.0f, 0.0f, 1.0f)), cylinderMaterial);

            var closedCylinder = new CsgUnion(cylinder, new CsgUnion(bottomDisk, upDisk), Transformation.Scale(new Vec(2.0f, 2.0f, 2.0f)));
            world.Add(closedCylinder);*/
            
            // Demo image
            var skyMaterial = new Material(
                new DiffuseBrdf(new UniformPigment(new Color())), 
                new UniformPigment(new Color(0.1f, 0.5f, 0.9f))
            );
            var groundMaterial = new Material(
                new DiffuseBrdf(
                    new CheckeredPigment(
                        new Color(0.3f, 0.5f, 0.1f), 
                        new Color(0.1f, 0.2f, 0.5f)
                    )
                )
            );
            var sphereMaterial = new Material(new DiffuseBrdf(new UniformPigment(new Color(0.3f, 0.4f, 0.8f))));
            var mirrorMaterial = new Material(new SpecularBrdf(new UniformPigment(new Color(0.6f, 0.2f, 0.3f))));            
            
            world.Add(new Sphere(
                    Transformation.Scale(new Vec(200f, 200f, 200f)) * Transformation.Translation(new Vec(0.0f, 0.0f, 0.4f)),
                    skyMaterial
                )
            );
            world.Add(new Plane(m: groundMaterial));
            world.Add(new Sphere(
                    Transformation.Translation(new Vec(0.0f, 0.0f, 1.0f)),
                    sphereMaterial
                )
            );
            world.Add(new Sphere(
                    Transformation.Translation(new Vec(1.0f, 2.5f, 0.0f)),
                    mirrorMaterial
                )
            );

            // Spheres at the vertices of the cube
            /*var scale = Transformation.Scale(new Vec(0.1f, 0.1f, 0.1f));
            
            // Colors of the image
            var c = new Color(0.2f, 0.5f, 0.2f);
            var c1 = new Color(0.5f, 0.1f, 0.1f);
            var c2 = new Color(0.1f, 0.1f, 0.5f);

            var cube = new List<float> {-0.5f, 0.5f};
            foreach (var x in cube)
            {
                foreach (var y in cube)
                {
                    foreach (var z in cube)
                    {

                        world.Add(new Sphere(Transformation.Translation(new Vec(x, y, z))
                                             * scale, new Material
                            (new DiffuseBrdf(new CheckeredPigment(Color.Black, Color.White)))));

                        world.Add(new Sphere(Transformation.Translation(new Vec(x, y, z)) * scale,
                            new Material(new DiffuseBrdf(new UniformPigment(c)))));
                    }
                }
            }

            // Asymmetrical spheres
            world.Add(new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, -0.5f)) * scale, 
                new Material(new DiffuseBrdf(new CheckeredPigment(c1, c2, 2)))));
            world.Add(new Sphere(Transformation.Translation(new Vec(0.0f, 0.5f, 0.0f)) * scale, 
                new Material(new DiffuseBrdf(new CheckeredPigment(c2, c1, 2))))); */
            
            // Creating the camera
            ICamera camera;
            if (Parameters.Orthogonal) camera = new OrthogonalCamera(aspectRatio: aspectRatio, t: obsRot * Transformation.Rotation_Y(30.0f) * Transformation.Translation(new Vec(-1.0f, -0.0f, 0.0f)));
            else camera = new PerspectiveCamera(aspectRatio: aspectRatio, t:  obsRot * Transformation.Rotation_Y(25.0f) * Transformation.Translation(new Vec(-1.0f, 0.0f, 0.5f)));
            
            var tracer = new ImageTracer(image, camera, Parameters.SamplesPerSide);
            
            // Rendering
            var alg = algorithm.Value() ?? "PATHTRACING";
            var upperAlg = alg.ToUpper();
            Solver renderer;
            switch (upperAlg)
            {
                case "ONOFF":
                    renderer = new OnOffTracing(world);
                    Console.WriteLine("Using on/off renderer\n");
                    break;
                case "FLAT":
                    renderer = new FlatTracing(world);
                    Console.WriteLine("Using flat renderer\n");
                    break;
                case "PATHTRACING":
                    renderer = new PathTracing(world, maxDepth: 3, numOfRays: 5, russianRouletteLimit: 3);
                    Console.WriteLine("Using path tracing\n");
                    break;
                default:
                    throw new RuntimeException($"\nInvalid renderer {alg}. Possible renderers are:" +
                                                   "\n - onoff \n - flat \n - pathtracing \n");
            }
            
            tracer.Fire_All_Rays(renderer);
            
            // Save pfm file
            const string pfmDemoPath = "Demo.pfm";
            using FileStream outputPfm = File.OpenWrite(pfmDemoPath);
            image.Write_pfm(outputPfm);
            Console.WriteLine($"\nHDR demo image written to {pfmDemoPath}");
            
            // Convert to Ldr
            // Tone mapping
            image.Luminosity_Norm(Parameters.Factor, luminosity: 0.2f);
            image.Clamp_Image();
            
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
// === END === END === END === END === END === END === END === END === END ===
// ===========================================================================


// ===========================================================================
// === CONVERT === CONVERT === CONVERT === CONVERT === CONVERT === CONVERT === 
// ===========================================================================

// Arguments are basic arguments, that are parsed in the order they are given
// e.g ConsoleArgs "first value" "second value"
// This is OK for really simple tasks, but generally you're better off using Options
// since they avoid confusion
app.Command("convert", command =>
{
    command.Description = "This is the description for convert.";
    command.ExtendedHelpText = "\nThis is the extended help text for convert.\n";


    var inputFilename = command.Option("-i|--inputFilename <INPUT_FILENAME>", "Path of the input file", CommandOptionType.SingleValue);
    var outputFilename = command.Option("-o|--outputFilename <OUTPUT_FILENAME>", "Path of the output ldr file", CommandOptionType.SingleValue);
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


// ===========================================================================
// ==== RENDER ==== RENDER ==== RENDER ==== RENDER ==== RENDER ==== RENDER === 
// ===========================================================================

// This is a command with no arguments - it just does default action.
app.Command("render", command =>
{
    // This is a command that has it's own options
    // description and help text of the command
    command.Description = "This is the description for render.";
    command.ExtendedHelpText = "\nThis is the extended help text for render.\n";

    var inputSceneName = command.Argument("inputSceneName", "Name of the input file with the scene.");
    var width = command.Option("--width <INTEGER>", "Width of the image. \t\t Default: 480",
        CommandOptionType.SingleValue);
    var height = command.Option("--height <INTEGER>", "Height of the image. \t\t Default: 480",
        CommandOptionType.SingleValue);
    var angleDeg = command.Option("-a|--angle-deg <FLOAT>", "Angle of view. \t\t\t Default: 0",
        CommandOptionType.SingleValue);
    var pfmoutputFilename = command.Option("--pfm-output <PFM_OUTPUT_FILENAME>",
        "Path of the output hdr file. \t Default: output.pfm", CommandOptionType.SingleValue);
    var outputFilename = command.Option("--output <OUTPUT_FILENAME>",
        "Path of the output ldr file. \t Default: output.png", CommandOptionType.SingleValue);
    var algorithm = command.Option("--algorithm <ALGORITHM>", "Algorithm of rendering. \t\t Default: pathtracing",
        CommandOptionType.SingleValue);
    var gamma = command.Option("-g|--gamma <FLOAT>", "Exponent for gamma-correction. \t Default: 1",
        CommandOptionType.SingleValue);
    var factor = command.Option("-f|--factor <FLOAT>", "Multiplicative factor. \t\t Default: 0,2",
        CommandOptionType.SingleValue);
    var numOfRays = command.Option("-n|--num-of-rays <INTEGER>",
        "Number of rays departing from each surface point (only applicable with --algorithm=pathtracing).\t\t Default: 10",
        CommandOptionType.SingleValue);
    var maxDepth = command.Option("-md|--max-depth <INTEGER>",
        "Maximum allowed ray depth (only applicable with --algorithm=pathtracing). \t\t Default: 3",
        CommandOptionType.SingleValue);
    var initState = command.Option("--init-state <INTEGER>",
        "Initial seed for the random number generator (positive number). \t\t Default: 45",
        CommandOptionType.SingleValue);
    var initSeq = command.Option("--init-seq <INTEGER>",
        "Identifier of the sequence produced by the random number generator (positive number). \t\t Default: 54",
        CommandOptionType.SingleValue);
    var samplesPerPixel = command.Option("-spp|--samples-per-pixel <SAMPLES_PER_PIXEL>",
        "Number of samples per pixel (must be a perfect square, e.g., 16).. \t\t Default: 0",
        CommandOptionType.SingleValue);
    var declareFloat = command.Option("-d|--declare-float <STRING>",
        "Declare a variable. The syntax is «--declare-float=VAR:VALUE». Example: --declare-float=clock:150",
        CommandOptionType.MultipleValue);

    command.HelpOption("-?|-h|--help");

    command.OnExecute(() =>
    {
        // Do the command's work here, or via another object/method  

        Console.WriteLine("Executing render");

        var input = inputSceneName.Value;
        var w = width.Value();
        var h = height.Value();
        var angle = angleDeg.Value();
        var g = gamma.Value();
        var f = factor.Value();
        var output = outputFilename.Value();
        var outputPfm = pfmoutputFilename.Value();
        var ssp = samplesPerPixel.Value();
        var num = numOfRays.Value();
        var max = maxDepth.Value();
        var inState = initState.Value();
        var inSeq = initSeq.Value();
        var decFloat = declareFloat.Values;

        try
        {
            Parameters.Parse_Command_Line_Render(w,h, angle, g, f, 
                output, ssp, inSeq, inState, max, outputPfm, decFloat, num, input);
            
            Console.WriteLine("Parameters: \n" 
                              + $"Width: {Parameters.Width} \n" 
                              + $"Height: {Parameters.Height} \n"
                              + $"Angle_Deg: {Parameters.AngleDeg} \n" 
                              + $"Gamma: {Parameters.Gamma} \n"
                              + $"A: {Parameters.Factor} \n" 
                              + $"Orthogonal: {Parameters.Orthogonal} \n" 
                              + $"Samples per side: {Parameters.SamplesPerSide} \n"
                              + $"Init State: {Parameters.InitState} \n"
                              + $"Init Seq: {Parameters.InitSeq} \n"
                              + $"Number of rays: {Parameters.NumOfRays} \n"
                              + $"Max depth: {Parameters.MaxDepth} \n"
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        var scene = new Scene();

        using (Stream fileStream = File.OpenRead(Parameters.InputSceneName))
        {
            try
            {
                scene = Scene.ParseScene(new InputStream(fileStream, Parameters.InputSceneName), 
                    Parameters.DeclareFloat);
            }
            catch (GrammarErrorException e)
            {
                var loc = e.Location;
                Console.WriteLine($"{loc.FileName}:{loc.LineNum}:{loc.ColumnNum}: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        try
        {
            Console.WriteLine($"Generating a {Parameters.Width}x{Parameters.Height} image");
            var image = new HdrImage(Parameters.Width, Parameters.Height);
            
            Console.WriteLine($"Aspect ratio: {((PerspectiveCamera) scene.Camera!).AspectRatio}");
            Console.WriteLine($"Distance: {((PerspectiveCamera) scene.Camera!).Distance}");
            
            Console.WriteLine($"SkyMaterial: { ((UniformPigment) scene.Materials["skyMaterial"].EmittedRadiance).C.ToString()}");
            var tracer = new ImageTracer(image, scene.Camera!, Parameters.SamplesPerSide);
            
            // Rendering
            var alg = algorithm.Value() ?? "PATHTRACING";
            var upperAlg = alg.ToUpper();
            Solver renderer;
            switch (upperAlg)
            {
                case "ONOFF":
                    renderer = new OnOffTracing(scene.Wd);
                    Console.WriteLine("Using on/off renderer\n");
                    break;
                case "FLAT":
                    renderer = new FlatTracing(scene.Wd);
                    Console.WriteLine("Using flat renderer\n");
                    break;
                case "PATHTRACING":
                    renderer = new PathTracing(scene.Wd, pcg: new Pcg(Parameters.InitState, Parameters.InitSeq), maxDepth: Parameters.MaxDepth, numOfRays: Parameters.NumOfRays);
                    Console.WriteLine("Using path tracing\n");
                    break;
                default:
                    throw new RuntimeException($"\nInvalid renderer {algorithm}. Possible renderers are:" +
                                               "\n - onoff \n - flat \n - pathtracing \n");
            }
            
            tracer.Fire_All_Rays(renderer);
        
            // Save pfm file
            using FileStream outputpfm = File.OpenWrite(Parameters.PfmOutputFilename);
            image.Write_pfm(outputpfm);
            Console.WriteLine($"\nHDR demo image written to {Parameters.PfmOutputFilename}");
            
            // Convert to Ldr
            // Tone mapping
            image.Luminosity_Norm(Parameters.Factor);
            image.Clamp_Image();
            
            image.Write_Ldr_Image(Parameters.OutputFileName, Parameters.Format, Parameters.Gamma);
            Console.WriteLine($"PNG demo image written to {Parameters.OutputFileName}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        return 0;
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