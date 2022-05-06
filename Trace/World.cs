namespace Trace;

public class World
{
    List<Shape> World1;

    public World()
    {
        World1 = new List<Shape>();
    }

    public void Add(Shape shape)
    {
        World1.Add(shape);
    }

    public HitRecord? ray_intersection(Ray ray)
    {
        HitRecord? closest = null;
        foreach (var v in World1)
        {
            var intersection = v.Ray_Intersection(ray);
            if (intersection == null) continue;

            if (closest == null || intersection.T < closest.T)
            {
                closest = intersection;
            }
        }

        return closest;
    }
}