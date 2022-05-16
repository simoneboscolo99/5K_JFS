namespace Trace;

// Abstract class
public abstract class Solver
{
    public World Wd;
    public Color BackgroundColor;

    public Solver(World? world = null, Color? backgroundColor = null)
    {
        Wd = world ?? new World();
        BackgroundColor = backgroundColor ?? Color.Black; 
    }
    
    // Abstract method
    public abstract Color Tracing(Ray ray);
}

public class SameColor : Solver
{
    public SameColor(World? world = null, Color? backgroundColor = null) : base(world, backgroundColor){}
    public override Color Tracing(Ray ray)
    {
        var color = new Color(1.0f, 2.0f, 3.0f);
        return color;
    }
}

public class OnOffTracing : Solver
{
    public Color ObjectColor;
    public OnOffTracing(World? world = null , Color? background = null, Color? objects = null) : base(world, background)
    {
        Wd = world ?? new World() ;
        BackgroundColor = background ?? Color.Black;
        ObjectColor = objects ?? Color.White;
    }
    
    public override Color Tracing(Ray ray)
    {
        return Wd.Ray_Intersection(ray) != null ? ObjectColor : BackgroundColor;
    }
}


/// <summary>
/// A «flat» renderer. <br/>
/// This renderer estimates the solution of the rendering equation by neglecting any contribution of the light. <br/>
/// It just uses the pigment of each surface to determine how to compute the final radiance.
/// </summary>
public class FlatTracing : Solver
{
    public FlatTracing(World? world = null, Color? background = null) : base(world, background) {}

    public override Color Tracing(Ray ray)
    {
        var hit = Wd.Ray_Intersection(ray);
        if (hit == null) return BackgroundColor;
        var material = hit.Mt;
        
        return (material.Brdf.Pg.Get_Color(hit.SurfacePoint) +
                material.EmittedRadiance.Get_Color(hit.SurfacePoint));
    }
}
