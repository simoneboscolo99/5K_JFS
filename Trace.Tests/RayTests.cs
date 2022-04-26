using Xunit;

namespace Trace.Tests;

public class RayTests
{
    Ray _ray1 = new (new (1.0f, 2.0f, 3.0f), new(5.0f, 4.0f, -1.0f));
    Ray _ray2 = new (new (1.0f, 2.0f, 3.0f), new(5.0f, 4.0f, -1.0f));
    Ray _ray3 = new(new (5.0f, 1.0f, 4.0f), new(3.0f, 9.0f, 4.0f));

    [Fact]
    public void Test_Ray()
    {
        Assert.True(_ray1.Is_Close(_ray2));
    }
    
    [Fact]
    public void Test_At()
    {
        Ray ray = new(new(1.0f, 2.0f, 4.0f), new(4.0f, 2.0f, 1.0f));
        float t_0 = 0.0f;
        float t_1 = 1.0f;
        float t_2 = 2.0f;
        Assert.True(ray.At(t_0).Is_Close(ray.Origin));
        Assert.True(ray.At(t_1).Is_Close(new Point(5.0f, 4.0f, 5.0f)));
        Assert.True(ray.At(t_2).Is_Close(new Point(9.0f, 6.0f, 6.0f)));
        
    }

    [Fact]
    public void Test_Transform_Ray()
    {
        Ray ray = new(new Point(1.0f, 2.0f, 3.0f), new Vec(6.0f, 5.0f, 4.0f));
        var tr = Transformation.Translation(new(10.0f, 11.0f, 12.0f))*Transformation.Rotation_X(90f);
        var rayTr= tr*ray;
        Assert.True(rayTr.Origin.Is_Close(new Point(11.0f, 8.0f, 14.0f)));
        Assert.True(rayTr.Dir.Is_Close(new (6.0f, -4.0f, 5.0f)));
    }
}