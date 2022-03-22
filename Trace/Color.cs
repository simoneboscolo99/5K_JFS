namespace Trace;

public struct Color
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }

   /// <summary>
   /// Color Constructor
   /// </summary>
   /// <param name="r"> red color </param>
   /// <param name="g"> green color </param>
   /// <param name="b"> blue color </param>
    public Color(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }
    
    public static Color operator +(Color a, Color b)
        => new(a.R + b.R, a.G + b.G, a.B + b.B);
    
    public static Color operator -(Color a, Color b)
        => new(a.R - b.R, a.G - b.G, a.B - b.B);
    
    public static Color operator *(float a, Color b)
        => new(a * b.R, a * b.G, a * b.B);
    
    public static Color operator *(Color b, float a)
        => new(a * b.R, a * b.G, a * b.B);
    
    public static Color operator *(Color a, Color b)
        => new(a.R * b.R, a.G * b.G, a.B * b.B);
    
    public override string ToString() => $"({R}, {G}, {B})";
    
	public bool Is_Close (Color b)
		=> Functions.Are_Close(this.R, b.R) && Functions.Are_Close(this.G, b.G) && Functions.Are_Close(this.B , b.B);
}
