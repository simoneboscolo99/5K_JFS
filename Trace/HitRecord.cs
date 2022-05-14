namespace Trace;

/// <summary>
///  A class holding information about a ray-shape intersection
/// The parameters defined in this dataclass are the following:
/// -   `world_point`: a :class:`.Point` object holding the world coordinates of the hit point
/// -   `normal`: a :class:`.Normal` object holding the orientation of the normal to the surface where the hit happened
/// -   `surface_point`: a :class:`.Vec2d` object holding the position of the hit point on the surface of the object
/// -   `t`: a floating-point value specifying the distance from the origin of the ray where the hit happened
/// -   `ray`: the ray that hit the surface
/// </summary>
public class HitRecord
{
    public Point WorldPoint { get; set; }
    public Normal Normal { get; set; }
    public float T { get; set; }
    public Ray Ray { get; set; }
    public Vec2D SurfacePoint { get; set; }
    public Material Mt { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="wp"> World point </param>
    /// <param name="nm"> Normal </param>
    /// <param name="t"> T </param>
    /// <param name="ray"> Ray </param>
    /// <param name="sp"> Surface Point </param>
    /// <param name="material"></param>
    public HitRecord(Point wp, Normal nm, float t, Ray ray, Vec2D sp, Material material)
    {
        WorldPoint = wp;
        Normal = nm;
        T = t;
        Ray = ray;
        SurfacePoint = sp;
        Mt = material;
    }

    /// <summary>
    /// Check whether two `HitRecord` represent the same hit event or not
    /// </summary>
    /// <param name="hr"></param>
    /// <returns></returns>
    public bool Is_Close(HitRecord? hr = null)
    {
        if (hr == null) return false;
        return WorldPoint.Is_Close(hr.WorldPoint) && Normal.Is_Close(hr.Normal) && Functions.Are_Close(T, hr.T) &&
               Ray.Is_Close(hr.Ray) && SurfacePoint.Is_Close(hr.SurfacePoint);
    }

}