using System.Runtime.CompilerServices;

namespace Trace;

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
