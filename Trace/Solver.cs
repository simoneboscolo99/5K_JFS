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
        return World.Ray_Intersection(ray) != null ? ObjectColor : BackgroundColor;
    }
}

/// <summary>
/// A «flat» renderer.
/// </summary>
/// This renderer estimates the solution of the rendering equation
///by neglecting any contribution of the light. It just uses the pigment of each surface
///to determine how to compute the final radiance.
public class FlatTracing : Solver
{
    public World World;
    public Color BackgroundColor;
    
    public FlatTracing(World world, Color? background)
    {
        World = world;
        BackgroundColor = background ?? Color.Black;
    }
    
    public override Color Tracing(Ray ray)
    {
        var hit = World.Ray_Intersection(ray);
        if (hit == null) return BackgroundColor;
        var material = hit.Material;
        return (material.Brdf.Pigment.Get_Color(hit.SurfacePoint) +
                Material.EmittedRadiance.Get_Color(hit.SurfacePoint));

    }
}

