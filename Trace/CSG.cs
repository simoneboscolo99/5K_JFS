namespace Trace;

public class CsgUnion : Shape
{
    /// <summary>
    /// First shape.
    /// </summary>
    public Shape S1;

    /// <summary>
    /// Second shape.
    /// </summary>
    public Shape S2;

    public CsgUnion(Shape s1, Shape s2, Transformation? t = null, Material? material = null) : base(t, material)
    {
        S1 = s1;
        S2 = s2;
    }
    
    public override HitRecord? Ray_Intersection(Ray r)
        => Ray_Intersection_List(r)?[0];

    // not very quick!
    public override bool Quick_Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var intersections1 = S1.Ray_Intersection_List(invRay);
        var intersections2 = S2.Ray_Intersection_List(invRay);
        return intersections1 != null || intersections2 != null;
    }

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

    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return S1.Is_Internal(p) || S2.Is_Internal(p);
    }
}

/// <summary>
/// s1 - s2
/// </summary>
// S1 - S2
public class CsgDifference : Shape
{
    /// <summary>
    /// First shape.
    /// </summary>
    public Shape S1;

    /// <summary>
    /// Second shape.
    /// </summary>
    public Shape S2;

    public CsgDifference(Shape s1, Shape s2, Transformation? t = null, Material? material = null) : base(t, material)
    {
        S1 = s1;
        S2 = s2;
    }

    public override HitRecord? Ray_Intersection(Ray r)
        => Ray_Intersection_List(r)?[0];

    // not very quick!
    public override bool Quick_Ray_Intersection(Ray r)
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

    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return S1.Is_Internal(p) && !S2.Is_Internal(p);
    }
}

public class CsgIntersection : Shape
{
    /// <summary>
    /// First shape.
    /// </summary>
    public Shape S1;

    /// <summary>
    /// Second shape.
    /// </summary>
    public Shape S2;

    public CsgIntersection(Shape s1, Shape s2, Transformation? t = null, Material? material = null) : base(t, material)
    {
        S1 = s1;
        S2 = s2;
    }
    
    public override HitRecord? Ray_Intersection(Ray r)
        => Ray_Intersection_List(r)?[0];
    
    // not very quick!
    public override bool Quick_Ray_Intersection(Ray r)
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

    // DA CAMBIARE!!!!
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

    public override bool Is_Internal(Point p)
    {
        p = Tr.Inverse * p;
        return S1.Is_Internal(p) && S2.Is_Internal(p);
    }
}