namespace Trace;

public class Sphere : Shape
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
        // delta = 0 is equal to a tangent ray, unnecessary

        var sqrtDelta = (float)Math.Sqrt(delta);
        var tmin = (float)(-b - sqrtDelta) / (2.0f * a);
        var tmax = (-b + sqrtDelta) / (2.0f * a);
        float firstHitT;

        if ((tmin < invRay.TMax) && (tmin > invRay.TMin)) firstHitT = tmin;
        else if ((tmax < invRay.TMax) && (tmax > invRay.TMin)) firstHitT = tmax;
        else return null;

        var hitPoint = invRay.At(firstHitT);
        return new HitRecord(
            Tr * hitPoint,
            Tr * Sphere_Normal(hitPoint, invRay.Dir),
            firstHitT,
            r,
            Sphere_Point_to_uv(hitPoint));
    }
}