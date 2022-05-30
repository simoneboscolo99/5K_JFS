//using System;
//using System.Numerics;
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
        var inv = new Matrix4x4(-3.75f, 2.75f, -1.0f, 0.0f, 4.375f, -3.875f, 2.0f, -0.5f, 0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f);
        var m1 = new Transformation(m, inv);
        Assert.True(m1.Is_Consistent(), "Test consistent");
        var m2 = m1.Clone();
        Assert.True(m1.Is_Close(m2), "Test consistent");
        var m3 = m1.Clone();
        m3.M.M22 += 1;
        Assert.False(m1.Is_Close(m3), "Test is not consistent");
        var m4 = m1.Clone();
        m4.InvM.M22 += 1;
        Assert.False(m1.Is_Close(m4), "Test is not consistent");
    }

    [Fact]
    public void TestMultiplication()
    {
        var m = new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 9.0f, 8.0f, 7.0f, 6.0f, 5.0f, 4.0f,
            1.0f);
        var inv = new Matrix4x4(-3.75f, 2.75f, -1.0f, 0.0f, 4.375f, -3.875f, 2.0f, -0.5f, 0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f);
        var m1 = new Transformation(m, inv);
        Assert.True(m1.Is_Consistent(), "Test consistent");
        var n = new Matrix4x4(3.0f, 5.0f, 2.0f, 4.0f, 4.0f, 1.0f, 0.0f, 5.0f, 6.0f, 3.0f, 2.0f, 0.0f, 1.0f, 4.0f,
            2.0f,
            1.0f);
        var inv2 = new Matrix4x4(0.4f, -0.2f, 0.2f, -0.6f, 2.9f, -1.7f, 0.2f, -3.1f, -5.55f, 3.15f, -0.4f, 6.45f,
            -0.9f, 0.7f, -0.2f, 1.1f);
        var m2 = new Transformation(n, inv2);
        Assert.True(m2.Is_Consistent(), "Test consistent");
        var mExpected = new Matrix4x4(33.0f, 32.0f, 16.0f, 18.0f, 89.0f, 84.0f, 40.0f, 58.0f, 118.0f, 106.0f, 48.0f,
            88.0f, 63.0f, 51.0f, 22.0f, 50.0f);
        var invExpected = new Matrix4x4(-1.45f, 1.45f, -1.0f, 0.6f, -13.95f, 11.95f, -6.5f, 2.6f, 25.525f, -22.025f,
            12.25f, -5.2f, 4.825f, -4.325f, 2.5f, -1.1f);
        var expected = new Transformation(mExpected, invExpected);
        //Assert.True(expected.Is_Consistent(), "Test Consistent");
        Assert.True(expected.Is_Close(m1 * m2), "Test consistent");
    }

    [Fact]
    public void TestVecPointMultiplication()
    {
        var m = new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 9.0f, 8.0f, 7.0f, 0.0f, 0.0f, 0.0f,
            1.0f);
        var inv = new Matrix4x4(-3.75f, 2.75f, -1.0f, 0.0f, 5.75f, -4.75f, 2.0f, 1.0f, -2.25f, 2.25f, -1.0f, -2.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
        var m1 = new Transformation(m, inv);
        Assert.True(m1.Is_Consistent(), "Test consistent");
        Vec expectedV = new(14.0f, 38.0f, 51.0f);
        Assert.True(expectedV.Is_Close(m1 * new Vec(1.0f, 2.0f, 3.0f)), "Test consistent");
        Point expectedP = new(18.0f, 46.0f, 58.0f);
        Assert.True(expectedP.Is_Close(m1 * new Point(1.0f, 2.0f, 3.0f)), "Test consistent");
        Normal expectedN = new(-8.75f, 7.75f, -3.0f);
        Assert.True(expectedN.Is_Close(m1 * new Normal(3.0f, 2.0f, 4.0f)), "Test consistent");
    }

    [Fact]
    public void TestInverse()
    {
        var m = new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 9.0f, 8.0f, 7.0f, 6.0f, 5.0f, 4.0f,
            1.0f);
        var invM = new Matrix4x4(-3.75f, 2.75f, -1.0f, 0.0f, 4.375f, -3.875f, 2.0f, -0.5f, 0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f);
        var m1 = new Transformation(m, invM);
        var m2 = m1.Inverse;
        Assert.True(m2.Is_Consistent(), "Test consistent");
        var prod = m1 * m2;
        Assert.True(prod.Is_Consistent(), "Test consistent");
        Assert.True(prod.Is_Close(Transformation.Identity()), "Test consistent");
    }

    [Fact]
    public void TestTranslations()
    {
        var tr1 = Transformation.Translation(new Vec(1.0f, 2.0f, 3.0f));
        Assert.True(tr1.Is_Consistent(), "Test consistent");
        var tr2 = Transformation.Translation(new Vec(4.0f, 6.0f, 8.0f));
        Assert.True(tr2.Is_Consistent(), "Test consistent");
        var prod = tr1 * tr2;
        Assert.True(prod.Is_Consistent(), "Test consistent");
        var expected = Transformation.Translation(new Vec(5.0f, 8.0f, 11.0f));
        Assert.True(prod.Is_Close(expected), "Test consistent");
    }

    [Fact]

    public void TestRotations()
    {
        Assert.True(Transformation.Rotation_X(0.1f).Is_Consistent(), "Test consistent");
        Assert.True(Transformation.Rotation_Y(0.1f).Is_Consistent(), "Test consistent");
        Assert.True(Transformation.Rotation_Z(0.1f).Is_Consistent(), "Test consistent");
        Assert.True(
            ((Transformation.Rotation_X(90.0f)) * (new Vec(0.0f, 1.0f, 0.0f))).Is_Close(new Vec(0.0f, 0.0f, 1.0f)),
            "Test consistent");
        Assert.True(
            ((Transformation.Rotation_Y(90.0f)) * (new Vec(0.0f, 0.0f, 1.0f))).Is_Close(new Vec(1.0f, 0.0f, 0.0f)),
            "Test consistent");
        Assert.True(
            ((Transformation.Rotation_Z(90.0f)) * (new Vec(1.0f, 0.0f, 0.0f))).Is_Close(new Vec(0.0f, 1.0f, 0.0f)),
            "Test consistent");
    }

    [Fact]
    public void TestScale()
    {
        var tr1 = Transformation.Scale(new Vec(2.0f, 5.0f, 10.0f));
        Assert.True(tr1.Is_Consistent(), "Test consistent");
        var tr2 = Transformation.Scale(new Vec(3.0f, 2.0f, 4.0f));
        Assert.True(tr2.Is_Consistent(), "Test consistent");
        var expected = Transformation.Scale(new Vec(6.0f, 10.0f, 40.0f));
        Assert.True(expected.Is_Close(tr1 * tr2), "Test consistent");
    }

    [Fact]
    public void TestIdentity()
    {
        var m = new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        var inv = new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        var m1 = new Transformation(m, inv);
        var m2 = Transformation.Identity();
        Assert.True(condition: m1.Is_Close(m2));
    }
}