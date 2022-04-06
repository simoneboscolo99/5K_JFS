using System.Numerics;

namespace Trace;

public struct Transformation
{
    public Matrix4x4 M; //= new Matrix4x4(); 
    public Matrix4x4 InvM; // = new Matrix4x4();

    public static Transformation Identity() => new(Matrix4x4.Identity, Matrix4x4.Identity);
    
    public Transformation(Matrix4x4 m, Matrix4x4 invM)
    {
        M = m;
        InvM = invM;
    }
    
    public Transformation Scale(Vec v)
        => new(Matrix4x4.CreateScale(v.X,v.Y,v.Z), Matrix4x4.CreateScale(1/v.X,1/v.Y,1/v.Z));

    public Transformation Rotation_X(float angle)
        => new Transformation(Matrix4x4.Transpose(Matrix4x4.CreateRotationX(angle)), Matrix4x4.CreateRotationX(angle));
    
    public Transformation Rotation_Y(float angle)
        => new Transformation(Matrix4x4.Transpose(Matrix4x4.CreateRotationY(angle)), Matrix4x4.CreateRotationY(angle));
    
    public Transformation Rotation_Z(float angle)
        => new Transformation(Matrix4x4.Transpose(Matrix4x4.CreateRotationZ(angle)), Matrix4x4.CreateRotationZ(angle));

    public bool Is_Consistent()
    {
        var I = Matrix4x4.Multiply(M, InvM);
        return Functions.Are_Matr_close(I, Matrix4x4.Identity);
    }

    public bool Is_Close(Transformation T)
        => Functions.Are_Matr_close(M, T.M) && Functions.Are_Matr_close(InvM, T.InvM);
}