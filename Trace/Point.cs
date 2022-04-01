using System.Numerics;

namespace Trace;

public struct Point
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    
    /// <summary>
    /// Point Constructor
    /// </summary>
    /// <param name="x"> x coordinate </param>
    /// <param name="y"> y coordinate </param>
    /// <param name="z"> z coordinate </param>
    public Point(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    /// <summary>
    /// Override 'ToString'
    /// </summary>: Converts to a string the Point's components
    /// <returns></returns>
    public override string ToString() => $"({X}, {Y}, {Z})";
    
    /// <summary>
    /// Is_Close
    /// </summary>: Returns true if the point variable is close to the current point class
    /// <param name="b"> Point </param>
    /// <returns></returns>
    public bool Is_Close (Point b)
        => Functions.Are_Close(X, b.X) && Functions.Are_Close(Y, b.Y) && Functions.Are_Close(Z , b.Z);
    
    /// <summary>
    /// Operator +
    /// </summary>: Overloading operator '+'
    /// <param name="a"> Point </param>
    /// <param name="b"> Vec </param>
    /// <returns></returns>
    public static Point operator +(Point a, Vec b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    
    /// <summary>
    /// Operator +
    /// </summary>: Overloading operator '+'
    /// <param name="a"> Point </param>
    /// <param name="b"> Point </param>
    /// <returns></returns>
    public static Point operator +(Point a, Point b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    
    /// <summary>
    /// Operator -
    /// </summary>: Overloading operator '-'
    /// <param name="a" > Point </param> 
    /// <param name="b" > Point </param>
    /// <returns></returns>
    public static Vec operator -(Point a, Point b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    
    /// <summary>
    /// Operator -
    /// </summary>: Overloading operator '-'
    /// <param name="a" > Point </param> 
    /// <param name="b" > Vec </param>
    /// <returns></returns>
    public static Point operator -(Point a, Vec b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    
    /// <summary>
    /// Operator *
    /// </summary>: Overloading operator '*' 
    /// <param name="b"> Point </param>
    /// <param name="a"> Scalar </param>
    /// <returns></returns>
    public static Point operator *(Point b, float a)
        => new(a * b.X, a * b.Y, a * b.Z);
    
    /// <summary>
    /// Operator *
    /// </summary>: Overloading operator '*' 
    /// <param name="a" > scalar </param> 
    /// <param name="b"> Point </param> 
    /// <returns></returns>
    public static Point operator *(float a, Point b)
        => new(a * b.X, a * b.Y, a * b.Z);
    
    /// <summary>
    /// Override 'Point.To_Vec()'
    /// </summary>: Converts to a Vec the Point's components
    /// <returns></returns>
    public Vec To_Vec()
        => new(X, Y, Z);
}