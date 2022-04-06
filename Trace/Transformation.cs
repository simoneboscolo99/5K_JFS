using System.Numerics;

namespace Trace;

public struct Transformation
{
    public Matrix4x4 M = new Matrix4x4(); 
    public Matrix4x4 InvM = new Matrix4x4();
}