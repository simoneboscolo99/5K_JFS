using Xunit;

namespace Trace.Tests;

public class SphereTests
{

    [Fact]
    public void Test_Ray_Intersection()
    {
        var sph = new Sphere();
        var ray1 = new Ray(new Point(0.0f, 0.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var inter = sph.Ray_Intersection(ray1);

        Assert.True(new HitRecord(new Point(0.0f, 0.0f, 1.0f), new Normal(0.0f, 0.0f, 1.0f), 1.0f, ray1,
            new Vec2D(0.0f, 0.0f)).Is_Close(inter));

    }
    
    
}

public class WorldTests
{
    
    [Fact]
    public void Test_Ray_Intersection()
    {
        var world = new World();
        var sphere1 = new Sphere(Transformation.Translation(new Vec(2.0f, 0.0f, 0.0f)));
        var sphere2 = new Sphere(Transformation.Translation(new Vec(8.0f, 0.0f, 0.0f)));
        world.Add(sphere1);
        world.Add(sphere2);
        var intersection1 = world.ray_intersection(new Ray(new Point(0.0f, 0.0f, 0.0f), new Vec(1.0f, 0.0f, 0.0f)));
        Assert.True(intersection1 != null && intersection1.WorldPoint.Is_Close(new Point(1.0f, 0.0f, 0.0f)));
        var intersection2 = world.ray_intersection(new Ray(new Point(10.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f)));
        Assert.True(intersection2 != null && intersection2.WorldPoint.Is_Close(new Point(9.0f, 0.0f, 0.0f)));
    }
/*
    [Fact]
    public void Test_Quick_Ray_Intersection()
    {
        var world = new World();
        var sphere1 = new Sphere(Transformation.Translation(new Vec(2.0f, 0.0f, 0.0f)));
        var sphere2 = new Sphere(Transformation.Translation(new Vec(8.0f, 0.0f, 0.0f)));        world.add_shape(sphere1)
        world.Add(sphere1);
        world.Add(sphere2);
        Assert.False(world.is);
    }
*/
}






