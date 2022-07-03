namespace Trace;

/// <summary>
/// Constructive Solid Geometry (CSG). <br/>
/// A 3D shape given by the union of two shapes: <see cref="S1"/> + <see cref="S2"/>.
/// </summary>
public class CsgUnion : Shape
{
    /// <summary>
    /// The first shape.
    /// </summary>
    private Shape S1;

    /// <summary>
    /// The second shape.
    /// </summary>
    private Shape S2;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s1"> The first shape. </param>
    /// <param name="s2"> The second shape. </param>
    /// <param name="t"> The transformation associated to the union shape. </param>
    /// <param name="material"> The material of the union shape. </param>
    public CsgUnion(Shape s1, Shape s2, Transformation? t = null, Material? material = null) : base(t, material)
    {
        S1 = s1;
        S2 = s2;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns></returns>
    public override HitRecord? Ray_Intersection(Ray r)
        => Ray_Intersection_List(r)?[0];
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns></returns>
    public override bool Quick_Ray_Intersection(Ray r) // not very quick!
    {
        var invRay = Tr.Inverse * r;
        var intersections1 = S1.Ray_Intersection_List(invRay);
        var intersections2 = S2.Ray_Intersection_List(invRay);
        return intersections1 != null || intersections2 != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns></returns>
    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var intersections1 = S1.Ray_Intersection_List(invRay);
        var intersections2 = S2.Ray_Intersection_List(invRay);

        var intersections = new List<HitRecord>();
        if (intersections1 != null) intersections.AddRange(intersections1);
        if (intersections2 != null) intersections.AddRange(intersections2);
        return intersections.Count != 0 ? intersections.OrderBy(o => o.T).ToList() : null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"> The point. </param>
    /// <returns></returns>
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return S1.Is_Internal(p) || S2.Is_Internal(p);
    }
}

/// <summary>
/// Constructive Solid Geometry (CSG). <br/>
/// A 3D shape given by the difference of two shapes: <see cref="S1"/> - <see cref="S2"/>.
/// </summary>
public class CsgDifference : Shape
{
    /// <summary>
    /// The first shape.
    /// </summary>
    public Shape S1;

    /// <summary>
    /// The second shape.
    /// </summary>
    public Shape S2;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s1"> The first shape. </param>
    /// <param name="s2"> The second shape. </param>
    /// <param name="t"> The transformation associated to the difference shape. </param>
    /// <param name="material"> The material of the difference shape. </param>
    public CsgDifference(Shape s1, Shape s2, Transformation? t = null, Material? material = null) : base(t, material)
    {
        S1 = s1;
        S2 = s2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns></returns>
    public override HitRecord? Ray_Intersection(Ray r)
        => Ray_Intersection_List(r)?[0];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns></returns>
    public override bool Quick_Ray_Intersection(Ray r) // not very quick!
    {
        var invRay = Tr.Inverse * r;
        var intersections1 = S1.Ray_Intersection_List(invRay);
        if (intersections1 == null) return false;
        var dim1 = intersections1.Count;
        for (var i = dim1 - 1; i >= 0; i--)
            if (S2.Is_Internal(intersections1[i].WorldPoint)) intersections1.RemoveAt(i);

        var intersections2 = S2.Ray_Intersection_List(invRay);
        if (intersections2 != null)
        {
            var dim2 = intersections2.Count;
            for (var i = dim2 - 1; i >= 0; i--)
                if (!S1.Is_Internal(intersections2[i].WorldPoint)) intersections2.RemoveAt(i);
        }

        var intersections = new List<HitRecord>();
        intersections.AddRange(intersections1);
        if (intersections2 != null) intersections.AddRange(intersections2);
        return intersections.Count > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns></returns>
    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var intersections1 = S1.Ray_Intersection_List(invRay);
        if (intersections1 == null) return null;
        var dim1 = intersections1.Count;
            for (var i = dim1 - 1; i >= 0; i--)
                if (S2.Is_Internal(intersections1[i].WorldPoint)) intersections1.RemoveAt(i);

        var intersections2 = S2.Ray_Intersection_List(invRay);
        if (intersections2 != null)
        {
            var dim2 = intersections2.Count;
            for (var i = dim2 - 1; i >= 0; i--)
                if (!S1.Is_Internal(intersections2[i].WorldPoint)) intersections2.RemoveAt(i);
        }

        var intersections = new List<HitRecord>();
        intersections.AddRange(intersections1);
        if (intersections2 != null) intersections.AddRange(intersections2);
        return intersections.Count != 0 ? intersections.OrderBy(o => o.T).ToList() : null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"> The point. </param>
    /// <returns></returns>
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return S1.Is_Internal(p) && !S2.Is_Internal(p);
    }
}

/// <summary>
/// Constructive Solid Geometry (CSG). <br/>
/// A 3D shape given by the intersection of two shapes: <see cref="S1"/> ∩ <see cref="S2"/>.
/// </summary>
public class CsgIntersection : Shape
{
    /// <summary>
    /// The first shape.
    /// </summary>
    private Shape S1;

    /// <summary>
    /// The second shape.
    /// </summary>
    private Shape S2;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s1"> The first shape. </param>
    /// <param name="s2"> The second shape. </param>
    /// <param name="t"> The transformation associated to the intersection shape. </param>
    /// <param name="material"> The material of the intersection shape. </param>
    public CsgIntersection(Shape s1, Shape s2, Transformation? t = null, Material? material = null) : base(t, material)
    {
        S1 = s1;
        S2 = s2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns></returns>
    public override HitRecord? Ray_Intersection(Ray r)
        => Ray_Intersection_List(r)?[0];
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns></returns>
    public override bool Quick_Ray_Intersection(Ray r) // not very quick!
    {
        var invRay = Tr.Inverse * r;
        var intersections1 = S1.Ray_Intersection_List(invRay);
        if (intersections1 == null) return false;
        var dim1 = intersections1.Count;
        for (var i = dim1 - 1; i >= 0; i--)
            if (!S2.Is_Internal(intersections1[i].WorldPoint)) intersections1.RemoveAt(i);

        var intersections2 = S2.Ray_Intersection_List(invRay);
        if (intersections2 == null) return false;
        var dim2 = intersections2.Count;
        for (var i = dim2 - 1; i >= 0; i--)
            if (!S1.Is_Internal(intersections2[i].WorldPoint)) intersections2.RemoveAt(i);

        var intersections = new List<HitRecord>();
        intersections.AddRange(intersections1);
        intersections.AddRange(intersections2);
        return intersections.Count > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"> The ray. </param>
    /// <returns></returns>
    public override List<HitRecord>? Ray_Intersection_List(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var intersections1 = S1.Ray_Intersection_List(invRay);
        if (intersections1 == null) return null;
        var dim1 = intersections1.Count;
            for (var i = dim1 - 1; i >= 0; i--)
                if (!S2.Is_Internal(intersections1[i].WorldPoint)) intersections1.RemoveAt(i);

        var intersections2 = S2.Ray_Intersection_List(invRay);
        if (intersections2 == null) return null;
        var dim2 = intersections2.Count;
            for (var i = dim2 - 1; i >= 0; i--)
                if (!S1.Is_Internal(intersections2[i].WorldPoint)) intersections2.RemoveAt(i);

        var intersections = new List<HitRecord>();
        intersections.AddRange(intersections1);
        intersections.AddRange(intersections2);
        return intersections.Count != 0 ? intersections.OrderBy(o => o.T).ToList() : null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"> The point. </param>
    /// <returns></returns>
    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return S1.Is_Internal(p) && S2.Is_Internal(p);
    }
}