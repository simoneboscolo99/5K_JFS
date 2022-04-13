namespace Trace;

/// <summary>
/// 
/// </summary>
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
    /// <param name="v"> <see cref="Vec"/> </param>
    /// <param name="w"> <see cref="Vec"/> </param>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vec operator -(Vec v)
        => new(-v.X, -v.Y, -v.Z);

    public float Dot(Vec v)
        => X * v.X + Y * v.Y + Z * v.Z;
   //dot product <vec,normal>
    public float Dot(Normal m)
        => X * m.X + Y * m.Y + Z * m.Z;

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

    public Normal ToNormal()
        => new Normal(X, Y, Z);

    /// <summary>
    /// Is_Close
    /// </summary>: Returns true if the vec variable is close to the current vec class
    /// <param name="v"> Vec </param>
    /// <returns></returns>
    public bool Is_Close (Vec v)
        => Functions.Are_Close(X, v.X) && Functions.Are_Close(Y, v.Y) && Functions.Are_Close(Z , v.Z);
    
    public override string ToString() => $"({X}, {Y}, {Z})";
}

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

public struct Normal
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    
   
    public Normal(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString() => $"({X}, {Y}, {Z})";
    
    public bool Is_Close(Normal m)
        => Functions.Are_Close(X, m.X) && Functions.Are_Close(Y, m.Y) && Functions.Are_Close(Z, m.Z);

    //negation (-n)
    public static Normal operator -(Normal n)
        => new (-n.X, -n.Y, -n.Z);

    //product with a scalar 
    public static Normal operator *(Normal n, float a)
        => new(a * n.X, a * n.Y, a * n.Z);
    
    public static Normal operator *(float a, Normal n)
        => new(a * n.X, a * n.Y, a * n.Z);
    
    //dot product normal*vec
    public float Dot(Vec m)
        => X * m.X + Y * m.Y + Z * m.Z;
    
    public float Dot(Normal m)
        => X * m.X + Y * m.Y + Z * m.Z;
    
    //vector product
    public Normal Cross(Vec v)
        => new(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
    public Normal Cross(Normal m)
        => new(Y * m.Z - Z * m.Y, Z * m.X - X * m.Z, X * m.Y - Y * m.X);

    public float SquaredNorm()
        => (float) (Math.Pow(X, 2f) + Math.Pow(Y, 2f) + Math.Pow(Z, 2f));

    public float Norm()
        => (float) Math.Sqrt(SquaredNorm());

    public Normal Normalize()
        => new(X / Norm(), Y / Norm(), Z / Norm());
       
}
