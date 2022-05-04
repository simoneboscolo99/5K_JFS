namespace Trace;

public abstract class Shape
{
    public Transformation Tr { get; set; }

    public Shape(Transformation? T = null)
    {
        Tr = T ?? Transformation.Identity();
    } 

    public void Init(Transformation t)
    {
        Tr = t;
    }

    public abstract HitRecord? Ray_Intersection(Ray r);

}