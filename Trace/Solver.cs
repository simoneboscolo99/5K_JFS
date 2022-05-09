using System.Xml.Schema;
using SixLabors.ImageSharp.Processing;

namespace Trace;

// Abstract class
public abstract class Solver
{
    // Abstract method
    public abstract Color Tracing(Ray ray);
}

public class SameColor : Solver
{
    public override Color Tracing(Ray ray)
    {
        var color = new Color(1.0f, 2.0f, 3.0f);
        return color;
    }
}

public class OnOffTracing : Solver
{
    public World World;
    public Color BackgroundColor;
    public Color ObjectColor;
    public OnOffTracing(World world, Color? background = null, Color? objects = null)
    {
        World = world;
        BackgroundColor = background ?? Color.Black;
        ObjectColor = objects ?? Color.White;
    }
    
    public override Color Tracing(Ray ray)
    {
        return World.ray_intersection(ray) != null ? Color.White : Color.Black;
    }
}


