// See https://aka.ms/new-console-template for more information

using _5K_JFS;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Trace;

try
{
    Parameters.Parse_Command_Line(args);
    Console.WriteLine(Parameters.InputPfmFileName);
    Console.WriteLine(Parameters.A);
    Console.WriteLine(Parameters.Gamma);
    Console.WriteLine(Parameters.OutputPngFileName);
    
    HdrImage image = new HdrImage(Parameters.InputPfmFileName);

    image.Luminosity_Norm(Parameters.A);
    image.Clamp_Image();
    // Create a sRGB bitmap
    //var bitmap = new Image<Rgb24>(Configuration.Default, image.Width, image.Height);

// Save the bitmap as a PNG file
   using (Stream fileStream = File.OpenWrite("output.pfm")) {
   //   bitmap.Save(fileStream, new PngEncoder());
    image.Write_Ldr_Image(fileStream, "png", Parameters.Gamma);
   }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine("Hello, World!");



