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
    

    public bool Is_Close (Normal m)
        => Functions.Are_Close(X, m.X) && Functions.Are_Close(Y, m.Y) && Functions.Are_Close(Z, m.Z);

    public void Neg()
    {
        X = -X;
        Y = -Y;
        Z = -Z;
    }
}
