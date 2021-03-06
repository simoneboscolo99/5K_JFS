using Xunit;

namespace Trace.Tests;

public class CsgUnionTests
{
    [Fact]
    public void TestHit()
    {
        var sphere1 = new Sphere();
        var sphere2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, 0.5f)));
        var csg = new CsgUnion(sphere1, sphere2);
        
        var ray1 = new Ray(new Point(0.0f, 0.0f, -2.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection1 = csg.Ray_Intersection(ray1);
        var hit1 = csg.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1!=null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, -1.0f), 
            new Normal(0.0f, 0.0f, -1.0f), 
            1.0f, 
            ray1, 
            new Vec2D(0.0f, 1.0f), new Material()
        ).Is_Close(intersection1), "Test hit 1");
        
        var ray2 = new Ray(new Point(0.0f, 0.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection2 = csg.Ray_Intersection(ray2);
        var hit2 = csg.Quick_Ray_Intersection(ray2);
        Assert.True(hit2, "Test quick intersection 2");
        Assert.True(intersection2!=null, "Test intersection 2");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 1.5f), 
            new Normal(0.0f, 0.0f, 1.0f), 
            0.5f, 
            ray2, 
            new Vec2D(0.0f, 0.0f), new Material()
        ).Is_Close(intersection2), "Test hit 2");
        
        var ray3 = new Ray(new Point(-2.0f, 0.0f, 1.6f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection3 = csg.Ray_Intersection(ray3);
        var hit3 = csg.Quick_Ray_Intersection(ray3);
        Assert.False(hit3, "Test quick intersection 3");
        Assert.True(intersection3 == null, "Test intersection 3");
    }

    [Fact]
    public void TestInnerHit()
    {
        var sphere1 = new Sphere();
        var sphere2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, 0.5f)));
        var csg = new CsgUnion(sphere1, sphere2);
        
        var ray1 = new Ray(new Point(0.0f, 0.0f, 0.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection1 = csg.Ray_Intersection(ray1);
        var hit1 = csg.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1!=null, "Test intersection 1");
    }

    [Fact]
    public void TestIsInternal()
    {
        var sphere1 = new Sphere();
        var sphere2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, 0.5f)));
        var csg = new CsgUnion(sphere1, sphere2);
        
        // Point in sphere1
        Assert.True(csg.Is_Internal(new Point(0.0f, 0.0f, -0.7f)), "Test internal 1");
        // Point sphere2 
        Assert.True(csg.Is_Internal(new Point(0.0f, 0.0f, 1.4f)), "Test internal 2");
        // Point in sphere1 and sphere2
        Assert.True(csg.Is_Internal(new Point(0.0f, 0.0f, 0.8f)), "Test internal 3");
        
        // Point outside sphere1 and sphere2
        Assert.False(csg.Is_Internal(new Point(0.0f, 0.0f, 1.6f)), "Test not internal 1");
        Assert.False(csg.Is_Internal(new Point(0.0f, 0.0f, -1.2f)), "Test not internal 2");
    }
}

public class CsgDifferenceTests
{
    [Fact]
    public void TestHit()
    {
        var sphere1 = new Sphere();
        var sphere2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, 0.5f)));
        var csg = new CsgDifference(sphere1, sphere2);
        
        var ray1 = new Ray(new Point(0.0f, 0.0f, -2.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection1 = csg.Ray_Intersection(ray1);
        var hit1 = csg.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1!=null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, -1.0f), 
            new Normal(0.0f, 0.0f, -1.0f), 
            1.0f, 
            ray1, 
            new Vec2D(0.0f, 1.0f), new Material()
        ).Is_Close(intersection1), "Test hit 1");
        
        var ray2 = new Ray(new Point(0.0f, 0.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection2 = csg.Ray_Intersection(ray2);
        var hit2 = csg.Quick_Ray_Intersection(ray2);
        Assert.True(hit2, "Test quick intersection 2");
        Assert.True(intersection2!=null, "Test intersection 2");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, -0.5f), 
            new Normal(0.0f, 0.0f, 1.0f), 
            2.5f, 
            ray2, 
            new Vec2D(0.0f, 1.0f), new Material()
        ).Is_Close(intersection2), "Test hit 2");
        
        var ray3 = new Ray(new Point(-2.0f, 0.0f, 0.5f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection3 = csg.Ray_Intersection(ray3);
        var hit3 = csg.Quick_Ray_Intersection(ray3);
        Assert.False(hit3, "Test quick intersection 3");
        Assert.True(intersection3 == null, "Test intersection 3");
        
        var ray4 = new Ray(new Point(0.0f, 2.0f, 1.3f), new Vec(0.0f, -1.0f, 0.0f));
        var intersection4 = csg.Ray_Intersection(ray4);
        var hit4 = csg.Quick_Ray_Intersection(ray4);
        Assert.False(hit4, "Test quick intersection 4");
        Assert.True(intersection4 == null, "Test intersection 4");
    }

    [Fact]
    public void TestInnerHit()
    {
        var sphere1 = new Sphere();
        var sphere2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, 0.5f)));
        var csg = new CsgDifference(sphere1, sphere2);
        
        var ray1 = new Ray(new Point(0.0f, 0.0f, 0.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection1 = csg.Ray_Intersection(ray1);
        var hit1 = csg.Quick_Ray_Intersection(ray1);
        Assert.False(hit1, "Test quick intersection 1");
        Assert.False(intersection1!=null, "Test intersection 1");
    }

    [Fact]
    public void TestIsInternal()
    {
        var sphere1 = new Sphere();
        var sphere2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, 0.5f)));
        var csg = new CsgDifference(sphere1, sphere2);
        
        // Point in sphere1
        Assert.True(csg.Is_Internal(new Point(0.0f, 0.0f, -0.7f)), "Test internal 1");
        
        // Point in sphere1 and sphere2 
        Assert.False(csg.Is_Internal(new Point(0.0f, 0.0f, -0.4f)), "Test not internal 1");
        Assert.False(csg.Is_Internal(new Point(0.0f, 0.0f, 0.8f)), "Test not internal 2");
        
        // Point in sphere 2
        Assert.False(csg.Is_Internal(new Point(0.0f, 0.0f, 1.3f)), "Test not internal 3");
    }
}

public class CsgIntersectionTests
{
    [Fact]
    public void TestHit()
    {
        var sphere1 = new Sphere();
        var sphere2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, 0.5f)));
        var csg = new CsgIntersection(sphere1, sphere2);
        
        var ray1 = new Ray(new Point(0.0f, 0.0f, -2.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection1 = csg.Ray_Intersection(ray1);
        var hit1 = csg.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1!=null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, -0.5f), 
            new Normal(0.0f, 0.0f, -1.0f), 
            1.5f, 
            ray1, 
            new Vec2D(0.0f, 1.0f), new Material()
        ).Is_Close(intersection1), "Test hit 1");
        
        var ray2 = new Ray(new Point(0.0f, 0.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection2 = csg.Ray_Intersection(ray2);
        var hit2 = csg.Quick_Ray_Intersection(ray2);
        Assert.True(hit2, "Test quick intersection 2");
        Assert.True(intersection2!=null, "Test intersection 2");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 1.0f), 
            new Normal(0.0f, 0.0f, 1.0f), 
            1.0f, 
            ray2, 
            new Vec2D(0.0f, 0.0f), new Material()
        ).Is_Close(intersection2), "Test hit 2");
        
        var ray3 = new Ray(new Point(-2.0f, 0.0f, 1.1f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection3 = csg.Ray_Intersection(ray3);
        var hit3 = csg.Quick_Ray_Intersection(ray3);
        Assert.False(hit3, "Test quick intersection 3");
        Assert.True(intersection3 == null, "Test intersection 3");
        
        var ray4 = new Ray(new Point(0.0f, 2.0f, -0.6f), new Vec(0.0f, -1.0f, 0.0f));
        var intersection4 = csg.Ray_Intersection(ray4);
        //var hit4 = csg.Quick_Ray_Intersection(ray4);
        //Assert.False(hit4, "Test quick intersection 4");
        Assert.True(intersection4 == null, "Test intersection 4");
    }

    [Fact]
    public void TestInnerHit()
    {
        var sphere1 = new Sphere();
        var sphere2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, 0.5f)));
        var csg = new CsgIntersection(sphere1, sphere2);
        
        var ray1 = new Ray(new Point(0.0f, 0.0f, 0.5f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection1 = csg.Ray_Intersection(ray1);
        var hit1 = csg.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1!=null, "Test intersection 1");
        
        var ray2 = new Ray(new Point(0.0f, 0.0f, 1.6f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection2 = csg.Ray_Intersection(ray2);
        var hit2 = csg.Quick_Ray_Intersection(ray2);
        Assert.False(hit2, "Test quick intersection 2");
        Assert.False(intersection2!=null, "Test intersection 2");
    }

    [Fact]
    public void TestIsInternal()
    {
        var sphere1 = new Sphere();
        var sphere2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, 0.5f)));
        var csg = new CsgIntersection(sphere1, sphere2);
        
        // Point in sphere1 and sphere2
        Assert.True(csg.Is_Internal(new Point(0.0f, 0.0f, -0.3f)), "Test internal 1");
        
        // Point in sphere1 or sphere2 
        Assert.False(csg.Is_Internal(new Point(0.0f, 0.0f, -0.6f)), "Test not internal 1");
        Assert.False(csg.Is_Internal(new Point(0.0f, 0.0f, 1.01f)), "Test not internal 2");
        Assert.False(csg.Is_Internal(new Point(0.0f, 0.0f, 1.3f)), "Test not internal 3");
    }
}