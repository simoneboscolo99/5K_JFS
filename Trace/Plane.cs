namespace Trace;

public class Plane : Shape
{
    public override HitRecord? Ray_Intersection(Ray r)
    {
        throw new NotImplementedException("Shape.Ray_Intersection is an abstract method and cannot be called directly");
    }
}