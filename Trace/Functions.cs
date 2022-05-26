using System.Numerics;

namespace Trace;

/// <summary>
/// 
/// </summary>
public class Functions
{

    /// <summary>
    /// Returns a value indicating whether two floating-point numbers represent roughly the same value. 
    /// </summary>
    /// <param name="val1"> The first number. </param>
    /// <param name="val2"> The second number. </param>
    /// <param name="eps"> The precision. </param>
    /// <returns> true if the two numbers differ by less than <see cref="eps"/>; otherwise, false. </returns>
    public static bool Are_Close(float val1, float val2, float eps = 1e-5f)
    {
        return Math.Abs(val1 - val2) < eps;
    }

    /// <summary>
    /// Returns a value indicating whether two 4x4 matrices represent roughly the same matrix. 
    /// </summary>
    /// <param name="m"> The first matrix. </param>
    /// <param name="n"> The second matrix. </param>
    /// <param name="eps"> The precision. </param>
    /// <returns> true if the corresponding elements of each matrix differ by less than <see cref="eps"/>; otherwise, false. </returns>
    public static bool Are_Matrices_close(Matrix4x4 m, Matrix4x4 n, float eps = 1e-5f)
    {
        return Are_Close(m.M11, n.M11, eps) && Are_Close(m.M12, n.M12, eps) && Are_Close(m.M13, n.M13, eps) && Are_Close(m.M14, n.M14, eps) &&
               Are_Close(m.M21, n.M21, eps) && Are_Close(m.M22, n.M22, eps) && Are_Close(m.M23, n.M23, eps) && Are_Close(m.M24, n.M24, eps) &&
               Are_Close(m.M31, n.M31, eps) && Are_Close(m.M32, n.M32, eps) && Are_Close(m.M33, n.M33, eps) && Are_Close(m.M34, n.M34, eps) &&
               Are_Close(m.M41, n.M41, eps) && Are_Close(m.M42, n.M42, eps) && Are_Close(m.M43, n.M43, eps) && Are_Close(m.M44, n.M44, eps);
    }

    /// <summary>
    /// Converts a floating-point number into a normalized floating-point number between 0 and 1.
    /// </summary>
    /// <param name="x"> The number. </param>
    /// <returns> Clamped number. </returns>
    public static float Clamp(float x)
        => x / (1 + x);

    /// <summary>
    /// Converts an angle (floating-point number) in degrees to an angle (floating-point number) in radians.
    /// </summary>
    /// <param name="angleDeg"> The angle in degree. </param>
    /// <returns> The angle in radians. </returns>
    public static float ToRadians(float angleDeg)
        => (float)(angleDeg * Math.PI / 180.0);
}
