namespace Trace;

/// <summary>
/// A generic 3D shape. This is an abstract class, and you should only use it
/// to derive concrete classes. Be sure to redefine the method:
/// meth:`.Shape.ray_intersection`.
/// </summary>
public abstract class Shape
{
    public Transformation Tr { get; set; }
    public Material Mt { get; }

    /// <summary>
    /// Create a shape, potentially associating a transformation to it
    /// </summary>
    /// <param name="t"></param>
    /// <param name="material"></param>
    public Shape(Transformation? t = null, Material? material = null)
    {
        Tr = t ?? Transformation.Identity();
        Mt = material ?? new Material();
    }

    /// <summary>
    /// Compute the intersection between a ray and this shape
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public abstract HitRecord? Ray_Intersection(Ray r);

    public abstract List<HitRecord>? Ray_Intersection_List(Ray r);

    /// <summary>
    /// Determine whether a ray hits the shape or not
    /// </summary>
    /// <param name="r"> Ray. </param>
    /// <returns></returns>
    public abstract bool Quick_Ray_Intersection(Ray r);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public abstract bool Is_Internal(Point p);

    /// <summary>
    /// Convert a 3D point on the surface of the unit sphere into a (u, v) 2D point
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Vec2D Sphere_Point_to_uv(Point p)
    {
        var u = (float)(Math.Atan2(p.Y, p.X) / (2.0 * Math.PI));
        if (u < 0) u += 1.0f;
        var vec = new Vec2D(u, (float) Math.Acos(p.Z)/ (float) Math.PI );
        return vec;
    }

    /// <summary>
    /// Compute the normal of a unit sphere. The normal is computed for `point`
    /// (a point on the surface of the sphere), and it is chosen so that it is
    /// always in the opposite direction with respect to `ray_dir`.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="rayDir"></param>
    /// <returns></returns>
    public Normal Sphere_Normal(Point point, Vec rayDir)
    {
        var result = new Normal(point.X, point.Y, point.Z);
        if (point.To_Vec().Dot(rayDir) > 0.0f) result = -result;
        return result;
    }
}

// ===========================================================================
// === SPHERE ==== SPHERE ==== SPHERE ==== SPHERE ==== SPHERE ==== SPHERE ====
// ===========================================================================

/// <summary>
/// A 3D unit sphere centered on the origin of the axes
/// </summary>
public class Sphere : Shape
{
    /// <summary>
    /// Create a unit sphere, potentially associating a transformation to it
    /// </summary>
    /// <param name="T"></param>
    /// <param name="m"></param>
    public Sphere(Transformation? T = null, Material? m = null) : base(T,m) { }
    
    /// <summary>
    /// Checks if a ray intersects the sphere. Return a `HitRecord`, or `Null` if no intersection was found.
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
            Sphere_Point_to_uv(hitPoint),
            Mt
            );
    }

    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var originVec = invRay.Origin.To_Vec();
        var a = invRay.Dir.Squared_Norm();
        var b = 2.0f * originVec.Dot(invRay.Dir);
        var c = originVec.Squared_Norm() - 1.0f;

        var delta = b * b - 4.0f * a * c;
        
        if (delta <= 0.0f) return null;
        
        var sqrtDelta = (float)Math.Sqrt(delta);
        var tmin = (-b - sqrtDelta) / (2.0f * a);
        var tmax = (-b + sqrtDelta) / (2.0f * a);
        var intersections = new List<HitRecord>();
        var hitPoint1 = invRay.At(tmin);
        var hitPoint2 = invRay.At(tmax);
        if (tmin < invRay.TMax && tmin > invRay.TMin) {
            intersections.Add(new HitRecord(
                Tr * hitPoint1,
                Tr * Sphere_Normal(hitPoint1, invRay.Dir),
                tmin,
                r,
                Sphere_Point_to_uv(hitPoint1),
                Mt
            ));
        }

        if (tmax < invRay.TMax && tmax > invRay.TMin) {
            intersections.Add(new HitRecord(
                Tr * hitPoint2,
                Tr * Sphere_Normal(hitPoint2, invRay.Dir), 
                tmax, 
                r, 
                Sphere_Point_to_uv(hitPoint2), 
                Mt
                ));
        }

        return intersections.Count == 0 ? null : intersections;
    }

    /// <summary>
    /// Quickly checks if a ray intersects the sphere
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public override bool Quick_Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var originVec = invRay.Origin.To_Vec();
        var a = invRay.Dir.Squared_Norm();
        var b = 2.0f * originVec.Dot(invRay.Dir);
        var c = originVec.Squared_Norm() - 1.0f;

        var delta = b * b - 4.0f * a * c;
        if (delta <= 0.0f) return false;
        var sqrtDelta = Math.Sqrt(delta);
        var tmin = (-b - sqrtDelta) / (2.0 * a);
        var tmax = (-b + sqrtDelta) / (2.0 * a);
        
        return (tmin > invRay.TMin && tmin < invRay.TMax) || (tmax > invRay.TMin  && tmax < invRay.TMax);
    }

    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return p.To_Vec().Squared_Norm() < 1.0f;
    }
}

// ===========================================================================
// ==== PLANE === PLANE === PLANE === PLANE === PLANE === PLANE === PLANE ====
// ===========================================================================

/// <summary>
/// A 3D infinite plane parallel to the x and y axis and passing through the origin.
/// </summary>
public class Plane : Shape
{
    /// <summary>
    /// Create a xy plane, potentially associating a transformation to it
    /// </summary>
    /// <param name="T"></param>
    /// <param name="m"></param>
    public Plane(Transformation? T = null, Material? m = null) : base(T,m) { }

    /// <summary>
    /// Checks if a ray intersects the plane. Return a `HitRecord`, or `None` if no intersection was found.
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public override HitRecord? Ray_Intersection(Ray r)
    { 
        var invRay = Tr.Inverse * r;
        // Direction of the ray must not be parallel to the plane: z-component of dir different from zero (10^-5)
        if (Math.Abs(invRay.Dir.Z) < 1e-5f) return null;
        
        var t = -invRay.Origin.Z / invRay.Dir.Z;
        //return null if the ray run out of range
        if (t < invRay.TMin || t >= invRay.TMax) return null;

        var hitPoint = invRay.At(t);

        float dZ;
        if (invRay.Dir.Z < 0.0f) dZ = 1.0f;
        else dZ = -1.0f;

        var vec2d = new Vec2D(hitPoint.X - (float)Math.Floor(hitPoint.X), hitPoint.Y - (float)Math.Floor(hitPoint.Y));
        return new HitRecord(
            Tr * hitPoint,
            Tr * new Normal(0.0f, 0.0f, dZ),
            t,
            r,
            vec2d,
            Mt
        );
    }

    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var hit = Ray_Intersection(r);
        if (hit == null) return null;
        var intersections = new List<HitRecord> {hit};
        return intersections;
    }

    /// <summary>
    /// Quickly checks if a ray intersects the plane
    /// </summary>
    /// <param name="r"> Ray </param>
    /// <returns> True if there is intersection, otherwise false </returns>
    public override bool Quick_Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        if (Math.Abs(invRay.Dir.Z) < 1e-5) return false;
        var t = -invRay.Origin.Z / invRay.Dir.Z;
        return invRay.TMin < t && t < invRay.TMax;
    }
    
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return Functions.Are_Close(p.Z, 0.0f);
    }
}

// ===========================================================================
// ==== CYLINDER ==== CYLINDER ==== CYLINDER ==== CYLINDER ==== CYLINDER =====
// ===========================================================================

/// <summary>
/// 
/// </summary>
public class Cylinder : Shape
{
    /// <summary>
    /// Create a cylinder, potentially associating a transformation to it
    /// </summary>
    /// <param name="T"></param>
    /// <param name="m"></param>
    public Cylinder(Transformation? T = null, Material? m = null) : base(T, m) { }

    public override HitRecord? Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var a = invRay.Dir.X * invRay.Dir.X + invRay.Dir.Y * invRay.Dir.Y;
        var b = 2.0f * (invRay.Dir.X * invRay.Origin.X + invRay.Dir.Y * invRay.Origin.Y);
        // if radius = 1
        var c = invRay.Origin.X * invRay.Origin.X + invRay.Origin.Y * invRay.Origin.Y - 1.0f;
        
        var delta = b * b - 4.0f * a * c;
        if (delta <= 0.0f) return null;
        var sqrtDelta = (float)Math.Sqrt(delta);
        
        // solutions for an infinitely long cylinder
        var tmin = (-b - sqrtDelta) / (2.0f * a);
        var tmax = (-b + sqrtDelta) / (2.0f * a);
        float firstHitT;
        
        if (tmin < invRay.TMax && tmin > invRay.TMin && invRay.At(tmin).Z is > 0.0f and < 1.0f) firstHitT = tmin;
        else if (tmax < invRay.TMax && tmax > invRay.TMin && invRay.At(tmax).Z is > 0.0f and < 1.0f) firstHitT = tmax;
        else return null;
        
        var hitPoint = invRay.At(firstHitT);
        //if (hitPoint.Z > 1.0f || hitPoint.Z < 0.0f) return null;

        var phi = (float) Math.Atan2(hitPoint.Y, hitPoint.X);
        if (phi < 0) phi += 2.0f * (float) Math.PI;
        var u = phi / (2.0f * (float) Math.PI);
        var v = hitPoint.Z;
        var normal = new Normal(hitPoint.X, hitPoint.Y, 0.0f);
        if (normal.To_Vec().Dot(invRay.Dir) > 0.0f) normal = -normal;
        return new HitRecord(
            Tr * hitPoint,
            Tr * normal,
            firstHitT,
            r,
            new Vec2D(u ,v),
            Mt
        );
    }

    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var a = invRay.Dir.X * invRay.Dir.X + invRay.Dir.Y * invRay.Dir.Y;
        var b = 2.0f * (invRay.Dir.X * invRay.Origin.X + invRay.Dir.Y * invRay.Origin.Y);
        var c = invRay.Origin.X * invRay.Origin.X + invRay.Origin.Y * invRay.Origin.Y - 1.0f;
        var delta = b * b - 4.0f * a * c;
        if (delta <= 0.0f) return null;
        var sqrtDelta = (float)Math.Sqrt(delta);
        var tmin = (-b - sqrtDelta) / (2.0f * a);
        var tmax = (-b + sqrtDelta) / (2.0f * a);
        
        var intersections = new List<HitRecord>();
        
        var hitPoint1 = invRay.At(tmin);
        var phi1 = (float) Math.Atan2(hitPoint1.Y, hitPoint1.X);
        if (phi1 < 0) phi1 += 2.0f * (float) Math.PI;
        if (tmin < invRay.TMax && tmin > invRay.TMin && hitPoint1.Z is > 0.0f and < 1.0f)
        {
            var u = phi1 / (2.0f * (float) Math.PI);
            var v = hitPoint1.Z;
            var normal = new Normal(hitPoint1.X, hitPoint1.Y, 0.0f);
            if (normal.To_Vec().Dot(invRay.Dir) > 0.0f) normal = -normal;
            intersections.Add(new HitRecord(
                Tr * hitPoint1,
                Tr * normal,
                tmin,
                r,
                new Vec2D(u ,v),
                Mt));
        }
        
        var hitPoint2 = invRay.At(tmax);
        var phi2 = (float) Math.Atan2(hitPoint2.Y, hitPoint2.X);
        if (phi2 < 0) phi2 += 2.0f * (float) Math.PI;
        if (tmax < invRay.TMax && tmax > invRay.TMin && hitPoint2.Z is > 0.0f and < 1.0f)
        {
            var u = phi2 / (2.0f * (float) Math.PI);
            var v = hitPoint2.Z;
            var normal = new Normal(hitPoint2.X, hitPoint2.Y, 0.0f);
            if (normal.To_Vec().Dot(invRay.Dir) > 0.0f) normal = -normal;
            intersections.Add(new HitRecord(
                Tr * hitPoint2,
                Tr * normal,
                tmax,
                r,
                new Vec2D(u ,v),
                Mt));
        }

        return intersections.Count == 0 ? null : intersections;
    }

    public override bool Quick_Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var a = invRay.Dir.X * invRay.Dir.X + invRay.Dir.Y * invRay.Dir.Y;
        var b = 2.0f * (invRay.Dir.X * invRay.Origin.X + invRay.Dir.Y * invRay.Origin.Y);
        // if radius = 1
        var c = invRay.Origin.X * invRay.Origin.X + invRay.Origin.Y * invRay.Origin.Y - 1.0f;
        
        var delta = b * b - 4.0f * a * c;
        if (delta <= 0.0f) return false;
        var sqrtDelta = (float)Math.Sqrt(delta);
        
        // solutions for an infinitely long cylinder
        var tmin = (-b - sqrtDelta) / (2.0f * a);
        var tmax = (-b + sqrtDelta) / (2.0f * a);

        if (tmin > invRay.TMin && tmin < invRay.TMax)
        {
            var hitPoint1 = invRay.At(tmin);
            if (hitPoint1.Z is > 0.0f and < 1.0f) return true;
        }
        if (tmax > invRay.TMin && tmax < invRay.TMax)
        {
            var hitPoint2 = invRay.At(tmax);
            if (hitPoint2.Z is > 0.0f and < 1.0f) return true;
        }
        return false;
    }
    
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        var dist = p.X * p.X + p.Y * p.Y;
        return dist < 1.0f && p.Z is > 0.0f and < 1.0f;
    }
}

// ===========================================================================
// === DISK === DISK === DISK === DISK === DISK === DISK === DISK === DISK ===
// ===========================================================================

/// <summary>
/// 
/// </summary>
public class Disk : Shape
{

    public Disk(Transformation? T = null, Material? m = null) : base(T, m) { }

    public override HitRecord? Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        // Direction of the ray must not be parallel to the plane: z-component of dir different from zero (10^-5)
        if (Math.Abs(invRay.Dir.Z) < 1e-5f) return null;
        
        var t = -invRay.Origin.Z / invRay.Dir.Z;
        // rRturn null if the ray run out of range
        if (t < invRay.TMin || t >= invRay.TMax) return null;
        
        var hitPoint = invRay.At(t);
        // See if hit point is inside disk radii and phimax
        var dist = hitPoint.X * hitPoint.X + hitPoint.Y * hitPoint.Y;
        var phi = (float) Math.Atan2(hitPoint.Y, hitPoint.X);
        if (phi < 0) phi += 2.0f * (float) Math.PI;
        if (dist > 1.0f) return null;

        var u = phi / (2.0f * (float) Math.PI);
        var v = (1.0f - (float) Math.Sqrt(dist)) / 1.0f;

        float dZ;
        if (invRay.Dir.Z < 0.0f) dZ = 1.0f;
        else dZ = -1.0f;

        return new HitRecord(
            Tr * hitPoint,
            Tr * new Normal(0.0f, 0.0f, dZ),
            t,
            r,
            new Vec2D(u, v),
            Mt
        );
    }

    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var hit = Ray_Intersection(r);
        if (hit == null) return null;
        var intersections = new List<HitRecord> {hit};
        return intersections;
    }

    public override bool Quick_Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        if (Math.Abs(invRay.Dir.Z) < 1e-5) return false;
        var t = -invRay.Origin.Z / invRay.Dir.Z;
        var hitPoint = invRay.At(t);
        var dist = hitPoint.X * hitPoint.X + hitPoint.Y * hitPoint.Y;
        return !(dist > 1.0f) && invRay.TMin < t && t < invRay.TMax;
    }
    
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        var dist = p.X * p.X + p.Y * p.Y;
        return dist < 1.0f && Functions.Are_Close(p.Z, 0.0f);
    }
}

/// <summary>
/// 
/// </summary>
public class Box : Shape
{
    public Box(Transformation? T = null, Material? m = null) : base(T, m) { }

    public override HitRecord? Ray_Intersection(Ray r)
    {
        /*var invRay = Tr.Inverse * r;
        float txmin, txmax, tymin, tymax, tzmin, tzmax, tmin, tmax;
        
        // Resolve negative zero problem
        var invDirX = 1 / invRay.Dir.X;
        if (invDirX >= 0)
        {
            txmin = (-1.0f - invRay.Origin.X) * invDirX;
            txmax = (1.0f - invRay.Origin.X) * invDirX;
        }
        else
        {
            txmin = (1.0f - invRay.Origin.X) * invDirX;
            txmax = (-1.0f - invRay.Origin.X) * invDirX;
        }

        var invDirY = 1 / invRay.Dir.Y;
        if (invDirY >= 0)
        {
            tymin = (-1.0f - invRay.Origin.Y) * invDirY;
            tymax = (1.0f - invRay.Origin.Y) * invDirY;
        }
        else
        {
            tymin = (1.0f - invRay.Origin.Y) * invDirY;
            tymax = (-1.0f - invRay.Origin.Y) * invDirY;
        }

        if (txmax < tymin || tymax < txmin) return null;
        tmin = txmin > tymin ? txmin : tymin;
        tmax = txmax < tymax ? txmax : tymax;
        
        if (invRay.Dir.Z >= 0)
        {
            tzmin = (-1.0f - invRay.Origin.Z) / invRay.Dir.Z;
            tzmax = (1.0f - invRay.Origin.Z) / invRay.Dir.Z;
        }
        else
        {
            tzmin = (1.0f - invRay.Origin.Z) / invRay.Dir.Z;
            tzmax = (-1.0f - invRay.Origin.Z) / invRay.Dir.Z;
        }

        if (tmax < tzmin || tzmax < tmin) return null;
        if (tzmin > tmin) tmin = tzmin;
        if (tzmax < tmax) tmax = tzmax;
        
        float firstHitT;
        if (tmin < invRay.TMax && tmin > invRay.TMin) firstHitT = tmin;
        else if (tmax < invRay.TMax && tmax > invRay.TMin) firstHitT = tmax;
        else return null;
        var hitPoint = invRay.At(firstHitT);

        return new HitRecord(
            Tr * hitPoint,
            Tr * new Normal(0.0f, 0.0f, dZ),
            tmin,
            r,
            new Vec2D(u, v),
            Mt
        ); */
        return null;

    }

    public override bool Quick_Ray_Intersection(Ray r)
    {
        return false;
    }

    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        return null;
    }
    
    public override bool Is_Internal(Point p)
        => p.X is < 1.0f and > -1.0f && p.Y is < 1.0f and > -1.0f && p.Z is < 1.0f and > -1.0f;
}