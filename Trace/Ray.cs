namespace Trace;

public struct Ray
{
    public Point Origin { get; set; }
    public Vec Dir { get; set; }
    private float TMin = 1e-5f;
    public float TMax = float.PositiveInfinity;
    public int Depth = 0;

    public Ray(Point p, Vec v, float? tmin = null)
    {
        Origin = p;
        Dir = v;
        if (tmin != null) TMin = (float)tmin;
    }

    public bool Is_Close(Ray b, float eps = 1e-5f)
         => Origin.Is_Close(b.Origin) && Dir.Is_Close(b.Dir);
    //Checks if two rays start from same origin and are parallel

    public Point At(float t) //returns the point reached by the ray at a distance t, measured in units length of Dir
        => Origin + Dir * t;


}
