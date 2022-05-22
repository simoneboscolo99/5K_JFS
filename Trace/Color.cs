namespace Trace;

/// <summary>
/// A RGB color. <br/>
/// This struct has three floating-point members: <see cref="R"/> (red), <see cref="G"/> (green), and <see cref="B"/> (blue).
/// </summary>
public struct Color
{
    /// <summary>
    /// The red component of the color.
    /// </summary>
    public float R { get; }
    
    /// <summary>
    /// The green component of the color.
    /// </summary>
    public float G { get; }
    
    /// <summary>
    /// The blue component of the color.
    /// </summary>
    public float B { get; }
    
    /// <summary>
    /// Get a <see cref="Color"/> object representing the black color.
    /// </summary>
    public static Color Black = new();
    
    /// <summary>
    /// Get a <see cref="Color"/> object representing the white color.
    /// </summary>
    public static Color White = new(1.0f, 1.0f, 1.0f);

    /// <summary>
    /// Color Constructor. Initialize a new instance of the <see cref="Color"/> struct that has the specified colors.
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
    /// Sums two <see cref="Color"/> objects.
    /// </summary>
    /// <param name="a"> The first color. </param>
    /// <param name="b"> The second color. </param>
    /// <returns> <see cref="Color"/> object given by the sum of <paramref name="a"/> and <paramref name="b"/>. </returns> 
    public static Color operator +(Color a, Color b)
        => new(a.R + b.R, a.G + b.G, a.B + b.B);

    /// <summary>
    /// Subtracts the second color from the first.
    /// </summary>
    /// <param name="a"> The first color. </param>
    /// <param name="b"> The second color. </param>
    /// <returns> The difference color.  </returns> 
    public static Color operator -(Color a, Color b)
        => new(a.R - b.R, a.G - b.G, a.B - b.B);

    /// <summary>
    /// Multiplies one color with one floating-point number.
    /// </summary>
    /// <param name="a" > The scalar value. </param> 
    /// <param name="b"> The color. </param>
    /// <returns> The scaled color. </returns>
    public static Color operator *(float a, Color b)
        => new(a * b.R, a * b.G, a * b.B);

    // let's make the operator * commutative
    
    /// <summary>
    /// Multiply one color with one floating-point number.
    /// </summary>
    /// <param name="b"> The color. </param>
    /// <param name="a"> The scalar value. </param>
    /// <returns> The scaled color. </returns>
    public static Color operator *(Color b, float a)
        => new(a * b.R, a * b.G, a * b.B);

    /// <summary>
    /// Multiply two colors. <br/>
    /// Returns a new <see cref="Color"/> object whose values are the product of each pair of elements in two specified vectors.
    /// </summary>
    /// <param name="a"> The first color. </param>
    /// <param name="b"> The second color. </param>
    /// <returns> The element-wise product color.  </returns>
    public static Color operator *(Color a, Color b)
        => new(a.R * b.R, a.G * b.G, a.B * b.B);

    /// <summary>
    /// Returns the string representation of the current <see cref="Color"/> instance.
    /// </summary>
    public override string ToString() => $"({R}, {G}, {B})";

    /// <summary>
    /// Returns a value indicating whether this instance and a specified <see cref="Color"/> object represent roughly the same color.
    /// </summary>
    /// <param name="b"> The color to compare to this instance. </param>
    /// <param name="eps"> The precision. </param>
    /// <returns> true if the three RGB components of the specified color and of the current instance differ by less than <paramref name="eps"/>; otherwise, false. </returns>
    public bool Is_Close(Color b,  float eps = 1e-5f)
        => Functions.Are_Close(R, b.R, eps) && Functions.Are_Close(G, b.G, eps) && Functions.Are_Close(B, b.B, eps);

    /// <summary>
    /// Return a rough measure of the luminosity associated with the current <see cref="Color"/> instance.
    /// </summary>
    /// <returns> Luminosity of the color. </returns>
    public float Luminosity()
    {
        return (Math.Max(R, Math.Max(G, B)) + Math.Min(R, Math.Min(G, B))) / 2.0f;
    }
}