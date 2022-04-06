using System.Numerics;

namespace Trace;

public struct Transformation
{
    public Matrix4x4 M = new Matrix4x4(); 
    public Matrix4x4 InvM = new Matrix4x4();

    public Transformation Translation(Vec v)
    {
        M = (1.0f, 0.0f, 0.0f, v.X, 0.0f, 1.0f, 0.0f, v.Y, 0.0f, 0.0f, 1.0f, v.Z, 0.0f, 0.0f, 0.0f, 1.0f);
        InvM = (1.0f, 0.0f, 0.0f, -v.X, 0.0f, 1.0f, 0.0f, -v.Y, 0.0f, 0.0f, 1.0f, -v.Z, 0.0f, 0.0f, 0.0f, 1.0f);
    }
    
}