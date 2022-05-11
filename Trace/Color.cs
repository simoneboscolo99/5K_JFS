namespace Trace;

public struct Color
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    
    public static Color Black = new Color();
    public static Color White = new Color(1.0f, 1.0f, 1.0f);

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

    /// <summary>
    /// Operator +
    /// </summary>: Overloading operator '+'
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Color operator +(Color a, Color b)
        => new(a.R + b.R, a.G + b.G, a.B + b.B);

    /// <summary>
    /// Operator -
    /// </summary>: Overloading operator '-'
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Color operator -(Color a, Color b)
        => new(a.R - b.R, a.G - b.G, a.B - b.B);

    /// <summary>
    /// Operator *
    /// </summary>: Overloading operator '*' 
    /// <param name="a" > scalar </param> 
    /// <param name="b"> Color </param> Color
    /// <returns></returns>
    public static Color operator *(float a, Color b)
        => new(a * b.R, a * b.G, a * b.B);

    /// <summary>
    /// Operator *
    /// </summary>: Overloading operator '*' 
    /// <param name="b"> Color </param>
    /// <param name="a"> Scalar </param>
    /// <returns></returns>
    public static Color operator *(Color b, float a)
        => new(a * b.R, a * b.G, a * b.B);

    /// <summary>
    /// Operator *-cross product
    /// </summary>: Gives the scalar A*B
    /// <param name="a"> Color </param>
    /// <param name="b"> Color </param>
    /// <returns></returns>
    public static Color operator *(Color a, Color b)
        => new(a.R * b.R, a.G * b.G, a.B * b.B);

    /// <summary>
    /// Override 'ToString'
    /// </summary>: Converts to a string the Color's components
    /// <returns></returns>
    public override string ToString() => $"({R}, {G}, {B})";

    /// <summary>
    /// Is_Close
    /// </summary>: Returns true if the color variable is close to the current color class
    /// <param name="b"> Color </param>
    /// <returns></returns>
    public bool Is_Close(Color b)
        => Functions.Are_Close(R, b.R) && Functions.Are_Close(G, b.G) && Functions.Are_Close(B, b.B);

    /// <summary>
    /// Luminosity
    /// </summary>: Returns the average luminosity of a pixel (just numerical, not already normalized)
    /// <returns></returns>
    public float Luminosity()
    {
        return (Math.Max(R, Math.Max(G, B)) + Math.Min(R, Math.Min(G, B))) / 2.0f;
    }
    
}

