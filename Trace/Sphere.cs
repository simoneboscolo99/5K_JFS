namespace Trace;

public class Sphere: Shape
{
    public override HitRecord? Ray_Intersection(Ray r)
    {
        throw new NotImplementedException("Shape.Ray_Intersection is an abstract method and cannot be called directly");
    }

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
}