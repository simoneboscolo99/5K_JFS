using Xunit;

namespace Trace.Tests;

public class RayTests
{
    [Fact]
    public void TestRay()
    {
        Ray ray1 = new (new Point(1.0f, 2.0f, 3.0f), new Vec(5.0f, 4.0f, -1.0f));
        Ray ray2 = new (new Point(1.0f, 2.0f, 3.0f), new Vec(5.0f, 4.0f, -1.0f));
        Ray ray3 = new(new Point(5.0f, 1.0f, 4.0f), new Vec(3.0f, 9.0f, 4.0f));
        Assert.True(ray1.Is_Close(ray2), "Test ray1-ray2");
        Assert.False(ray1.Is_Close(ray3), "Test ray1-ray3");
    }
    
    [Fact]
    public void TestAt()
    {
        Ray ray = new(new Point(1.0f, 2.0f, 4.0f), new Vec(4.0f, 2.0f, 1.0f));
        const float t0 = 0.0f;
        const float t1 = 1.0f;
        const float t2 = 2.0f;
        Assert.True(ray.At(t0).Is_Close(ray.Origin), "Test ray origin");
        Assert.True(ray.At(t1).Is_Close(new Point(5.0f, 4.0f, 5.0f)), "Test point 1");
        Assert.True(ray.At(t2).Is_Close(new Point(9.0f, 6.0f, 6.0f)), "Test point 2");
    }

    [Fact]
    public void TestTransformRay()
    {
        Ray ray = new(new Point(1.0f, 2.0f, 3.0f), new Vec(6.0f, 5.0f, 4.0f));
        var tr = Transformation.Translation(new(10.0f, 11.0f, 12.0f))*Transformation.Rotation_X(90f);
        var rayTr= tr*ray;
        Assert.True(rayTr.Origin.Is_Close(new Point(11.0f, 8.0f, 14.0f)), "Test transform origin");
        Assert.True(rayTr.Dir.Is_Close(new (6.0f, -4.0f, 5.0f)), "Test transform dir");
    }
}