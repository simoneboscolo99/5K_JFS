using System;
using Xunit;
using Xunit.Abstractions;

namespace Trace.Tests;

public class NormalTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    Normal _a = new(1.0f, 2.0f, 3.0f);
    Normal _aNorm = new( 1f / 3.7416575f, 2f / 3.7416575f, 3f / 3.7416575f);
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
        Assert.True(_a.Cross(_v).Is_Close(new Normal(7.0f, 7.0f, -7.0f)), "Test cross product 1");
        Assert.True(_a.Cross(_b).Is_Close(new Normal(7.0f, 7.0f, -7.0f)), "Test cross product 1"); 
        Assert.True(Functions.Are_Close(_a.SquaredNorm(),14.0f),"SquaredNorm doesn't work");
        _testOutputHelper.WriteLine($"{_a.Norm()}");
        Assert.True(Functions.Are_Close(_a.Norm(),(float) Math.Sqrt(14.0f)),"Norm doesnt work");
        Assert.True(_aNorm.Is_Close(_a.Normalize()));
        
    }
    
}
