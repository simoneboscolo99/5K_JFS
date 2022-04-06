using System.Numerics;

namespace Trace;

public struct Transformation
{
    public Matrix4x4 M; //= new Matrix4x4(); 
    public Matrix4x4 InvM; // = new Matrix4x4();

    public Transformation Identity()
        => new Transformation(Matrix4x4.Identity, Matrix4x4.Identity);
    
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

    public Transformation Scale(Vec v)
        => new(Matrix4x4.CreateScale(v.X,v.Y,v.Z), Matrix4x4.CreateScale(1/v.X,1/v.Y,1/v.Z));

}