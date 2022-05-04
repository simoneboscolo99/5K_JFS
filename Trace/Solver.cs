namespace Trace;

// Abstract class
public abstract class Solver
{

    // Abstract method
    public abstract Color AbstractMethod();

    // Abstract properties

}

public class DerivedClass : Solver
{
    public override Color AbstractMethod()
    {
        var color = new Color(1.0f, 2.0f, 3.0f);
        return color;
    }
}


