namespace Trace;

/// <summary>
/// A generic 3D shape. This is an abstract class, and you should only use it to derive concrete classes:
/// <see cref="Sphere"/>, <see cref="Plane"/>, <see cref="Cylinder"/>, <see cref="Disk"/>, <see cref="Box"/>.
/// </summary>
public abstract class Shape
{
    /// <summary>
    /// The transformation associated to the shape.
    /// </summary>
    public Transformation Tr { get; set; }
    
    /// <summary>
    /// The material of the shape.
    /// </summary>
    protected Material Mt { get; }

    /// <summary>
    /// Shape constructor. Initialize a new instance of the <see cref="Shape"/> class, potentially associating a transformation to it.
    /// This is an abstract constructor, therefore it cannot be directly used in the code.
    /// </summary>
    /// <param name="t"> The transformation associated to the shape.</param>
    /// <param name="material"> The material of the shape.</param>
    protected Shape(Transformation? t = null, Material? material = null)
    {
        Tr = t ?? Transformation.Identity();
        Mt = material ?? new Material();
    }

    /// <summary>
    /// Compute the intersection between a ray and this shape.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The <see cref="HitRecord"/> containing all information about the intersection.
    /// If no intersection is found, <see langword="null"/> is returned. </returns>
    public abstract HitRecord? Ray_Intersection(Ray r);

    /// <summary>
    /// Compute the list of all intersections between a ray and this shape. This method is necessary for the implementation of CSG
    /// (Constructive Solid Geometry).
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The list of <see cref="HitRecord"/> of intersections, ordered by the distance from the origin of the ray.
    /// If no intersection is found, <see langword="null"/> is returned. </returns>
    public abstract List<HitRecord>? Ray_Intersection_List(Ray r);

    /// <summary>
    /// Determine whether a ray hits the shape or not.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> True is there is an intersection, false otherwise. </returns>
    public abstract bool Quick_Ray_Intersection(Ray r);

    /// <summary>
    /// Determine whether the given point is inside the shape or not.
    /// </summary>
    /// <param name="p"> The point. </param>
    /// <returns> True if the point is inside the shape, false otherwise. </returns>
    public abstract bool Is_Internal(Point p);
    
    /// <summary>
    /// Computes the union (CSG) between two shapes.
    /// </summary>
    /// <param name="s1"> The first shape. </param>
    /// <param name="s2"> The second shape. </param>
    /// <returns> The union shape: <see cref="CsgUnion"/>. </returns>           
    public static CsgUnion operator +(Shape s1, Shape s2)
        => new(s1, s2);
    
    /// <summary>
    /// Computes the difference (CSG) between two shapes.
    /// </summary>
    /// <param name="s1"> The first shape. </param>
    /// <param name="s2"> The second shape. </param>
    /// <returns> The difference shape: <see cref="CsgDifference"/>. </returns>
    public static CsgDifference operator -(Shape s1, Shape s2)
        => new(s1, s2);
    
    /// <summary>
    /// Computes intersection (CSG) between two shapes.
    /// </summary>
    /// <param name="s1"> The first shape. </param>
    /// <param name="s2"> The second shape. </param>
    /// <returns> The intersection shape: <see cref="CsgIntersection"/>. </returns>
    public static CsgIntersection operator *(Shape s1, Shape s2)
        => new (s1, s2);
}

// ===========================================================================
// === SPHERE ==== SPHERE ==== SPHERE ==== SPHERE ==== SPHERE ==== SPHERE ====
// ===========================================================================

/// <summary>
/// A 3D unit sphere centered on the origin of the axes.
/// </summary>
public class Sphere : Shape
{
    /// <summary>
    /// Sphere constructor. Initialize a new instance of the <see cref="Sphere"/> class, potentially associating a transformation to it.
    /// </summary>
    /// <param name="T"> The transformation associated to the sphere. </param>
    /// <param name="m"> The material of the sphere. </param>
    public Sphere(Transformation? T = null, Material? m = null) : base(T,m) { }
    
    /// <summary>
    /// Convert a 3D point on the surface of the unit sphere into a (u, v) 2D point.
    /// </summary>
    /// <param name="p"> The 3D point. </param>
    /// <returns> A <see cref="Vec2D"/> containing the coordinates of the surface point of the sphere. </returns>
    private Vec2D Sphere_Point_To_UV(Point p)
    {
        var u = (float)(Math.Atan2(p.Y, p.X) / (2.0 * Math.PI));
        if (u < 0) u += 1.0f;
        var vec = new Vec2D(u, (float) Math.Acos(p.Z)/ (float) Math.PI );
        return vec;
    }

    /// <summary>
    /// Compute the normal of a unit sphere. The normal is computed for <paramref name="point"/> (a point on the surface of
    /// the sphere), and it is chosen so that it is always in the opposite direction with respect to <paramref name="rayDir"/>.
    /// </summary>
    /// <param name="point"> The point. </param>
    /// <param name="rayDir"> The direction of the ray. </param>
    /// <returns> The normal. </returns>
    private Normal Sphere_Normal(Point point, Vec rayDir)
    {
        var result = new Normal(point.X, point.Y, point.Z);
        if (point.To_Vec().Dot(rayDir) > 0.0f) result = -result;
        return result;
    }
    
    /// <summary>
    /// Check if a ray intersects the sphere.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The <see cref="HitRecord"/>, or <see langword="null"/> if no intersection was found. </returns>
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
            Sphere_Point_To_UV(hitPoint),
            Mt
            );
    }

    /// <summary>
    /// Check if a ray intersects the sphere by computing a list of all possible intersections.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The list of <see cref="HitRecord"/> of intersections, ordered by the distance from the origin of the ray.
    /// If no intersection is found, <see langword="null"/> is returned. </returns>
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
                Sphere_Point_To_UV(hitPoint1),
                Mt
            ));
        }

        if (tmax < invRay.TMax && tmax > invRay.TMin) {
            intersections.Add(new HitRecord(
                Tr * hitPoint2,
                Tr * Sphere_Normal(hitPoint2, invRay.Dir), 
                tmax, 
                r, 
                Sphere_Point_To_UV(hitPoint2), 
                Mt
                ));
        }

        return intersections.Count == 0 ? null : intersections;
    }

    /// <summary>
    /// Quickly check if a ray intersects the sphere.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> True if there is intersection, false otherwise. </returns>
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

    /// <summary>
    /// Check if a point is inside the sphere.
    /// </summary>
    /// <param name="p"> The point. </param>
    /// <returns> True if the point is inside the sphere, false otherwise. </returns>
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return p.To_Vec().Squared_Norm() < 1.0f;
    }
}

// ===========================================================================
// === END === END === END === END === END === END === END === END === END ===
// ===========================================================================

// ===========================================================================
// ==== PLANE === PLANE === PLANE === PLANE === PLANE === PLANE === PLANE ====
// ===========================================================================

/// <summary>
/// A 2D infinite plane parallel to the x and y axis and passing through the origin.
/// </summary>
public class Plane : Shape
{
    /// <summary>
    /// Plane constructor. Initialize a new instance of the <see cref="Plane"/> class, potentially associating a transformation to it.
    /// </summary>
    /// <param name="T"> The transformation associated to the plane. </param>
    /// <param name="m"> The material of the plane. </param>
    public Plane(Transformation? T = null, Material? m = null) : base(T,m) { }

    /// <summary>
    /// Check if a ray intersects the plane.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The <see cref="HitRecord"/>, or <see langword="null"/> if no intersection was found. </returns>
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

    /// <summary>
    /// Check if a ray intersects the plane by computing a list of all possible intersections.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The list of <see cref="HitRecord"/> of intersections, ordered by the distance from the origin of the ray.
    /// If no intersection is found, <see langword="null"/> is returned. </returns>
    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var hit = Ray_Intersection(r);
        if (hit == null) return null;
        var intersections = new List<HitRecord> {hit};
        return intersections;
    }
    
    /// <summary>
    /// Quickly checks if a ray intersects the plane.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> True if there is intersection, false otherwise. </returns>
    public override bool Quick_Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        if (Math.Abs(invRay.Dir.Z) < 1e-5) return false;
        var t = -invRay.Origin.Z / invRay.Dir.Z;
        return invRay.TMin < t && t < invRay.TMax;
    }
   
    /// <summary>
    /// Determine whether the given point belongs the plane or not.
    /// </summary>
    /// <param name="p"> The point. </param>
    /// <returns> True if the point lies on the plane, false otherwise. </returns>
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return Functions.Are_Close(p.Z, 0.0f);
    }
}

// ===========================================================================
// === END === END === END === END === END === END === END === END === END ===
// ===========================================================================

// ===========================================================================
// ==== CYLINDER ==== CYLINDER ==== CYLINDER ==== CYLINDER ==== CYLINDER =====
// ===========================================================================

/// <summary>
/// A 3D open cylinder centered around the z axis with unit radius. The z-coordinate range is 0 to 1.
/// </summary>
public class Cylinder : Shape
{
    /// <summary>
    /// Cylinder constructor. Initialize a new instance of the <see cref="Cylinder"/> class, potentially associating a transformation to it.
    /// </summary>
    /// <param name="T"> The transformation associated to the cylinder. </param>
    /// <param name="m"> The material of the cylinder. </param>
    public Cylinder(Transformation? T = null, Material? m = null) : base(T, m) { }

    /// <summary>
    /// Check if a ray intersects the cylinder.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The <see cref="HitRecord"/>, or <see langword="null"/> if no intersection was found. </returns>
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

    /// <summary>
    /// Check if a ray intersects the cylinder by computing a list of all possible intersections.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The list of <see cref="HitRecord"/> of intersections, ordered by the distance from the origin of the ray.
    /// If no intersection is found, <see langword="null"/> is returned. </returns>
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

    /// <summary>
    /// Quickly checks if a ray intersects the cylinder.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> True if there is intersection, false otherwise. </returns>
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
  
    /// <summary>
    /// Check if a point is inside the cylinder.
    /// </summary>
    /// <param name="p"> The point. </param>
    /// <returns> True if the point is inside the cylinder, false otherwise. </returns>
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        var dist = p.X * p.X + p.Y * p.Y;
        return dist < 1.0f && p.Z is > 0.0f and < 1.0f;
    }
}

// ===========================================================================
// === END === END === END === END === END === END === END === END === END ===
// ===========================================================================

// ===========================================================================
// === DISK === DISK === DISK === DISK === DISK === DISK === DISK === DISK ===
// ===========================================================================

/// <summary>
/// A 2D circular disk of unit radius, parallel to the x and y axis and passing through the origin.
/// </summary>
public class Disk : Shape
{

    /// <summary>
    /// Disk constructor. Initialize a new instance of the <see cref="Disk"/> class, potentially associating a transformation to it.
    /// </summary>
    /// <param name="T"> The transformation associated to the disk. </param>
    /// <param name="m"> The material of the disk. </param>
    public Disk(Transformation? T = null, Material? m = null) : base(T, m) { }

    /// <summary>
    /// Check if a ray intersects the disk.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The <see cref="HitRecord"/>, or <see langword="null"/> if no intersection was found. </returns>
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

    /// <summary>
    /// Check if a ray intersects the disk by computing a list of all possible intersections.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The list of <see cref="HitRecord"/> of intersections, ordered by the distance from the origin of the ray.
    /// If no intersection is found, <see langword="null"/> is returned. </returns>
    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var hit = Ray_Intersection(r);
        if (hit == null) return null;
        var intersections = new List<HitRecord> {hit};
        return intersections;
    }

    /// <summary>
    /// Quickly checks if a ray intersects the disk.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> True if there is intersection, false otherwise. </returns>
    public override bool Quick_Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        if (Math.Abs(invRay.Dir.Z) < 1e-5) return false;
        var t = -invRay.Origin.Z / invRay.Dir.Z;
        var hitPoint = invRay.At(t);
        var dist = hitPoint.X * hitPoint.X + hitPoint.Y * hitPoint.Y;
        return !(dist > 1.0f) && invRay.TMin < t && t < invRay.TMax;
    }
    
    /// <summary>
    /// Determine whether the given point belongs the disk or not.
    /// </summary>
    /// <param name="p"> The point. </param>
    /// <returns> True if the point lies on the disk, false otherwise. </returns>
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        var dist = p.X * p.X + p.Y * p.Y;
        return dist < 1.0f && Functions.Are_Close(p.Z, 0.0f);
    }
}

// ===========================================================================
// === END === END === END === END === END === END === END === END === END ===
// ===========================================================================

// ===========================================================================
// === BOX === BOX === BOX === BOX === BOX === BOX === BOX === BOX === BOX ===
// ===========================================================================

/// <summary>
/// A 3D box with faces parallel to the axes.
/// </summary>
public class Box : Shape
{
    /// <summary>
    /// An array of two points representing the minimum (first element) and maximum (second element) extent of the box.
    /// </summary>
    public Point[] Bounds = new Point[2];
    
    /// <summary>
    /// Box constructor. Initialize a new instance of the <see cref="Box"/> class, potentially associating a transformation to it.
    /// </summary>
    /// <param name="max"> The point representing the maximum extent of each axis of the box. If not specified, the default value of 1 is used for each axis. </param>
    /// <param name="min"> The point representing the minimum extent of each axis of the box. If not specified, the default value of -1 is used for each axis. </param>
    /// <param name="T"> The transformation associated to the box. </param>
    /// <param name="m"> The material of the box. </param>
    public Box(Point? max = null, Point? min = null, Transformation? T = null, Material? m = null) : base(T, m)
    {
        Bounds[0] = min ?? new Point(-1.0f, -1.0f, -1.0f);
        Bounds[1] = max ?? new Point(1.0f, 1.0f, 1.0f);
    }

    /// <summary>
    /// Compute the normal to a box. The normal is computed for <paramref name="point"/> (a point on the surface of
    /// the box), and it is chosen so that it is always in the opposite direction with respect to <paramref name="rayDir"/>.
    /// </summary>
    /// <param name="point"> The point. </param>
    /// <param name="rayDir"> The direction of the ray. </param>
    /// <returns> The normal. </returns>
    private Normal Box_Normal(Point point, Vec rayDir)
    {
        Normal result;
        if (Functions.Are_Close(point.X, Bounds[0].X)) result = new Normal(-1.0f, 0.0f, 0.0f);
        else if (Functions.Are_Close(point.X, Bounds[1].X)) result = new Normal(1.0f, 0.0f, 0.0f);
        else if (Functions.Are_Close(point.Y, Bounds[0].Y)) result = new Normal(0.0f, -1.0f, 0.0f);
        else if (Functions.Are_Close(point.Y, Bounds[1].Y)) result = new Normal(0.0f, 1.0f, 0.0f);
        else if (Functions.Are_Close(point.Z, Bounds[0].Z)) result = new Normal(0.0f, 0.0f, -1.0f);
        else result = new Normal(0.0f, 0.0f, 1.0f);
        if (result.To_Vec().Dot(rayDir) > 0.0f) result = -result;
        return result;
    }

    /// <summary>
    /// Convert a 3D point on the surface of the box into a (u, v) 2D point.
    /// </summary>
    /// <param name="p"> The 3D point. </param>
    /// <returns> A <see cref="Vec2D"/> containing the coordinates of the surface point of the box. </returns>
    public Vec2D Box_Point_To_UV(Point p) // Reference: http://raytracerchallenge.com/bonus/texture-mapping.html
                                          // http://ilkinulas.github.io/development/unity/2016/05/06/uv-mapping.html
    {
        Vec2D uv;
        // Left face
        if (Functions.Are_Close(p.X, Bounds[0].X))
            uv = new Vec2D(Coord(2, false, p) * 0.25f, 1/3.0f + Coord(1, false, p)/3.0f);
            //uv = new Vec2D(Coord(2, false, p), Coord(1, false, p));
        // Right face
        else if (Functions.Are_Close(p.X, Bounds[1].X))
            uv = new Vec2D(0.5f + Coord(2, true, p) * 0.25f, 1/3.0f + Coord(1, false, p)/3.0f);
            //uv = new Vec2D( Coord(2, true, p), Coord(1, false, p));
        // Down face
        else if (Functions.Are_Close(p.Y, Bounds[0].Y))
            uv = new Vec2D(0.25f + Coord(0, false, p) * 0.25f, 2/3.0f + Coord(2, false, p)/3.0f);
            //uv = new Vec2D(Coord(0, false, p), Coord(2, false, p));
        // Up face
        else if (Functions.Are_Close(p.Y, Bounds[1].Y))
            uv = new Vec2D(0.25f + Coord(0, false, p) * 0.25f, Coord(2, true, p)/3.0f);
            //uv = new Vec2D(Coord(0, false, p), Coord(2, true, p));
        // Back face
        else if (Functions.Are_Close(p.Z, Bounds[0].Z))
            uv = new Vec2D(0.75f + Coord(0, true, p) * 0.25f, 1/3.0f + Coord(1, false, p)/3.0f);
            //uv = new Vec2D( Coord(0, true, p), Coord(1, false, p));
        // Front face
        else 
            uv = new Vec2D(0.25f + Coord(0, false, p) * 0.25f, 1/3.0f + Coord(1, false, p)/3.0f);
            //uv = new Vec2D(Coord(0, false, p), Coord(1, false, p));
        return uv;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="minus"></param>
    /// <param name="p"> The point. </param>
    /// <returns></returns>
    private float Coord(float axis, bool minus, Point p)
    {
        var min = Bounds[0].Z;
        var max = Bounds[1].Z;
        var coord = p.Z;
        switch (axis)
        {
            case 0:
                min = Bounds[0].X;
                max = Bounds[1].X;
                coord = p.X;
                break;
            case 1:
                min = Bounds[0].Y;
                max = Bounds[1].Y;
                coord = p.Y;
                break;
        }

        if (minus) return (max - coord) / (max - min);
        return (coord - min) / (max - min);
    }

    /// <summary>
    /// Check if a ray intersects the box.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The <see cref="HitRecord"/>, or <see langword="null"/> if no intersection was found. </returns>
    public override HitRecord? Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        float tmin, tmax, tymin, tymax, tzmin, tzmax;

        var invDirX = 1 / invRay.Dir.X;
        var invDirY = 1 / invRay.Dir.Y;
        var invDirZ = 1 / invRay.Dir.Z;
        var signx = (invRay.Dir.X < 0).GetHashCode();
        var signy = (invRay.Dir.Y < 0).GetHashCode();
        var signz = (invRay.Dir.Z < 0).GetHashCode();

        tmin = (Bounds[signx].X - invRay.Origin.X) * invDirX;
        tmax = (Bounds[1-signx].X - invRay.Origin.X) * invDirX;
        tymin = (Bounds[signy].Y - invRay.Origin.Y) * invDirY;
        tymax = (Bounds[1-signy].Y - invRay.Origin.Y) * invDirY;

        if (tmin > tymax || tymin > tmax) return null;
        if (tymin > tmin) tmin = tymin;
        if (tymax < tmax) tmax = tymax;
        
        tzmin = (Bounds[signz].Z - invRay.Origin.Z) * invDirZ;
        tzmax = (Bounds[1-signz].Z - invRay.Origin.Z) * invDirZ;
        
        if (tmin > tzmax || tzmin > tmax) return null;
        if (tzmin > tmin) tmin = tzmin;
        if (tzmax < tmax) tmax = tzmax;
        
        float firstHitT;
        if (tmin < invRay.TMax && tmin > invRay.TMin) firstHitT = tmin;
        else if (tmax < invRay.TMax && tmax > invRay.TMin) firstHitT = tmax;
        else return null;
        var hitPoint = invRay.At(firstHitT);
        
        return new HitRecord(
            Tr * hitPoint,
            Tr * Box_Normal(hitPoint, invRay.Dir),
            firstHitT,
            r,
            Box_Point_To_UV(hitPoint),
            Mt
        );
    }

    /// <summary>
    /// Quickly checks if a ray intersects the box.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> True if there is intersection, false otherwise. </returns>
    public override bool Quick_Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        float tmin, tmax, tymin, tymax, tzmin, tzmax;

        var invDirX = 1 / invRay.Dir.X;
        var invDirY = 1 / invRay.Dir.Y;
        var invDirZ = 1 / invRay.Dir.Z;
        var signx = (invRay.Dir.X < 0).GetHashCode();
        var signy = (invRay.Dir.Y < 0).GetHashCode();
        var signz = (invRay.Dir.Z < 0).GetHashCode();

        tmin = (Bounds[signx].X - invRay.Origin.X) * invDirX;
        tmax = (Bounds[1-signx].X - invRay.Origin.X) * invDirX;
        tymin = (Bounds[signy].Y - invRay.Origin.Y) * invDirY;
        tymax = (Bounds[1-signy].Y - invRay.Origin.Y) * invDirY;

        if (tmin > tymax || tymin > tmax) return false;
        if (tymin > tmin) tmin = tymin;
        if (tymax < tmax) tmax = tymax;
        
        tzmin = (Bounds[signz].Z - invRay.Origin.Z) * invDirZ;
        tzmax = (Bounds[1-signz].Z - invRay.Origin.Z) * invDirZ;

        if (tmin > tzmax || tzmin > tmax) return false;
        if (tzmin > tmin) tmin = tzmin;
        if (tzmax < tmax) tmax = tzmax;
        return (tmin > invRay.TMin && tmin < invRay.TMax) || (tmax > invRay.TMin && tmax < invRay.TMax);
    }

    /// <summary>
    /// Check if a ray intersects the box by computing a list of all possible intersections.
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns> The list of <see cref="HitRecord"/> of intersections, ordered by the distance from the origin of the ray.
    /// If no intersection is found, <see langword="null"/> is returned. </returns>
    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var invRay = Tr.Inverse * r;
        float tmin, tmax, tymin, tymax, tzmin, tzmax;

        var invDirX = 1 / invRay.Dir.X;
        var invDirY = 1 / invRay.Dir.Y;
        var invDirZ = 1 / invRay.Dir.Z;
        var signx = (invRay.Dir.X < 0).GetHashCode();
        var signy = (invRay.Dir.Y < 0).GetHashCode();
        var signz = (invRay.Dir.Z < 0).GetHashCode();

        tmin = (Bounds[signx].X - invRay.Origin.X) * invDirX;
        tmax = (Bounds[1-signx].X - invRay.Origin.X) * invDirX;
        tymin = (Bounds[signy].Y - invRay.Origin.Y) * invDirY;
        tymax = (Bounds[1-signy].Y - invRay.Origin.Y) * invDirY;

        if (tmin > tymax || tymin > tmax) return null;
        if (tymin > tmin) tmin = tymin;
        if (tymax < tmax) tmax = tymax;
        
        tzmin = (Bounds[signz].Z - invRay.Origin.Z) * invDirZ;
        tzmax = (Bounds[1-signz].Z - invRay.Origin.Z) * invDirZ;
        
        if (tmin > tzmax || tzmin > tmax) return null;
        if (tzmin > tmin) tmin = tzmin;
        if (tzmax < tmax) tmax = tzmax;
        var intersections = new List<HitRecord>();
        var hitPoint1 = invRay.At(tmin);
        var hitPoint2 = invRay.At(tmax);
        if (tmin < invRay.TMax && tmin > invRay.TMin) {
            intersections.Add(new HitRecord(
                Tr * hitPoint1,
                Tr * Box_Normal(hitPoint1, invRay.Dir),
                tmin,
                r,
                Box_Point_To_UV(hitPoint1),
                Mt
            ));
        }

        if (tmax < invRay.TMax && tmax > invRay.TMin) {
            intersections.Add(new HitRecord(
                Tr * hitPoint2,
                Tr * Box_Normal(hitPoint2, invRay.Dir), 
                tmax, 
                r, 
                Box_Point_To_UV(hitPoint2), 
                Mt
            ));
        }

        return intersections.Count == 0 ? null : intersections;
    }

    /// <summary>
    /// Check if a point is inside the box.
    /// </summary>
    /// <param name="p"> The point. </param>
    /// <returns> True if the point is inside the box, false otherwise. </returns>
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return p.X is < 1.0f and > -1.0f && p.Y is < 1.0f and > -1.0f && p.Z is < 1.0f and > -1.0f;
    }
}

// ===========================================================================
// === END === END === END === END === END === END === END === END === END ===
// ===========================================================================