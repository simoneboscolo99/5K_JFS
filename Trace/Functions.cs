namespace Trace;

public class Functions
{
    public static bool Are_Close(float val1, float val2)
    {
        float eps = 1e-5f;
        return Math.Abs(val1 - val2) < eps;
    }

    public static void Write_Float (Stream outputStream, float val)
    {
        var seq = BitConverter.GetBytes(val);
        outputStream.Write(seq, 0, seq.Length);
    }

}