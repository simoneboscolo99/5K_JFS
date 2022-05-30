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
    List<Shape> Wd;

    /// <summary>
    /// 
    /// </summary>
    public World()
    {
        Wd = new List<Shape>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    public void Add(Shape shape)
    {
        Wd.Add(shape);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public HitRecord? Ray_Intersection(Ray ray)
    {
        HitRecord? closest = null;
        foreach (var v in Wd)
        {
            var intersection = v.Ray_Intersection(ray);
            if (intersection == null) continue;

            if (closest == null || intersection.T < closest.T) closest = intersection;
        }

        return closest;
    }
}