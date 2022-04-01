using Xunit;

namespace Trace.Tests;

public class VecTests
{
    [Fact]
    public void TestVec()
    {
        var v = new Vec(1.0f, 2.0f, 3.0f);
        var w = new Vec(4.0f, 6.0f, 8.0f);
        Assert.True(v.Is_Close(new Vec(1.0f, 2.0f, 3.0f)), "Test constructor vec v");
        Assert.True(w.Is_Close(new Vec(4.0f, 6.0f, 8.0f)),"Test constructor vec w");
    }
    
    [Fact]
    public void TestOperations()
    {
        var v = new Vec(1.0f, 2.0f, 3.0f);
        var w = new Vec(4.0f, 6.0f, 8.0f);
        var m = new Normal(5.0f, 3.0f, 8.0f);
        Assert.True((v + w).Is_Close(new Vec(5.0f, 8.0f, 11.0f)), "Test add");
        Assert.True((w - v).Is_Close(new Vec(3.0f, 4.0f, 5.0f)), "Test diff");
        Assert.True((v * 2.0f).Is_Close(new Vec(2.0f, 4.0f, 6.0f)), "Test scalar mult 1");
        Assert.True((2.0f * v).Is_Close(new Vec(2.0f, 4.0f, 6.0f)), "Test scalar mult 2");
        Assert.True(v.Neg().Is_Close(new Vec(-1.0f, -2.0f, -3.0f)), "Test neg 1");
        Assert.True((-v).Is_Close(new Vec(-1.0f, -2.0f, -3.0f)), "Test neg 2");
        Assert.True(Functions.Are_Close(v.Dot(w), 40.0f), "Test scalar product");
        Assert.True(Functions.Are_Close(35.0f,v.Dot(m)), "Test scalar vec*normal");
        Assert.True(v.Cross(w).Is_Close(new Vec(-2.0f, 4.0f, -2.0f)), "Test cross product 1");
        Assert.True(Vec.Cross(v,w).Is_Close(new Vec(-2.0f, 4.0f, -2.0f)), "Test cross product 2");
        Assert.True(w.Cross(v).Is_Close(new Vec(2.0f, -4.0f, 2.0f)), "Test cross product 3");
        Assert.True(Vec.Cross(w,v).Is_Close(new Vec(2.0f, -4.0f, 2.0f)), "Test cross product 2");
        Assert.True(Functions.Are_Close(v.Squared_Norm(), 14.0f), "Test squared norm");
        Assert.True(Functions.Are_Close(v.Norm() * v.Norm(), 14.0f), "Test squared norm");
    }
    
}