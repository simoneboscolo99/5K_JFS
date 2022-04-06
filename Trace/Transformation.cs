using System.Numerics;

namespace Trace;

public struct Transformation
{
    public Matrix4x4 M; //= new Matrix4x4(); 
    public Matrix4x4 InvM; // = new Matrix4x4();
    
    public Transformation(Matrix4x4 m, Matrix4x4 invM)
    {
        //var m1 = m ?? Matrix4x4.Identity;
        M = m;
        InvM = invM;
    }

    public bool Is_Consistent()
    {
        var I = Matrix4x4.Multiply(M, InvM);
        return I.IsIdentity;
    }
    
}