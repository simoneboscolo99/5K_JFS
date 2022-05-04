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
}