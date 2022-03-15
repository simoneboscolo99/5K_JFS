using System.Runtime.CompilerServices;
using Xunit;

namespace Trace.Tests;

public class ColorTests
{
    [Fact]
    public void TestAdd()
    {
        Color a = new Color(1.0f, 2.0f, 3.0f);
        Color b = new Color(5.0f, 6.0f, 7.0f);
        float c = 2.0f;

        Assert.True((a + b).Is_Close(new Color (6.0f, 8.0f, 10.0f)));
        Assert.True((a - b).Is_Close(new Color(4.0f, 4.0f, 4.0f)));
        Assert.True((c * a).Is_Close(new Color(2.0f, 4.0f, 6.0f)));
        Assert.True((a * b).Is_Close(new Color(5.0f, 6.0f, 21.0f)));

    }
}