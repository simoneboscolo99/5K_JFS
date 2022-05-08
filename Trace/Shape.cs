namespace Trace;

public abstract class Shape
{
    public Transformation Tr { get; set; }

    /// <summary>
    /// Create a shape, potentially associating a transformation to it
    /// </summary>
    /// <param name="t"></param>
    public Shape(Transformation? t = null)
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
        float u = (float) (Math.Atan2(p.Y, p.X) / (2.0f * Math.PI));
        if (u < 0.0f) u = u + 1.0f;
        var vec = new Vec2D(u, (float) (Math.Acos(p.Z) / Math.PI));
        return vec;
    }

    public Normal Sphere_Normal(Point point, Vec rayDir)
    {
        var result = new Normal(point.X, point.Y, point.Z);
        if (point.To_Vec().Dot(rayDir) > 0.0f)
        {
            result = -result;
        }

        return result;
    }
}

public class Sphere : Shape
{
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="T"></param>
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
            var tmin = (-b - sqrtDelta) / (2.0f * a);
            var tmax = (-b + sqrtDelta) / (2.0f * a);
            float firstHitT;

            if (tmin < invRay.TMax && tmin > invRay.TMin) firstHitT = tmin;
            else if (tmax < invRay.TMax && tmax > invRay.TMin) firstHitT = tmax;
            else return null;

            var hitPoint = invRay.At(firstHitT);
            // point of intersection and normal are calculated in the sphere's frame of reference
            // I have to go back to the world frame by direct transformation
            return new HitRecord(
                Tr * hitPoint,
                Tr * Sphere_Normal(hitPoint, invRay.Dir),
                firstHitT,
                r,
                Sphere_Point_to_uv(hitPoint));
        }
    
}