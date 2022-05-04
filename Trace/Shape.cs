namespace Trace;

public abstract class Shape
{
    public Transformation Tr { get; set; }

    /// <summary>
    /// Create a shape, potentially associating a transformation to it
    /// </summary>
    /// <param name="t"></param>
    public Shape (Transformation? t = null)
    {
        Tr = t ?? Transformation.Identity();
    }

    /// <summary>
    /// Compute the intersection between a ray and this shape
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public abstract HitRecord? Ray_Intersection(Ray r);
    
    public Vec2D Sphere_Point_to_uv(Point p)
    {
        float u = (float) (Math.Atan2(p.Y, p.X) / (2.0 * Math.PI));
        if (u < 0)
        {
            u = u +1.0f;
        }
        var vec = new Vec2D(u, (float) Math.Acos(Math.Cos((p.Z)/Math.PI)));
        return vec;
    }

    public Normal Sphere_Normal(Point point, Vec ray_dir)
    {
        var result = new Normal(point.X, point.Y, point.Z);
        if (point.To_Vec().Dot(ray_dir) > 0.0f)
        {
            result = -result;
        }

        return result;
    }
    
    
}



