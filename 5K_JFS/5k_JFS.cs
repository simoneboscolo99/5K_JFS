﻿// See https://aka.ms/new-console-template for more information
//not yet :) .... This program is an "image order" RayTracer

using _5K_JFS;
using Trace;

HdrImage image;
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
Console.WriteLine("Hello, world");
