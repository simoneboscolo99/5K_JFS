namespace Trace;

public struct Vec
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    
    /// <summary>
    /// Vec Constructor
    /// </summary>
    /// <param name="x"> x coordinate </param>
    /// <param name="y"> y coordinate </param>
    /// <param name="z"> z coordinate </param>
    public Vec(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Operator +
    /// </summary>: Overloading operator '+'
    /// <param name="v"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static Vec operator +(Vec v, Vec w)
        => new(v.X + w.X, v.Y + w.Y, v.Z + w.Z);
    
    /// <summary>
    /// Operator -
    /// </summary>: Overloading operator '-'
    /// <param name="v"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static Vec operator -(Vec v, Vec w)
        => new(v.X - w.X, v.Y - w.Y, v.Z - w.Z);
    
    /// <summary>
    /// Operator *
    /// </summary>: Overloading operator '*' 
    /// <param name="a" > scalar </param> 
    /// <param name="v"> Color </param> Color
    /// <returns></returns>
    public static Vec operator *(float a, Vec v)
        => new(a * v.X, a * v.Y, a * v.Z);
    
    /// <summary>
    /// Operator *
    /// </summary>: Overloading operator '*' 
    /// <param name="v"> Color </param>
    /// <param name="a"> Scalar </param>
    /// <returns></returns>
    public static Vec operator *(Vec v, float a)
        => new(a * v.X, a * v.Y, a * v.Z);

    public Vec Neg()
        => new(-X, -Y, -Z);
    
    public static Vec operator -(Vec v)
        => new(-v.X, -v.Y, -v.Z);

    public float Dot(Vec v)
        => X * v.X + Y * v.Y + Z * v.Z;

    public Vec Cross(Vec v)
        => new(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);

    public static Vec Cross(Vec v, Vec w)
        => new(v.Y * w.Z - v.Z * w.Y, v.Z * w.X - v.X * w.Z, v.X * w.Y - v.Y * w.X);

    public float Squared_Norm()
        => X * X + Y * Y + Z * Z;

    public float Norm()
        => (float) Math.Sqrt(Squared_Norm());

    public Vec Normalize()
    {
        var mod = Norm();
        return new Vec(X / mod, Y / mod, Z / mod);
    }
       


    /// <summary>
    /// Is_Close
    /// </summary>: Returns true if the vec variable is close to the current vec class
    /// <param name="v"> Vec </param>
    /// <returns></returns>
    public bool Is_Close (Vec v)
        => Functions.Are_Close(X, v.X) && Functions.Are_Close(Y, v.Y) && Functions.Are_Close(Z , v.Z);
    
    public override string ToString() => $"({X}, {Y}, {Z})";
}