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
        var hit1 = sphere.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1!=null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 1.0f), 
            new Normal(0.0f, 0.0f, 1.0f), 
            1.0f, 
            ray1, 
            new Vec2D(0.0f, 0.0f), new Material()
            ).Is_Close(intersection1), "Test hit 1");
        var intersections1 = sphere.Ray_Intersection_List(ray1);
        Assert.True(intersections1 is {Count: 2}, "Test list hits");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, -1.0f),
            new Normal(0.0f, 0.0f, 1.0f),
            3.0f,
            ray1,
            new Vec2D(0.0f, 1.0f), new Material()
            ).Is_Close(intersections1?[1]), "Test second hit");
        
        var ray2 = new Ray(new Point(3.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection2 = sphere.Ray_Intersection(ray2);
        var hit2 = sphere.Quick_Ray_Intersection(ray1);
        Assert.True(hit2, "Test quick intersection 2");
        Assert.True(intersection2!=null, "Test intersection 2");
        Assert.True(new HitRecord(
            new Point(1.0f, 0.0f, 0.0f), 
            new Normal(1.0f, 0.0f, 0.0f), 
            2.0f, 
            ray2, 
            new Vec2D(0.0f, 0.5f), new Material()
        ).Is_Close(intersection2), "Test hit 2");
        
        Assert.False(sphere.Ray_Intersection(new Ray(new Point(0.0f, 10.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f))) != null, "Test no intersection");
        Assert.False(sphere.Quick_Ray_Intersection(new Ray(new Point(0.0f, 10.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f))), "Test no quick intersection");

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
            new Vec2D(0.0f, 0.5f), new Material()
        ).Is_Close(intersection), "Test hit");

        var intersections = sphere.Ray_Intersection_List(ray);
        Assert.True(intersections is {Count: 1}, "Test list hits");
        
    }
     [Fact]
     public void TestTransformation()
     {
         var sphere = new Sphere(Transformation.Translation(new Vec(10.0f, 0.0f, 0.0f)));
         var ray1 = new Ray(new Point(10.0f, 0.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
         var intersection1 = sphere.Ray_Intersection(ray1);
         var hit1 = sphere.Quick_Ray_Intersection(ray1);
         Assert.True(hit1, "Test quick intersection 1");
         Assert.True(intersection1 != null, "Test intersection 1");
         Assert.True(new HitRecord(
             new Point(10.0f, 0.0f, 1.0f),
             new Normal(0.0f, 0.0f, 1.0f),
             1.0f,
             ray1,
             new Vec2D(0.0f, 0.0f), new Material()
         ).Is_Close(intersection1), "Test hit 1");
         var intersections1 = sphere.Ray_Intersection_List(ray1);
         Assert.True(intersections1 is {Count: 2}, "Test list hits");
         Assert.True(new HitRecord(
             new Point(10.0f, 0.0f, -1.0f),
             new Normal(0.0f, 0.0f, 1.0f),
             3.0f,
             ray1,
             new Vec2D(0.0f, 1.0f), new Material()
         ).Is_Close(intersections1?[1]), "Test second hit");
 
         var ray2 = new Ray(new Point(13.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f));
         var intersection2 = sphere.Ray_Intersection(ray2);
         Assert.True(intersection2 != null, "Test intersection 2");
         Assert.True(new HitRecord(
             new Point(11.0f, 0.0f, 0.0f),
             new Normal(1.0f, 0.0f, 0.0f),
             2.0f,
             ray2,
             new Vec2D(0.0f, 0.5f), new Material()
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
            intersection != null && intersection.N.Normalize().Is_Close(new Normal(1.0f, 4.0f, 0.0f).Normalize()),
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
                    intersection.N.Normalize().Is_Close(new Normal(0.0f, 1.0f, 0.0f).Normalize()));
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

    [Fact]
    public void TestIsInternal()
    {
        var sphere = new Sphere();
        Assert.True(sphere.Is_Internal(new Point(0.35f, 0.22f, 0.81f)), "Test internal 1");
        Assert.False(sphere.Is_Internal(new Point(0.51f, 1.04f, 0.74f)), "Test internal 2");
        
        sphere.Tr = Transformation.Scale(new Vec(0.8f, 0.8f, 0.8f));
        Assert.False(sphere.Is_Internal(new Point(0.35f, 0.22f, 0.81f)), "Test internal 3");
        
        sphere.Tr = Transformation.Translation(new Vec(10.0f, 0.0f, 0.0f));
        Assert.True(sphere.Is_Internal(new Point(10.35f, 0.22f, 0.81f)), "Test internal 4");
        Assert.False(sphere.Is_Internal(new Point(0.51f, 1.04f, 0.74f)), "Test internal 5");
    }
}

public class WorldTests
{

    [Fact]
    public void TestRayIntersection()
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
}

public class PlaneTests
{
    [Fact]
    public void TestHit()
    {
        var plane = new Plane();

        var ray1 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection1 = plane.Ray_Intersection(ray1);
        var hit1 = plane.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");

        Assert.True(intersection1 != null, "Check intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 0.0f), 
            new Normal(0.0f, 0.0f, 1.0f), 1.0f, ray1, 
            new Vec2D(0.0f, 0.0f), new Material()
            ).Is_Close(intersection1), "Test hit plane 1");

        var intersections1 = plane.Ray_Intersection_List(ray1);
        Assert.True(intersections1 is {Count: 1}, "Test list hits");
        
        var ray2 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection2 = plane.Ray_Intersection(ray2);
        var hit2 = plane.Quick_Ray_Intersection(ray2);
        Assert.False(hit2, "Test quick intersection 2");
        Assert.False((intersection2 != null), "Hit 2 Plane");

        var ray3 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.1f, 0.0f, 0.0f));
        var intersection3 = plane.Ray_Intersection(ray3);
        Assert.False((intersection3 != null), "Hit 3 Plane");

        var ray4 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 1.0f, 0.0f));
        var intersection4 = plane.Ray_Intersection(ray4);
        Assert.False((intersection4 != null), "Hit 4 Plane");
    }

    [Fact]
    public void TestTransformation()
    {
        var plane = new Plane(Transformation.Rotation_Y(90.0f));
        var ray1 = new Ray(new Point(1.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection1 = plane.Ray_Intersection(ray1);
        var hit1 = plane.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 0.0f),
            new Normal(1.0f, 0.0f, 0.0f),
            1.0f,
            ray1,
            new Vec2D(0.0f, 0.0f), new Material()
        ).Is_Close(intersection1), "Test hit plane 1");
        var intersections1 = plane.Ray_Intersection_List(ray1);
        Assert.True(intersections1 is {Count: 1}, "Test list hits");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 0.0f),
            new Normal(1.0f, 0.0f, 0.0f),
            1.0f,
            ray1,
            new Vec2D(0.0f, 0.0f), new Material()
        ).Is_Close(intersections1?[0]), "Test list hits");

        var ray2 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection2 = plane.Ray_Intersection(ray2);
        var hit2 = plane.Quick_Ray_Intersection(ray2);
        Assert.False(hit2, "Test quick intersection 2");
        Assert.False(intersection2 != null, "Test hTp 2");

        var ray3 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(1.0f, 0.0f, 0.0f));
        var intersection3 = plane.Ray_Intersection(ray3);
        Assert.False((intersection3 != null), "Hit 3 transformPlane");

        var ray4 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 1.0f, 0.0f));
        var intersection4 = plane.Ray_Intersection(ray4);
        Assert.False((intersection4 != null), "Hit 4 transformPlane");
    }

    [Fact]
    public void TestUvCoordinates()
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

    [Fact]
    public void TestIsInternal()
    {
        // plane z=0
        var plane = new Plane();
        Assert.True(plane.Is_Internal(new Point(13.4f, -26.8f, 0.0f)), "Test internal 1");
        
        plane.Tr = Transformation.Translation(new Vec(0.0f, 0.0f, 0.3f));
        Assert.False(plane.Is_Internal(new Point(13.4f, -26.8f, 0.0f)), "Test internal 2");
        
        // now plane x=0
        plane.Tr = Transformation.Rotation_Y(90.0f);
        Assert.True(plane.Is_Internal(new Point(0.0f, -50.3f, 21.4f)), "Test internal 3");
        Assert.False(plane.Is_Internal(new Point(13.4f, -26.8f, 0.0f)), "Test internal 4");
    }
}

public class CylinderTests
{
    [Fact]
    public void TestHit()
    {
        var cylinder = new Cylinder();
        var ray1 = new Ray(new Point(2.0f, 0.0f, 0.5f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection1 = cylinder.Ray_Intersection(ray1);
        var hit1 = cylinder.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1 != null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(1.0f, 0.0f, 0.5f),
            new Normal(1.0f, 0.0f, 0.0f),
            1.0f,
            ray1,
            new Vec2D(0.0f, 0.5f), new Material()
            ).Is_Close(intersection1), "Test hit 1");
        var intersections1 = cylinder.Ray_Intersection_List(ray1);
        Assert.True(intersections1 is {Count: 2}, "Test list hits");
        Assert.True(new HitRecord(
            new Point(-1.0f, 0.0f, 0.5f),
            new Normal(1.0f, 0.0f, 0.0f),
            3.0f,
            ray1,
            new Vec2D(0.5f, 0.5f), new Material()
        ).Is_Close(intersections1?[1]), "Test second hit");
        
        var ray2 = new Ray(new Point(2.0f, 0.0f, 1.1f), new Vec(-1.0f, 0.0f, 0.0f));
        var hit2 = cylinder.Quick_Ray_Intersection(ray2);
        Assert.False(hit2, "Test quick intersection 2");
        
        var ray3 = new Ray(new Point(0.0f, 0.0f, -2.0f), new Vec(0.0f, 0.0f, 1.0f));
        var hit3 = cylinder.Quick_Ray_Intersection(ray3);
        Assert.False(hit3, "Test quick intersection 2");
    }

    [Fact]
    public void TestInnerHit()
    {
        var cylinder = new Cylinder();
        var ray1 = new Ray(new Point(0.0f, 0.0f, 0.5f), new Vec(1.0f, 0.0f, 0.0f));
        var intersection1 = cylinder.Ray_Intersection(ray1);
        var hit1 = cylinder.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1 != null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(1.0f, 0.0f, 0.5f),
            new Normal(-1.0f, 0.0f, 0.0f),
            1.0f,
            ray1,
            new Vec2D(0.0f, 0.5f), new Material()
        ).Is_Close(intersection1), "Test hit 1");
        var intersections1 = cylinder.Ray_Intersection_List(ray1);
        Assert.True(intersections1 is {Count: 1}, "Test list hits");
    }

    [Fact]
    public void TestTransformation()
    {
        var cylinder = new Cylinder(Transformation.Rotation_Y(90.0f));
        var ray1 = new Ray(new Point(0.5f, 0.0f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection1 = cylinder.Ray_Intersection(ray1);
        var hit1 = cylinder.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1 != null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(0.5f, 0.0f, 1.0f),
            new Normal(0.0f, 0.0f, 1.0f),
            1.0f,
            ray1,
            new Vec2D(0.5f, 0.5f), new Material()
        ).Is_Close(intersection1), "Test hit 1");
        var intersections1 = cylinder.Ray_Intersection_List(ray1);
        Assert.True(intersections1 is {Count: 2}, "Test list hits");
        Assert.True(new HitRecord(
            new Point(0.5f, 0.0f, -1.0f),
            new Normal(0.0f, 0.0f, 1.0f),
            3.0f,
            ray1,
            new Vec2D(0.0f, 0.5f), new Material()
        ).Is_Close(intersections1?[1]), "Test second hit");
    }

    [Fact]
    public void TestIsInternal()
    {
        var cylinder = new Cylinder();
        Assert.True(cylinder.Is_Internal(new Point(-0.8f, 0.4f, 0.5f)), "Test internal 1");
        Assert.False(cylinder.Is_Internal(new Point(-1.1f, 0.0f, 0.5f)), "Test internal 2");
        Assert.False(cylinder.Is_Internal(new Point(-0.8f, 0.0f, 1.2f)), "Test internal 3");

        cylinder.Tr = Transformation.Rotation_Y(90.0f);
        Assert.True(cylinder.Is_Internal(new Point(0.5f, 0.0f, 0.0f)), "Test internal 4");
        Assert.False(cylinder.Is_Internal(new Point(-0.5f, 0.0f, 0.0f)), "Test internal 5");

        
        cylinder.Tr = Transformation.Translation(new Vec(0.5f, 0.0f, 12.0f));
        Assert.False(cylinder.Is_Internal(new Point(-0.8f, 0.4f, 0.5f)), "Test internal 6");
        Assert.True(cylinder.Is_Internal(new Point(0.7f, 0.0f, 12.5f)), "Test internal 7");
    }
}

public class DiskTests
{
    [Fact]
    public void TestHit()
    {
        var disk = new Disk();
        var ray1 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection1 = disk.Ray_Intersection(ray1);
        var hit1 = disk.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");

        Assert.True(intersection1 != null, "Check intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 0.0f), 
            new Normal(0.0f, 0.0f, 1.0f), 
            1.0f, 
            ray1, 
            new Vec2D(0.0f, 1.0f), new Material()
        ).Is_Close(intersection1), "Test hit disk 1");

        var intersections1 = disk.Ray_Intersection_List(ray1);
        Assert.True(intersections1 is {Count: 1}, "Test list hits");
        
        var ray2 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection2 = disk.Ray_Intersection(ray2);
        var hit2 = disk.Quick_Ray_Intersection(ray2);
        Assert.False(hit2, "Test quick intersection 2");
        Assert.False((intersection2 != null), "Hit 2 disk");

        var ray3 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(1.0f, 0.0f, 0.0f));
        var intersection3 = disk.Ray_Intersection(ray3);
        Assert.False((intersection3 != null), "Hit 3 disk");

        var ray4 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 1.0f, 0.0f));
        var intersection4 = disk.Ray_Intersection(ray4);
        Assert.False((intersection4 != null), "Hit 4 disk");
    }

    [Fact]
    public void TestTransformation()
    {
        var disk = new Disk(Transformation.Rotation_Y(90.0f));
        var ray1 = new Ray(new Point(1.0f, 0.0f, 0.0f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection1 = disk.Ray_Intersection(ray1);
        var hit1 = disk.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 0.0f),
            new Normal(1.0f, 0.0f, 0.0f),
            1.0f,
            ray1,
            new Vec2D(0.0f, 1.0f), new Material()
        ).Is_Close(intersection1), "Test hit disk 1");
        var intersections1 = disk.Ray_Intersection_List(ray1);
        Assert.True(intersections1 is {Count: 1}, "Test list hits");
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 0.0f),
            new Normal(1.0f, 0.0f, 0.0f),
            1.0f,
            ray1,
            new Vec2D(0.0f, 1.0f), new Material()
        ).Is_Close(intersections1?[0]), "Second test hit disk 1");

        var ray2 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 0.0f, 1.0f));
        var intersection2 = disk.Ray_Intersection(ray2);
        var hit2 = disk.Quick_Ray_Intersection(ray2);
        Assert.False(hit2, "Test quick intersection 2");
        Assert.False(intersection2 != null, "Test hit 2");

        var ray3 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(1.0f, 0.0f, 0.0f));
        var intersection3 = disk.Ray_Intersection(ray3);
        Assert.False((intersection3 != null), "Hit 3 transform disk");

        var ray4 = new Ray(new Point(0.0f, 0.0f, 1.0f), new Vec(0.0f, 1.0f, 0.0f));
        var intersection4 = disk.Ray_Intersection(ray4);
        Assert.False((intersection4 != null), "Hit 4 transform disk");
    }

    [Fact]
    public void TestIsInternal()
    {
        var disk = new Disk();
        Assert.True(disk.Is_Internal(new Point(0.2f, -0.5f, 0.0f)), "Test internal 1");
        
        disk.Tr = Transformation.Translation(new Vec(0.0f, 0.0f, 0.3f));
        Assert.False(disk.Is_Internal(new Point(0.2f, -0.5f, 0.0f)), "Test internal 2");
        
        disk.Tr = Transformation.Rotation_Y(90.0f);
        Assert.True(disk.Is_Internal(new Point(0.0f, -0.5f, 0.2f)), "Test internal 3");
        Assert.False(disk.Is_Internal(new Point(0.4f, -0.5f, 0.2f)), "Test internal 4");
    }
}

public class BoxTest
{
    [Fact]
    public void TestHit()
    {
        var box = new Box();
        var ray1 = new Ray(new Point(-0.5f, 0.5f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection1 = box.Ray_Intersection(ray1);
        var hit1 = box.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1 != null, "Check intersection 1");
        Assert.True(new HitRecord(
            new Point(-0.5f, 0.5f, 1.0f),
            new Normal(0.0f, 0.0f, 1.0f),
            1.0f,
            ray1,
            new Vec2D(0.3125f, 0.583333333f),
            new Material()
            ).Is_Close(intersection1), "Test hit 1");
        var intersections1 = box.Ray_Intersection_List(ray1);
        Assert.True(intersections1!.Count == 2, "Test list hits");
        Assert.True(intersections1[1].WorldPoint.Is_Close(new Point(-0.5f, 0.5f, -1.0f)), 
            "Test point second hit");
        Assert.True(intersections1[1].N.Is_Close(new Normal(0.0f, 0.0f, 1.0f)), 
            "Test normal second hit");
        Assert.True(Functions.Are_Close(intersections1[1].T, 3.0f), 
            "Test distance second hit");

        var ray2 = new Ray(new Point(0.0f, -0.5f, 0.5f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection2 = box.Ray_Intersection(ray2);
        var hit2 = box.Quick_Ray_Intersection(ray2);
        Assert.True(hit2, "Test quick intersection 2");
        Assert.True(intersection2 != null, "Check intersection 2");
        Assert.True(new HitRecord(
            new Point(-1.0f, -0.5f, 0.5f),
            new Normal(1.0f, 0.0f, 0.0f),
            1.0f,
            ray2,
            new Vec2D(0.1875f, 0.41666666666f),
            new Material()
        ).Is_Close(intersection2), "Test hit 2");

        var ray3 = new Ray(new Point(-2.0f, -0.5f, 0.5f), new Vec(-1.0f, 0.0f, 0.0f));
        var intersection3 = box.Ray_Intersection(ray3);
        var intersections3 = box.Ray_Intersection_List(ray3);
        var hit3 = box.Quick_Ray_Intersection(ray3);
        Assert.False(hit3, "Test quick intersection 3");
        Assert.False(intersection3 != null, "Check intersection 3");
        Assert.False(intersections3 != null, "Test list hits");
        
        var ray4 = new Ray(new Point(-2.0f, -0.5f, 0.5f), new Vec(0.0f, 1.0f, 0.0f));
        var intersection4 = box.Ray_Intersection(ray4);
        var hit4 = box.Quick_Ray_Intersection(ray4);
        Assert.False(hit4, "Test quick intersection 4");
        Assert.False(intersection4 != null, "Check intersection 4");
        
        // Check with all faces to find bugs
        var ray5 = new Ray(new Point(0.8f, 2.0f, -0.8f), new Vec(0.0f, -1.0f, 0.0f));
        var hit5 = box.Quick_Ray_Intersection(ray5);
        Assert.True(hit5, "Test 5");
        var ray6 = new Ray(new Point(-0.8f, 2.0f, 0.8f), new Vec(0.0f, -1.0f, 0.0f));
        var hit6 = box.Quick_Ray_Intersection(ray6);
        Assert.True(hit6, "Test 6");
        var ray7 = new Ray(new Point(0.8f, 2.0f, 0.8f), new Vec(0.0f, -1.0f, 0.0f));
        var hit7 = box.Quick_Ray_Intersection(ray7);
        Assert.True(hit7, "Test 7");
        var ray8 = new Ray(new Point(-0.8f, 2.0f, -0.8f), new Vec(0.0f, -1.0f, 0.0f));
        var hit8 = box.Quick_Ray_Intersection(ray8);
        Assert.True(hit8, "Test 8");
    }

    [Fact]
    public void TestTransformation()
    {
        var box = new Box(T: Transformation.Translation(new Vec(10.0f, 0.0f, 0.0f)));
        var ray1 = new Ray(new Point(9.5f, 0.5f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection1 = box.Ray_Intersection(ray1);
        var hit1 = box.Quick_Ray_Intersection(ray1);
        Assert.True(hit1, "Test quick intersection 1");
        Assert.True(intersection1 != null, "Test intersection 1");
        Assert.True(new HitRecord(
            new Point(9.5f, 0.5f, 1.0f),
            new Normal(0.0f, 0.0f, 1.0f),
            1.0f,
            ray1,
            new Vec2D(0.3125f, 0.583333333f), 
            new Material()
        ).Is_Close(intersection1), "Test hit 1");
        
        var ray2 = new Ray(new Point(-0.5f, 0.5f, 2.0f), new Vec(0.0f, 0.0f, -1.0f));
        var intersection2 = box.Ray_Intersection(ray2);
        var hit2 = box.Quick_Ray_Intersection(ray2);
        Assert.False(hit2, "Test quick intersection 2");
        Assert.False(intersection2 != null, "Check intersection 2");
    }
    
    [Fact]
    public void TestIsInternal()
    {
        var box = new Box();
        Assert.True(box.Is_Internal(new Point(0.4f, -0.95f, 0.0f)), "Test internal 1");
        Assert.False(box.Is_Internal(new Point(0.2f, -0.95f, -1.1f)), "Test internal 2");
        
        box.Tr = Transformation.Translation(new Vec(0.0f, 0.0f, 0.3f));
        Assert.False(box.Is_Internal(new Point(0.2f, -0.95f, -0.8f)), "Test internal 3");
        Assert.True(box.Is_Internal(new Point(0.4f, -0.5f, 0.2f)), "Test internal 4");
    }
}