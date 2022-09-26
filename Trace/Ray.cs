namespace Trace;

public struct Ray
{
    public Point Origin { get; set; }
    public Vec Dir { get; set; }
    public float TMin = 1e-5f;
    public float TMax = float.PositiveInfinity;
    public int Depth = 0;

    public Ray(Point o, Vec dir, float? tmin = null, float? tmax = null, int? depth = null)
    {
        Origin = o;
        Dir = dir;
        if (tmin != null) TMin = (float)tmin;
        if (tmax != null) TMax = (float)tmax;
        if (depth != null) Depth = (int) depth;
    }
    

    public bool Is_Close(Ray b, float eps = 1e-5f)
        => Origin.Is_Close(b.Origin) && Dir.Is_Close(b.Dir);
    //Checks if two rays start from same origin and are parallel

    public Point At(float t) //returns the point reached by the ray at a distance t, measured in units length of Dir
        => Origin + Dir * t;
    
}