using System.Numerics;

namespace Trace;

public struct Transformation
{
    public Matrix4x4 M; //= new Matrix4x4(); 
    public Matrix4x4 InvM; // = new Matrix4x4();
    
    public Transformation(Matrix4x4 m, Matrix4x4 invM)
    {
        M = m;
        InvM = invM;
    }

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
    
}