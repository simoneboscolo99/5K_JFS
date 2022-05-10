using Xunit;

namespace Trace.Tests;

public class SphereTests
{

    [Fact]
    public void TestHit()
    {
        var sphere = new Sphere();
        var ray1 = new Ray(new Point(0.0f, 0.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection1 = sphere.Ray_Intersection(ray1);
        Assert.True(intersection1!=null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 1.0f), 
            new Normal(0.0f, 0.0f, 1.0f), 
            1.0f, 
            ray1, 
            new Vec2D(0.0f, 0.0f)
            ).Is_Close(intersection1), "Test hit 1");
        
        var ray2 = new Ray(new Point(3.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection2 = sphere.Ray_Intersection(ray2);
        Assert.True(intersection2!=null, "Test intersection 2");
        Assert.True(new HitRecord(
            new Point(1.0f, 0.0f, 0.0f), 
            new Normal(1.0f, 0.0f, 0.0f), 
            2.0f, 
            ray2, 
            new Vec2D(0.0f, 0.5f)
        ).Is_Close(intersection2), "Test hit 2");
        
        Assert.False(sphere.Ray_Intersection(new Ray(new Point(0.0f, 10.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f))) != null, "Test no intersection");
    }
    [Fact]
    public void TestInnerHit()
    {
        var sphere = new Sphere();
        var ray = new Ray(new Point(0.0f, 0.0f, 0.0f), new Vec(1.0f, 0.0f, 0.0f));
        var intersection = sphere.Ray_Intersection(ray);
        Assert.True(intersection != null, "Test intersection");
        Assert.True(new HitRecord(
            new Point(1.0f, 0.0f, 0.0f),
            new Normal(-1.0f, 0.0f, 0.0f),
            1.0f,
            ray,
            new Vec2D(0.0f, 0.5f)
        ).Is_Close(intersection), "Test hit");
    }

    [Fact]
    public void TestTransformation()
    {
        var sphere = new Sphere(Transformation.Translation(new Vec(10.0f, 0.0f, 0.0f)));
        var ray1 = new Ray(new Point(10.0f, 0.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection1 = sphere.Ray_Intersection(ray1);
        Assert.True(intersection1 != null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(10.0f, 0.0f, 1.0f),
            new Normal(0.0f, 0.0f, 1.0f),
            1.0f,
            ray1,
            new Vec2D(0.0f, 0.0f)
        ).Is_Close(intersection1), "Test hit 1");

        var ray2 = new Ray(new Point(13.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection2 = sphere.Ray_Intersection(ray2);
        Assert.True(intersection2 != null, "Test intersection 2");
        Assert.True(new HitRecord(
            new Point(11.0f, 0.0f, 0.0f),
            new Normal(1.0f, 0.0f, 0.0f),
            2.0f,
            ray2,
            new Vec2D(0.0f, 0.5f)
        ).Is_Close(intersection2), "Test hit 2");

        // Check if the sphere failed to move by trying to hit the untransformed shape
        Assert.False(sphere.Ray_Intersection(new Ray(new Point(0.0f, 0.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f))) != null,
            "Test no intersection 1");

        // Check if the *inverse* transformation was wrongly applied
        Assert.False(
            sphere.Ray_Intersection(new Ray(new Point(-10.0f, 0.0f, 0.0f), new Vec(0.0f, 0.0f, -1.0f))) != null,
            "Test no intersection 2");
    }

    [Fact]
    public void TestNormals()
    
    {
        var sphere = new Sphere(Transformation.Scale(new Vec(2.0f, 1.0f, 1.0f)));
        var ray = new Ray(new Point(1.0f, 1.0f, 0.0f), new Vec(-1.0f, -1.0f, 0.0f));
        var intersection = sphere.Ray_Intersection(ray);
        Assert.True(intersection != null);
        // We normalize "intersection.Normal" as we are not interested in its length
        Assert.True(
            intersection != null && intersection.Normal.Normalize().Is_Close(new Normal(1.0f, 4.0f, 0.0f).Normalize()),
            "Test normal");
    }

    [Fact]
    public void TestNormalDirection()
    {
        // Scaling a sphere by -1 keeps the sphere the same but reverses its reference frame
        var sphere = new Sphere(Transformation.Scale(new Vec(-1.0f, -1.0f, -1.0f)));
        var ray = new Ray(new Point(0.0f, 2.0f, 0.0f), new Vec(0.0f, -1.0f, 0.0f));
        var intersection = sphere.Ray_Intersection(ray);
        // We normalize "intersection.Normal" as we are not interested in its length
        Assert.True(intersection != null &&
                    intersection.Normal.Normalize().Is_Close(new Normal(0.0f, 1.0f, 0.0f).Normalize()));
    }

    [Fact]
    public void TestUV_Coordinates()
    {
        var sphere = new Sphere();

        // The first four rays hit the unit sphere at the
        // points P1, P2, P3, and P4.
        //
        //                    ^ y
        //                    | P2
        //              , - ~ * ~ - ,
        //          , '       |       ' ,
        //        ,           |           ,
        //       ,            |            ,
        //      ,             |             , P1
        // -----*-------------+-------------*---------> x
        //   P3 ,             |             ,
        //       ,            |            ,
        //        ,           |           ,
        //          ,         |        , '
        //            ' - , _ * _ ,  '
        //                    | P4
        //
        // P5 and P6 are aligned along the x axis and are displaced
        // along z (ray5 in the positive direction, ray6 in the negative
        // direction).

        var ray1 = new Ray(new Point(2.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection1 = sphere.Ray_Intersection(ray1);
        Assert.True(intersection1 != null && intersection1.SurfacePoint.Is_Close(new Vec2D(0.0f, 0.5f)),
            "Test UV ray1");

        var ray2 = new Ray(new Point(0.0f, 2.0f, 0.0f), new Vec(0.0f, -1.0f, 0.0f));
        var intersection2 = sphere.Ray_Intersection(ray2);
        Assert.True(intersection2 != null && intersection2.SurfacePoint.Is_Close(new Vec2D(0.25f, 0.5f)),
            "Test UV ray2");

        var ray3 = new Ray(new Point(-2.0f, 0.0f, 0.0f), new Vec(1.0f, 0.0f, 0.0f));
        var intersection3 = sphere.Ray_Intersection(ray3);
        Assert.True(intersection3 != null && intersection3.SurfacePoint.Is_Close(new Vec2D(0.5f, 0.5f)),
            "Test UV ray3");

        var ray4 = new Ray(new Point(0.0f, -2.0f, 0.0f), new Vec(0.0f, 1.0f, 0.0f));
        var intersection4 = sphere.Ray_Intersection(ray4);
        Assert.True(intersection4 != null && intersection4.SurfacePoint.Is_Close(new Vec2D(0.75f, 0.5f)),
            "Test UV ray4");

        var ray5 = new Ray(new Point(2.0f, 0.0f, 0.5f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection5 = sphere.Ray_Intersection(ray5);
        Assert.True(intersection5 != null && intersection5.SurfacePoint.Is_Close(new Vec2D(0.0f, (float) 1 / 3)),
            "Test UV ray5");

        var ray6 = new Ray(new Point(2.0f, 0.0f, -0.5f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection6 = sphere.Ray_Intersection(ray6);
        Assert.True(intersection6 != null && intersection6.SurfacePoint.Is_Close(new Vec2D(0.0f, (float) 2 / 3)),
            "Test UV ray6");
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
        var intersection1 = world.Ray_Intersection(new Ray(new Point(0.0f, 0.0f, 0.0f), new Vec(1.0f, 0.0f, 0.0f)));
        Assert.True(intersection1 != null && intersection1.WorldPoint.Is_Close(new Point(1.0f, 0.0f, 0.0f)));
        var intersection2 = world.Ray_Intersection(new Ray(new Point(10.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f)));
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

public class PlaneTests
{
    [Fact]
    public void TestPlane()
    {
        var plane = new Plane();

        var ray1 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection1 = plane.Ray_Intersection(ray1);
        
        Assert.True(intersection1 != null, "Check intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 0.0f), 
            new Normal(0.0f, 0.0f, 1.0f), 1.0f, ray1, 
            new Vec2D(0.0f, 0.0f)
            ).Is_Close(intersection1), "Test hit plane 1"); 


        var ray2 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection2 = plane.Ray_Intersection(ray2);
        Assert.False((intersection2 != null), "Hit 2 Plane");

        var ray3 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.1f, 0.0f, 0.0f));
        var intersection3 = plane.Ray_Intersection(ray3);
        Assert.False((intersection3 != null), "Hit 3 Plane");

        var ray4 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 1.0f, 0.0f));
        var intersection4 = plane.Ray_Intersection(ray4);
        Assert.False((intersection4 != null), "Hit 4 Plane");
    }

    [Fact]
    //test after transformation
    public void TestTransformationPlane()
    {
        var plane = new Plane(Transformation.Rotation_Y(90.0f));
        var ray1 = new Ray(new Point(1.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection1 = plane.Ray_Intersection(ray1);
        
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 0.0f),
            new Normal(1.0f, 0.0f, 0.0f),
            1.0f,
            ray1,
            new Vec2D(0.0f, 0.0f)
        ).Is_Close(intersection1), "Test hTplane 1");

        var ray2 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection2 = plane.Quick_Ray_Intersection(ray2);
        Assert.False(intersection2, "Test hTp 2");

        var ray3 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(1.0f, 0.0f, 0.0f));
        var intersection3 = plane.Ray_Intersection(ray3);
        Assert.False((intersection3 != null), "Hit 3 transformPlane");

        var ray4 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 1.0f, 0.0f));
        var intersection4 = plane.Ray_Intersection(ray4);
        Assert.False((intersection4 != null), "Hit 4 transformPlane");

    }

    [Fact]
    public void TestUvCoordinatesPlane()
    {
        var plane = new Plane();
        var ray1 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection1 = plane.Ray_Intersection(ray1);
        Assert.True(intersection1 != null && intersection1.SurfacePoint.Is_Close(new Vec2D(0.0f, 0.0f)), 
            "test UV coord planes 1");

        var ray2 = new Ray(new Point(0.25f, 0.75f, 1.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection2 = plane.Ray_Intersection(ray2);
        Assert.True(intersection2 != null && intersection2.SurfacePoint.Is_Close(new Vec2D(0.25f, 0.75f)),
            "test UV coord planes 2");

        var ray3 = new Ray(new Point(4.25f, 7.75f, 1f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection3 = plane.Ray_Intersection(ray3);
        Assert.True(intersection3 != null && intersection3.SurfacePoint.Is_Close(new Vec2D(0.25f, 0.75f)),
            "test UV coord planes 3");
    }
}
        



