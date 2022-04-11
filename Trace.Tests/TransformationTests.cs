using System.Numerics;
using Xunit;

namespace Trace.Tests;
using System.Numerics;

public class TransformationTests
{
    [Fact]
    public void TestIsClose()
    {
        var m = new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 9.0f, 8.0f, 7.0f, 6.0f, 5.0f, 4.0f,
            1.0f);
        var invm = new Matrix4x4(-3.75f, 2.75f, -1.0f, 0.0f, 4.375f, -3.875f, 2.0f, -0.5f, 0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f);
        var m1 = new Transformation(m, invm);
        Assert.True(m1.Is_Consistent(), "Test consistent");
        //var m2 = new Transformation(m, invm);
        var m2 = m1.Clone();
        Assert.True(m1.Is_Close(m2), "Test consistent");
    }

    [Fact]
    public void Test_Inverse()
    {
        var m = new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 9.0f, 8.0f, 7.0f, 6.0f, 5.0f, 4.0f,
            1.0f);
        var invm = new Matrix4x4(-3.75f, 2.75f, -1.0f, 0.0f, 4.375f, -3.875f, 2.0f, -0.5f, 0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f);
        var m1 = new Transformation(m, invm);
        var m2 = m1.Inverse;
        Assert.True(m2.Is_Consistent(), "Test consistent");
    }

    [Fact]
    public void Test_Translation()
    {
        var m = new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 9.0f, 8.0f, 7.0f, 6.0f, 5.0f, 4.0f,
            1.0f);
        var invm = new Matrix4x4(-3.75f, 2.75f, -1.0f, 0.0f, 4.375f, -3.875f, 2.0f, -0.5f, 0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f);
        var m1 = new Transformation(m, invm);
        var m2 = m1.Inverse;
        Assert.True(m2.Is_Consistent(), "Test consistent");
    }

    [Fact]
    public void TestScale()
    {
        var v = new Vec(1f, 2f, 3f);
        var m = new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 9.0f, 8.0f, 7.0f, 6.0f, 5.0f, 4.0f, 1.0f);
        var invm = new Matrix4x4(-3.75f, 2.75f, -1.0f, 0.0f, 4.375f, -3.875f, 2.0f, -0.5f, 0.5f, 0.5f, -1.0f, 1.0f, -1.375f, 0.875f, 0.0f, -0.5f);
        var t = new Transformation(m, invm);
        var scaleT = t.Scale(v);
        Assert.True(scaleT.Is_Consistent());

    }

    [Fact]
    public void TestIdentity()
    {
        var t = new Matrix4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);
        var invent = new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        var tra = new Transformation(t, invent);
        var idt = Transformation.Identity();
        Assert.True(condition: tra.Is_Close(idt));
    }

    [Fact]
    public void TestRotations()
    {
        Assert.True(Transformation.Rotation_X(0.1f).Is_Consistent());
        Assert.True(Transformation.Rotation_Y(0.1f).Is_Consistent());
        Assert.True(Transformation.Rotation_Z(0.1f).Is_Consistent());
    }
}