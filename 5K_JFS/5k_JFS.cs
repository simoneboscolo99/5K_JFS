// See https://aka.ms/new-console-template for more information

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Trace;


var image = new HdrImage("memorial.pfm");
// Create a sRGB bitmap
var bitmap = new Image<Rgb24>(Configuration.Default, image.Width, image.Height);

// Save the bitmap as a PNG file
using (Stream fileStream = File.OpenWrite("output.png")) {
    bitmap.Save(fileStream, new PngEncoder());
}
Console.WriteLine("Hello, World!");



