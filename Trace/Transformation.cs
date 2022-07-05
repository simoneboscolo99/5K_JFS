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

    /// <summary>
    /// Scale
    /// </summary>: Returns a Transformation with scale factors v.X, v.Y, v.Z, 1.0.
    /// <param name="v"> Vec </param>
    /// <returns></returns>
    public static Transformation Scale(Vec v)
        => new(Matrix4x4.CreateScale(v.X, v.Y, v.Z), Matrix4x4.CreateScale(1 / v.X, 1 / v.Y, 1 / v.Z));

    // ANTICLOCKWISE ROTATION!!!!!!!!!!
    
    /// <summary>
    /// 
    /// </summary> Rotation around X ccw
    /// <param name="angleDeg">degree</param>
    /// <returns></returns>
    public static Transformation Rotation_X(float angleDeg)
        => new Transformation(Matrix4x4.Transpose(Matrix4x4.CreateRotationX(Functions.ToRadians(angleDeg))), Matrix4x4.CreateRotationX(Functions.ToRadians(angleDeg)));

    /// <summary>
    /// 
    /// </summary> Rotation around Y ccw
    /// <param name="angleDeg">degree</param>
    /// <returns></returns>
    public static Transformation Rotation_Y(float angleDeg)
        => new Transformation(Matrix4x4.Transpose(Matrix4x4.CreateRotationY(Functions.ToRadians(angleDeg))), Matrix4x4.CreateRotationY(Functions.ToRadians(angleDeg)));

    /// <summary>
    /// 
    /// </summary> Rotation around Z ccw
    /// <param name="angleDeg">degree</param>
    /// <returns></returns>
    public static Transformation Rotation_Z(float angleDeg)
        => new Transformation(Matrix4x4.Transpose(Matrix4x4.CreateRotationZ(Functions.ToRadians(angleDeg))), Matrix4x4.CreateRotationZ(Functions.ToRadians(angleDeg)));


    public bool Is_Consistent()
    {
        var I = Matrix4x4.Multiply(M, InvM);
        return Functions.Are_Matrices_close(I, Matrix4x4.Identity);
    }

    /// <summary>
    /// Translation of v.X along x, of v.Y along y and of v.Z along z.
    /// </summary> 
    /// <param name="v">Vec(X,Y,Z)</param>
    /// <returns></returns>
    public static Transformation Translation(Vec v)
        => new Transformation(Matrix4x4.Transpose(Matrix4x4.CreateTranslation(v.X, v.Y, v.Z)), Matrix4x4.Transpose(Matrix4x4.CreateTranslation(-v.X, -v.Y, -v.Z)));

    /// <summary>
    /// Inverse
    /// </summary>: returns a new Transformation with each Matrix4x4 inverted
    public Transformation Inverse
        => new(InvM, M);

    public static Transformation operator *(Transformation a, Transformation b)
        => new(Matrix4x4.Multiply(a.M, b.M), Matrix4x4.Multiply(b.InvM, a.InvM));

    public static Point operator *(Transformation a, Point p)
    {
        var newp = new Point(p.X * a.M.M11 + p.Y * a.M.M12 + p.Z * a.M.M13 + a.M.M14,
            p.X * a.M.M21 + p.Y * a.M.M22 + p.Z * a.M.M23 + a.M.M24,
            p.X * a.M.M31 + p.Y * a.M.M32 + p.Z * a.M.M33 + a.M.M34);
        var w = p.X * a.M.M41 + p.Y * a.M.M42 + p.Z * a.M.M43 + a.M.M44;
        if (Math.Abs(w - 1.0) < 10e-5)
            return newp;
        else
        {
            var newpNorm = new Point(newp.X / w, newp.Y / w, newp.Z / w);
            return newpNorm;
        }
    }

    public static Vec operator *(Transformation a, Vec v)
    {
        var newVec = new Vec(v.X * a.M.M11 + v.Y * a.M.M12 + v.Z * a.M.M13,
            v.X * a.M.M21 + v.Y * a.M.M22 + v.Z * a.M.M23,
            v.X * a.M.M31 + v.Y * a.M.M32 + v.Z * a.M.M33);
        return newVec;
    }

    public static Normal operator *(Transformation a, Normal n)
    {
        var c = new Normal(n.X * a.InvM.M11 + n.Y * a.InvM.M21 + n.Z * a.InvM.M31,
            n.X * a.InvM.M12 + n.Y * a.InvM.M22 + n.Z * a.InvM.M32,
            n.X * a.InvM.M13 + n.Y * a.InvM.M23 + n.Z * a.InvM.M33);
        return c;
    }

    public static Ray operator *(Transformation a, Ray ray)
        => new Ray(a * ray.Origin, a * ray.Dir);

    public bool Is_Close(Transformation T)
        => Functions.Are_Matrices_close(M, T.M) && Functions.Are_Matrices_close(InvM, T.InvM);

    public Transformation Clone()
    {
        // ReSharper disable once HeapView.BoxingAllocation
        return ((Transformation)MemberwiseClone());

    }
}