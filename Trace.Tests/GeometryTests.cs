using Xunit;
using System;
using Xunit.Abstractions;

namespace Trace.Tests;

public class VecTests
{
    [Fact]
    public void TestVec()
    {
        var v = new Vec(1.0f, 2.0f, 3.0f);
        var w = new Vec(4.0f, 6.0f, 8.0f);
        Assert.True(v.Is_Close(new Vec(1.0f, 2.0f, 3.0f)), "Test constructor vec v");
        Assert.True(w.Is_Close(new Vec(4.0f, 6.0f, 8.0f)), "Test constructor vec w");
    }

    [Fact]
    public void TestVecOperations()
    {
        var v = new Vec(1.0f, 2.0f, 3.0f);
        var w = new Vec(4.0f, 6.0f, 8.0f);
        var m = new Normal(5.0f, 3.0f, 8.0f);
        Assert.True((v + w).Is_Close(new Vec(5.0f, 8.0f, 11.0f)), "Test add");
        Assert.True((w - v).Is_Close(new Vec(3.0f, 4.0f, 5.0f)), "Test diff");
        Assert.True((v * 2.0f).Is_Close(new Vec(2.0f, 4.0f, 6.0f)), "Test scalar mult 1");
        Assert.True((2.0f * v).Is_Close(new Vec(2.0f, 4.0f, 6.0f)), "Test scalar mult 2");
        Assert.True((-v).Is_Close(new Vec(-1.0f, -2.0f, -3.0f)), "Test neg 2");
        Assert.True(Functions.Are_Close(v.Dot(w), 40.0f), "Test scalar product");
        Assert.True(Functions.Are_Close(35.0f, v.Dot(m)), "Test scalar vec*normal");
        Assert.True(v.Cross(w).Is_Close(new Vec(-2.0f, 4.0f, -2.0f)), "Test cross product 1");
        Assert.True(Vec.Cross(v, w).Is_Close(new Vec(-2.0f, 4.0f, -2.0f)), "Test cross product 2");
        Assert.True(w.Cross(v).Is_Close(new Vec(2.0f, -4.0f, 2.0f)), "Test cross product 3");
        Assert.True(Vec.Cross(w, v).Is_Close(new Vec(2.0f, -4.0f, 2.0f)), "Test cross product 2");
        Assert.True(Functions.Are_Close(v.Squared_Norm(), 14.0f), "Test squared norm");
        Assert.True(Functions.Are_Close(v.Norm() * v.Norm(), 14.0f), "Test squared norm");
        Assert.True(Functions.Are_Close(v.Normalize().Norm(), 1.0f), "Test Normalization");
        Assert.True(v.ToNormal().Is_Close(new Normal(1.0f, 2.0f, 3.0f)), "Test to Normal");
    }

}


public class PointTests
{
    Point _a = new(1.0f, 2.0f, 3.0f);
    Point b = new(4.0f, 6.0f, 8.0f);
    float scalar = 2.0f;
    private Vec c = new(4.0f, 6.0f, 8.0f);

    [Fact]
    public void TestPoints()
    {
        Assert.True(_a.Is_Close(_a), "Test constructor 1");
        Assert.False(_a.Is_Close(b), "Test constructor 2");
    }

    [Fact]
    public void Test_Point_Operations()
    {
        Assert.True((scalar * b).Is_Close(new Point(8.0f, 12.0f, 16.0f)), "Test scalar*Point");
        Assert.True((_a * scalar).Is_Close(new Point(2.0f, 4.0f, 6.0f)), "Test Point*scalar");
        Assert.False((scalar * b).Is_Close(new Point(9.0f, 12.0f, 16.0f)), "scalar*Point does not work");
        Assert.False((_a * scalar).Is_Close(new Point(2.0f, 5.0f, 6.0f)), "Point*scalar does not work");
        Assert.True((_a + b).Is_Close(new Point(5.0f, 8.0f, 11.0f)), "Test Point+Point");
        Assert.False((_a + b).Is_Close(new Point(5.0f, 9.0f, 11.0f)), "Point+Point does not work");
        Assert.True((b - _a).Is_Close(new Vec(3.0f, 4.0f, 5.0f)), "Test Point-Point");
        Assert.False((b - _a).Is_Close(new Vec(3.0f, 4.0f, 6.0f)), "Point-Point does not work");
        Assert.True((_a + c).Is_Close(new Point(5.0f, 8.0f, 11.0f)), "Test Point+Vec");
        Assert.False((_a + c).Is_Close(new Point(5.0f, 10.0f, 11.0f)), "Point+Vec does not work");
        Assert.True((_a - c).Is_Close(new Point(-3.0f, -4.0f, -5.0f)), "Test Point-Vec");
        Assert.False((_a - c).Is_Close(new Point(3.0f, -4.0f, -5.0f)), "Point-Vec does not work");
    }

}


public class NormalTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    Normal _a = new(1.0f, 2.0f, 3.0f);
    Normal _aNorm = new(1f / 3.7416575f, 2f / 3.7416575f, 3f / 3.7416575f);
    Normal _b = new(5.0f, 3.0f, 8.0f);
    Vec _v = new(5.0f, 3.0f, 8.0f);
    private float scalar = 2.0f;

    public NormalTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestNormal()
    {
        Normal negControl = new(-1.0f, -2.0f, -3.0f);
        Assert.True(negControl.Is_Close(-_a), "Function Neg doesn't work");
        Assert.True((scalar * _b).Is_Close(new Normal(10.0f, 6.0f, 16.0f)), "Test scalar*normal");
        Assert.True((_a * scalar).Is_Close(new Normal(2.0f, 4.0f, 6.0f)), "Test normal*scalar");
        Assert.True(Functions.Are_Close(35.0f, _a.Dot(_v)), "Test dot product(normals)");
        Assert.True(Functions.Are_Close(35.0f, _a.Dot(_b)), "Test dot product(normals)");
        Assert.True(_a.Cross(_v).Is_Close(new Normal(7.0f, 7.0f, -7.0f)), "Test cross product 1(V,N)");
        Assert.True(_a.Cross(_b).Is_Close(new Normal(7.0f, 7.0f, -7.0f)), "Test cross product 1(N,N)");
        Assert.True(Functions.Are_Close(_a.SquaredNorm(), 14.0f), "SquaredNorm doesn't work");
        _testOutputHelper.WriteLine($"{_a.Norm()}");
        Assert.True(Functions.Are_Close(_a.Norm(), (float)Math.Sqrt(14.0f)), "Norm doesnt work");
        Assert.True(_aNorm.Is_Close(_a.Normalize()), "Normalization problem(Normal class)");
    }
}