namespace Trace;

public class Functions
{
    public static bool Are_Close(float val1, float val2)
    {
        float eps = 1e-5f;
        return Math.Abs(val1 - val2) < eps;
    }

}

