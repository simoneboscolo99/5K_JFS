namespace Trace;

public class Functions
{
    public static bool Are_Close(float value1, float value2)
    {
        float eps = 1e-5f;
        return Math.Abs(value1 - value2) < eps;
    }

}