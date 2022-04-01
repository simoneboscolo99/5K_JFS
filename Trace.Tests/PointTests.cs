using Xunit;

namespace Trace.Tests;

public class PointTests
{
    Point a = new(1.0f, 2.0f, 3.0f);
    Point b = new(4.0f, 6.0f, 8.0f);

    [Fact]
    public void TestPoints()
    {
        Assert.True(a.Is_Close(a), "Test 1");
        Assert.False(a.Is_Close(b), "Test 2");
    }
    
}