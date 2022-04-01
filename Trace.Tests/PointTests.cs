using Xunit;

namespace Trace.Tests;

public class PointTests
{
    Point a = new(1.0f, 2.0f, 3.0f);
    Point b = new(4.0f, 6.0f, 8.0f);
    float scalar = 2.0f;

    [Fact]
    public void TestPoints()
    {
        Assert.True(a.Is_Close(a), "Test constructor 1");
        Assert.False(a.Is_Close(b), "Test constructor 2");
    }

    [Fact]
    public void Test_Point_Operations()
    {
        Assert.True((scalar * b).Is_Close(new Point(8.0f, 12.0f, 16.0f)), "Test scalar*Point");
        Assert.True((a * scalar).Is_Close(new Point(2.0f, 4.0f, 6.0f)), "Test Point*scalar");
        Assert.False((scalar * b).Is_Close(new Point(9.0f, 12.0f, 16.0f)), "scalar*Point does not work");
        Assert.False((a * scalar).Is_Close(new Point(2.0f, 5.0f, 6.0f)), "Point*scalar does not work");
        Assert.True((a + b).Is_Close(new Point(5.0f, 8.0f, 11.0f)), "Test Point+Point");
        Assert.False((a + b).Is_Close(new Point(5.0f, 9.0f, 11.0f)), "Point+Point does not work");
        Assert.True((b - a).Is_Close(new Vec(3.0f, 4.0f, 5.0f)), "Test Point-Point");
        Assert.False((b - a).Is_Close(new Vec(3.0f, 4.0f, 6.0f)), "Point-Point does not work");
    }
    
}