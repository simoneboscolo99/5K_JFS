using Xunit;

namespace Trace.Tests;

public class ColorTests
{
    Color a = new(1.0f, 2.0f, 3.0f);
    Color b = new(5.0f, 3.0f, 8.0f);
    float scalar = 2.0f;

    [Fact]
    public void TestCreateAndClose()
    {
        Assert.True(a.Is_Close(new Color(1.0f, 2.0f, 3.0f)), "Test creation");
        Assert.False(a.Is_Close(new Color(3.0f, 4.0f, 5.0f)), "Test is_close function");
    }

    [Fact]
    public void TestAdd()
    {
        Assert.True((a + b).Is_Close(new Color(6.0f, 5.0f, 11.0f)), "Test 1");
        Assert.False((a + b).Is_Close(new Color(6.0f, 5.0f, 10.0f)), "Test 2");
    }

    [Fact]
    public void TestDiff()
    {
        Assert.True((b - a).Is_Close(new Color(4.0f, 1.0f, 5.0f)), "Test 1");
        Assert.False((b - a).Is_Close(new Color(4.0f, 0.0f, 5.0f)), "Test 2");
    }

    [Fact]
    public void TestScalar()
    {
        Assert.True((scalar * b).Is_Close(new Color(10.0f, 6.0f, 16.0f)), "Test 1");
        Assert.True((a * scalar).Is_Close(new Color(2.0f, 4.0f, 6.0f)), "Test 2");
        Assert.False((scalar * b).Is_Close(new Color(9.0f, 6.0f, 16.0f)), "Test 3");
        Assert.False((a * scalar).Is_Close(new Color(2.0f, 5.0f, 6.0f)), "Test 4");
    }

    [Fact]
    public void TestMult()
    {
        Assert.True((a * b).Is_Close(new Color(5.0f, 6.0f, 24.0f)), "Test 1");
        Assert.False((a * b).Is_Close(new Color(4.0f, 6.0f, 24.0f)), "Test 2");
    }

    [Fact]
    public void TestLuminosity()
    {
        Color col1 = new Color(1.0f, 2.0f, 3.0f);
        Color col2 = new Color(9.0f, 5.0f, 7.0f);
        Assert.True(Functions.Are_Close(col1.Luminosity(), 2.0f), "Luminosity does not work");
        Assert.True(Functions.Are_Close(col2.Luminosity(), 7.0f), "Luminosity does not work");

    }
}