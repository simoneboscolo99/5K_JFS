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
    public Normal Neg()
        => new (-X, -Y, -Z);

    //product with a scalar 
    public static Normal operator *(Normal n, float a)
        => new(a * n.X, a * n.Y, a * n.Z);
    
    public static Normal operator *(float a, Normal n)
        => new(a * n.X, a * n.Y, a * n.Z);
    
    //dot product normal*vec
    public float Dot(Vec m)
        => X * m.X + Y * m.Y + Z * m.Z;
    
    public Vec Cross(Vec v)
        => new(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
    
    

}
