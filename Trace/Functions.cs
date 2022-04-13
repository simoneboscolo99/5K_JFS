using System.Numerics;

namespace Trace;

public class Functions
{
    /// <summary>
    /// Are_Close
    /// </summary>: : Returns true if |a-b| is smaller than 10^-5
    /// <param name="val1"></param>
    /// <param name="val2"></param>
    /// <param name="eps"></param>
    /// <returns></returns>
    public static bool Are_Close(float val1, float val2, float eps = 1e-5f)
    {
        return Math.Abs(val1 - val2) < eps;
    }
    /// <summary>
    /// Are_Matr_Close
    /// </summary>: returns true if each entry of two matrixes are_close(10^-5) else returns false 
    /// <param name="m"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static bool Are_Matr_close(Matrix4x4 m, Matrix4x4 n)
    {
        return Are_Close(m.M11, n.M11) && Are_Close(m.M12, n.M12) && Are_Close(m.M13, n.M13) && Are_Close(m.M14, n.M14) && 
               Are_Close(m.M21, n.M21) && Are_Close(m.M22, n.M22) && Are_Close(m.M23, n.M23) && Are_Close(m.M24, n.M24) && 
               Are_Close(m.M31, n.M31) && Are_Close(m.M32, n.M32) && Are_Close(m.M33, n.M33) && Are_Close(m.M34, n.M34) &&
               Are_Close(m.M41, n.M41) && Are_Close(m.M42, n.M42) && Are_Close(m.M43, n.M43) && Are_Close(m.M44, n.M44);
    }

    /// <summary>
    /// Clamp
    /// </summary>: normalizes float from 0 to 1
    /// /// <param name="x"></param>
    /// <returns></returns>
    public static float Clamp(float x)
        => x / (1 + x);

    public static float ToRadians(float val)
        =>  (float) (val * Math.PI / 180.0);
}
