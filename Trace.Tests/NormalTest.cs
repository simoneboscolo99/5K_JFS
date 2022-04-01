using Xunit;

namespace Trace.Tests;

public class NormalTest
{
    Normal _a = new(1.0f, 2.0f, 3.0f);
    Normal _b = new(5.0f, 3.0f, 8.0f);
    Vec _v = new(5.0f, 3.0f, 8.0f);
    float scalar = 2.0f;


    [Fact]
    public void TestNeg()
    {
        Normal control = new(-1.0f, -2.0f, -3.0f);
        var c = _a.Neg();
        Assert.True(c.Is_Close(control), "Function Neg doesn't work");
        Assert.True((scalar * _b).Is_Close(new Normal(10.0f, 6.0f, 16.0f)), "Test scalar*normal");
        Assert.True((_a * scalar).Is_Close(new Normal(2.0f, 4.0f, 6.0f)), "Test normal*scalar");
        Assert.True(Functions.Are_Close(35.0f, _a.Dot(_v)), "Test dot product(normals)");
        Assert.True(_a.Cross(_v).Is_Close(new Vec(-2.0f, 4.0f, -2.0f)), "Test cross product 1");
        
        
    }
    
}