using System.Xml.Schema;
using SixLabors.ImageSharp.Processing;

namespace Trace;

// Abstract class
public abstract class Solver
{
    public World Wd;
    public Color BackGround;
    
    // Abstract method
    public Solver(World ? world = null, Color ? color = null)
    {
        Wd = world ?? new World();
        BackGround = color ?? Color.Black;
    }

    public abstract Color Tracing(Ray ray);
}

public class SameColor : Solver
{
    public SameColor(World? world = null, Color? color = null)
        : base(world, color) {}

    public override Color Tracing(Ray ray)
    {
        var color = new Color(1.0f, 2.0f, 3.0f);
        return color;
    }


}

public class OnOffTracing : Solver
{
    public Color ObjectColor;
    public OnOffTracing(World world, Color? background = null, Color? objects = null) : base(world, background)
    {
        ObjectColor = objects ?? Color.White;
    }
    
    public override Color Tracing(Ray ray)
    {
        return Wd.Ray_Intersection(ray) != null ? ObjectColor : BackGround;
    }
}

public class FlatTracing : Solver
{
    public FlatTracing(World world, Color? background) : base(world, background) {}
    public override Color Tracing(Ray ray)
    {
        var hit = Wd.Ray_Intersection(ray);
        if (hit == null) return BackGround;

        var material = hit.Mt;

        if (material.BRdf?.Pg != null)
            return (material.BRdf.Pg.Get_Color(hit.SurfacePoint) +
                    material.EmittedRadiance.Get_Color(hit.SurfacePoint));
        return default;
    }


}


