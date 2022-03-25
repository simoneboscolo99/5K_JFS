namespace Trace;

public class Functions
{
    
    
    /// <summary>
    /// Are_Close
    /// </summary>: : Returns true if |a-b| is smaller than 10^-5
    /// <param name="val1"></param>
    /// <param name="val2"></param>
    /// <returns></returns>
    

    public static bool Are_Close(float val1, float val2)
    {
        float eps = 1e-5f;
        return Math.Abs(val1 - val2) < eps;
    }

    /// <summary>
    /// Clamp
    /// </summary>: Normalizes float from 0 to 1
    /// /// <param name="x"></param>
    /// <returns></returns>
    public static float Clamp(float x)
        => x / (1 + x);
}
