// See https://aka.ms/new-console-template for more information

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Trace;

Console.WriteLine("Hello, World!");

var image = new HdrImage("memorial.pfm");
Stream outputStream = new MemoryStream();

image.Write_Ldr_Image(outputStream, "Png", 1.8f);





