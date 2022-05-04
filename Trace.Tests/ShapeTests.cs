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