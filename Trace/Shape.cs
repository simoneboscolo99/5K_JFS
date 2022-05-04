namespace Trace;

public abstract class Shape
{
    public Transformation Tr { get; set; }

    public void Init(Transformation t)
    {
        Tr = t;
    }

    public abstract HitRecord? Ray_Intersection(Ray r);
}