namespace Trace;

public class HitRecord
{
    public Point WorldPoint { get; set; }
    public Normal Normal { get; set; }
    public float T { get; set; }
    public Ray Ray { get; set; }
    public Vec2D SurfacePoint { get; set; }

    public HitRecord(Point wp, Normal nm, float t, Ray ray, Vec2D sp)
    {
        WorldPoint = wp;
        Normal = nm;
        T = t;
        Ray = ray;
        SurfacePoint = sp;
    }

    public bool Is_Close(HitRecord? hr = null)
    {
        if (hr == null) return false;
        return WorldPoint.Is_Close(hr.WorldPoint) && Normal.Is_Close(hr.Normal) && Functions.Are_Close(T, hr.T) && Ray.Is_Close(hr.Ray) && SurfacePoint.Is_Close(hr.SurfacePoint);
    }

}