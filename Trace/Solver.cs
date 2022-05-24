namespace Trace;

/// <summary>
/// A class implementing a solver of the rendering equation. <br/>
/// This is an abstract class; you should use a derived concrete class.
/// </summary>
public abstract class Solver
{
    /// <summary>
    /// 
    /// </summary>
    public World Wd;
    
    /// <summary>
    /// colore del background (è il colore che ho se il raggio non colpisce niente): utile per debugging
    /// </summary>
    public Color BackgroundColor;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="world"></param>
    /// <param name="backgroundColor"></param>
    public Solver(World? world = null, Color? backgroundColor = null)
    {
        Wd = world ?? new World();
        BackgroundColor = backgroundColor ?? Color.Black; 
    }
    
    /// <summary>
    /// Estimate the radiance along a ray.
    /// </summary>
    /// <param name="ray"> The ray. </param>
    /// <returns> The color ... </returns>
    public abstract Color Tracing(Ray ray);
}

/// <summary>
///
/// This renderer is mostly useful for debugging purposes.
/// </summary>
public class SameColor : Solver
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="world"></param>
    /// <param name="backgroundColor"></param>
    public SameColor(World? world = null, Color? backgroundColor = null) : base(world, backgroundColor){}
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public override Color Tracing(Ray ray)
    {
        var color = new Color(1.0f, 2.0f, 3.0f);
        return color;
    }
}

/// <summary>
/// A on/off renderer. <br/>
/// This renderer is mostly useful for debugging purposes, as it is really fast, but it produces boring images.
/// </summary>
public class OnOffTracing : Solver
{
    /// <summary>
    /// 
    /// </summary>
    public Color ObjectColor;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="world"></param>
    /// <param name="background"></param>
    /// <param name="objects"></param>
    public OnOffTracing(World? world = null , Color? background = null, Color? objects = null) : base(world, background)
    {
        Wd = world ?? new World() ;
        BackgroundColor = background ?? Color.Black;
        ObjectColor = objects ?? Color.White;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="world"></param>
    /// <param name="background"></param>
    public FlatTracing(World? world = null, Color? background = null) : base(world, background) {}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public override Color Tracing(Ray ray)
    {
        var hit = Wd.Ray_Intersection(ray);
        if (hit == null) return BackgroundColor;
        var material = hit.Mt;
        
        return (material.Brdf.Pg.Get_Color(hit.SurfacePoint) +
                material.EmittedRadiance.Get_Color(hit.SurfacePoint));
    }
}

/// <summary>
/// A simple path-tracing renderer. <br/>
/// The algorithm implemented here allows the caller to tune number of rays thrown at each iteration, as well as the
/// maximum depth. It implements Russian roulette, so in principle it will take a finite time to complete the
/// calculation even if you set <see cref="MaxDepth"/> to <see cref="float.PositiveInfinity"/>.
/// </summary>
public class PathTracing : Solver
{
    /// <summary>
    /// generatore di numeri casuali
    /// </summary>
    public Pcg Pcg;
    
    /// <summary>
    /// numero di raggi da generare
    /// </summary>
    public int NumOfRays;
    
    /// <summary>
    /// profondità massima dei raggi (poi mi fermo): utile per debugging
    /// </summary>
    public int MaxDepth;

    /// <summary>
    /// limite per la profondità oltre il quale usare la Roulette russa
    /// </summary>
    public int RussianRouletteLimit;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="world"></param>
    /// <param name="background"></param>
    /// <param name="pcg"></param>
    /// <param name="numOfRays"></param>
    /// <param name="maxDepth"></param>
    /// <param name="russianRouletteLimit"></param>
    public PathTracing(World? world = null, Color? background = null, Pcg? pcg = null, int numOfRays = 10, int maxDepth = 2, int russianRouletteLimit = 3) : base(world, background)
    {
        Pcg = pcg ?? new Pcg();
        NumOfRays = numOfRays;
        MaxDepth = maxDepth;
        RussianRouletteLimit = russianRouletteLimit;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public override Color Tracing(Ray ray)
    {
        if (ray.Depth > MaxDepth) return Color.Black;

        var hitRecord = Wd.Ray_Intersection(ray);
        if (hitRecord == null) return BackgroundColor;
        
        var hitMaterial = hitRecord.Mt;
        var hitColor = hitMaterial.Brdf.Pg.Get_Color(hitRecord.SurfacePoint);
        var emittedRadiance = hitMaterial.EmittedRadiance.Get_Color(hitRecord.SurfacePoint);

        var hitColorLum = Math.Max(hitColor.R, Math.Max(hitColor.G, hitColor.B));
        
        // Russian roulette
        
    }
    
}