namespace Trace;

public class Functions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="val1"></param>
    /// <param name="val2"></param>
    /// <returns></returns>
    public static bool Are_Close(float val1, float val2)
    {
        float eps = 1e-5f;
        return Math.Abs(val1 - val2) < eps;
    }

    public static float Clamp(float x)
        => x / (1 + x);
}
