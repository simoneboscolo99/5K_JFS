using System.Collections.Generic;
using Xunit;

namespace Trace.Tests;

public class PcgTests
{
    [Fact]
    public void TestRandom()
    {
        var pcg = new Pcg();
        Assert.True(pcg.State == 1753877967969059832, "Test state");
        Assert.True(pcg.Inc == 109, "Test inc");

        var expected = new List<uint> { 
            2707161783,
            2068313097,
            3122475824,
            2211639955,
            3215226955,
            3421331566,};
        foreach (var x in expected) Assert.True(pcg.Random() == x, $"Test random");
    }
}