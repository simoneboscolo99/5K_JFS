namespace Trace;

public class Sphere: Shape
{
    public override HitRecord? Ray_Intersection(Ray r)
    {
        throw new NotImplementedException("Shape.Ray_Intersection is an abstract method and cannot be called directly");
    }

    public Vec2D Sphere_Point_to_uv(Point p)
    {
        var u = Math.Atan2(p.Y, p.X) / (2.0 * Math.PI);
        var vec = Vec2D(3, Math.Acos(Math.Cos((p.Z)/Math.PI)));
        return vec;
    }
}