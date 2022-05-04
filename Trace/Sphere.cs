namespace Trace;

public class Sphere: Shape
{

    public Sphere(Transformation? T = null)
        : base(T) { }

    /// <summary>
    /// Checks if a ray intersects the sphere. Return a `HitRecord`, or `None` if no intersection was found.
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public override HitRecord? Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var originVec = invRay.Origin.To_Vec();
        var a = invRay.Dir.Squared_Norm();
        var b = 2.0f * originVec.Dot(invRay.Dir);
        var c = originVec.Squared_Norm() - 1.0f;
        var delta = b * b - 4.0f * a * c;

        if (delta <= 0.0f) return null;
        
        var sqrtDelta = (float) Math.Sqrt(delta);
        var tmin = (-b - sqrtDelta) / (2.0f * a);
        var tmax = (-b + sqrtDelta) / (2.0f * a);
        float firstHitT;
        
        if ((tmin < invRay.TMax) && (tmin > invRay.TMin)) firstHitT = tmin;
        else if ((tmax < invRay.TMax) && (tmax > invRay.TMin)) firstHitT = tmax;
        else return null;

        var hitPoint = invRay.At(firstHitT);
        return new HitRecord(
            Tr * hitPoint,
            Tr * Sphere_Normal(hitPoint, invRay.Dir),
            Sphere_Point_to_uv(hitPoint),
            firstHitT,
            r);
    }


    public Vec2D Sphere_Point_to_uv(Point p)
    {
        var u = Math.Atan2(p.Y, p.X) / (2.0 * Math.PI);
        var vec = new Vec2D(3, (float) Math.Acos(Math.Cos((p.Z)/Math.PI)));
        return vec;
    }
}