namespace Trace;

/// <summary>
/// A class holding a list of shapes, which make a «world». <br/>
/// You can add shapes to a world using :meth:`.World.add`. Typically, you call
/// :meth:`.World.ray_intersection` to check whether a light ray intersects any
/// of the shapes in the world.
/// </summary>
public class World
{
    /// <summary>
    /// 
    /// </summary>
    public List<Shape> World1;

    /// <summary>
    /// 
    /// </summary>
    public World()
    {
        World1 = new List<Shape>();
    }

    /// <summary>
    /// using List(T).Add method, adding an object to the end of the list
    /// </summary>
    /// <param name="shape"></param>
    public void Add(Shape shape)
    {
        World1.Add(shape);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public HitRecord? Ray_Intersection(Ray ray)
    {
        HitRecord? closest = null;
        foreach (var v in World1)
        {
            var intersection = v.Ray_Intersection(ray);
            if (intersection == null) continue;

            if (closest == null || intersection.T < closest.T) closest = intersection;
        }

        return closest;
    }
}