namespace Trace;

/// <summary>
/// A RGB color <br/>
/// The struct has three floating-point members: <see cref="R"/> (red), <see cref="G"/> (green), and <see cref="B"/> (blue)
/// </summary>
public struct Color
{
    /// <summary>
    /// Floating-point number: red component of the color
    /// </summary>
    public float R { get; }
    
    /// <summary>
    /// Floating-point number: green component of the color
    /// </summary>
    public float G { get; }
    
    /// <summary>
    /// Floating-point number: blue component of the color
    /// </summary>
    public float B { get; }
    
    public static Color Black = new();
    public static Color White = new(1.0f, 1.0f, 1.0f);

    /// <summary>
    /// Color Constructor
    /// </summary>
    /// <param name="r"> Red color </param>
    /// <param name="g"> Green color </param>
    /// <param name="b"> Blue color </param>
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

    // Return True if the three RGB components of two colors are close by less than `epsilon`
    /// <summary>
    /// Is_Close
    /// </summary>: Returns true if the color variable is close to the current color class
    /// <param name="b"> Color </param>
    /// <returns></returns>
    public bool Is_Close(Color b)
        => Functions.Are_Close(R, b.R) && Functions.Are_Close(G, b.G) && Functions.Are_Close(B, b.B);

    // Return a rough measure of the luminosity associated with the color
    /// <summary>
    /// Luminosity
    /// </summary>: Returns the average luminosity of a pixel (just numerical, not already normalized)
    /// <returns></returns>
    public float Luminosity()
    {
        return (Math.Max(R, Math.Max(G, B)) + Math.Min(R, Math.Min(G, B))) / 2.0f;
    }
}