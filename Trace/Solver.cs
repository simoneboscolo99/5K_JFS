namespace Trace;

// Abstract class
public abstract class Solver
{
    
    // Abstract method
    public abstract Color Tracing(Ray ray);

    // Abstract properties
    
}

public class SameColor : Solver
{
    public override Color Tracing(Ray ray)
    {
        var color = new Color(1.0f, 2.0f, 3.0f);
        return color;
    }
}


