// See https://aka.ms/new-console-template for more information
//not yet :) .... This program is an "image order" RayTracer

using _5K_JFS;
using Microsoft.Extensions.CommandLineUtils;
using Trace;


            try
            {
                Parameters.Parse_Command_Line_Demo(args);
                Console.WriteLine("Parameters: \n" + $"Width: {Parameters.Width} \n" + $"Height: {Parameters.Height} \n"
                                  + $"Angle_Deg: {Parameters.AngleDeg} \n" + $"Gamma: {Parameters.Gamma} \n"
                                  + $"A: {Parameters.A} \n" + $"Orthogonal: {Parameters.Orthogonal} \n");
                Console.WriteLine($"Generating a {Parameters.Width}x{Parameters.Height} image");
                var obsRot = Transformation.Rotation_Z(Parameters.AngleDeg);
                var aspetcRatio = (float) Parameters.Width / Parameters.Height;

                var image = new HdrImage(Parameters.Width, Parameters.Height);

                // Creating the scene
                var world = new World();
                var rscale = Transformation.Scale(new Vec(0.1f, 0.1f, 0.1f));

                // Spheres at the vertices of the cube
                world.Add(new Sphere(Transformation.Translation(new Vec(0.5f, 0.5f, 0.5f)) * rscale));
                world.Add(new Sphere(Transformation.Translation(new Vec(-0.5f, 0.5f, 0.5f)) * rscale));
                world.Add(new Sphere(Transformation.Translation(new Vec(0.5f, -0.5f, 0.5f)) * rscale));
                world.Add(new Sphere(Transformation.Translation(new Vec(0.5f, 0.5f, -0.5f)) * rscale));
                world.Add(new Sphere(Transformation.Translation(new Vec(-0.5f, -0.5f, 0.5f)) * rscale));
                world.Add(new Sphere(Transformation.Translation(new Vec(-0.5f, 0.5f, -0.5f)) * rscale));
                world.Add(new Sphere(Transformation.Translation(new Vec(0.5f, -0.5f, -0.5f)) * rscale));
                world.Add(new Sphere(Transformation.Translation(new Vec(-0.5f, -0.5f, -0.5f)) * rscale));

                // Asymmetrical spheres
                world.Add(new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, -0.5f)) * rscale));
                world.Add(new Sphere(Transformation.Translation(new Vec(0.0f, 0.5f, 0.0f)) * rscale));

                // Creating the camera
                ICamera camera;
                if (Parameters.Orthogonal)
                    camera = new OrthogonalCamera(aspectRatio: aspetcRatio,
                        t: obsRot * Transformation.Rotation_Y(10.0f) *
                           Transformation.Translation(new Vec(-2.0f, 0.0f, 0.0f)));
                else
                    camera = new PerspectiveCamera(aspectRatio: aspetcRatio,
                        t: obsRot * Transformation.Translation(new Vec(-2.0f, 0.0f, 0.0f)));

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
        
// ========================================
// =============== pfm2png ================
// ========================================

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
}
Console.WriteLine("Hello, world");*/
