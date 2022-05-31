using Xunit;

namespace Trace.Tests;

public class PigmentsTests
{
    [Fact]
    public void Test_Uniform_Pigment()
    {
        var color = new Color(1.0f, 2.0f, 3.0f);
        var pigment = new UniformPigment(color);
        Assert.True(pigment.Get_Color(new Vec2D(0.0f, 0.0f)).Is_Close(color));
        Assert.True(pigment.Get_Color(new Vec2D(1.0f, 0.0f)).Is_Close(color));
        Assert.True(pigment.Get_Color(new Vec2D(0.0f, 1.0f)).Is_Close(color));
        Assert.True(pigment.Get_Color(new Vec2D(1.0f, 1.0f)).Is_Close(color));
    }

    [Fact]
    public void Test_Image_Pigment()
    {
        var image = new HdrImage( 2, 2);
        image.Set_Pixel(0, 0, new Color(1.0f, 2.0f, 3.0f));
        image.Set_Pixel(1, 0, new Color(2.0f, 3.0f, 1.0f));
        image.Set_Pixel(0, 1, new Color(2.0f, 1.0f, 3.0f));
        image.Set_Pixel(1, 1, new Color(3.0f, 2.0f, 1.0f));
        var pigment = new ImagePigment(image);
        Assert.True(pigment.Get_Color(new Vec2D(0.0f, 0.0f))
            .Is_Close(new Color(1.0f, 2.0f, 3.0f))); 
        Assert.True(pigment.Get_Color(new Vec2D(1.0f, 0.0f))
            .Is_Close(new Color(2.0f, 3.0f, 1.0f)));
        Assert.True(pigment.Get_Color(new Vec2D(0.0f, 1.0f))
            .Is_Close(new Color(2.0f, 1.0f, 3.0f)));
        Assert.True(pigment.Get_Color(new Vec2D(1.0f, 1.0f))
            .Is_Close(new Color(3.0f, 2.0f, 1.0f)));
    }

    [Fact]
    public void Test_Checkered_Pigment()
    {
        var color1 = new Color(1.0f, 2.0f, 3.0f);
        var color2 = new Color(10.0f, 20.0f, 30.0f);

        var pigment = new CheckeredPigment(color1, color2, 2);

// With num_of_steps == 2, the pattern should be the following:
        //
//              (0.5, 0)
//   (0, 0) +------+------+ (1, 0)
//          |      |      |
//          | col1 | col2 |
//          |      |      |
// (0, 0.5) +------+------+ (1, 0.5)
//          |      |      |
//          | col2 | col1 |
//          |      |      |
//   (0, 1) +------+------+ (1, 1)
//              (0.5, 1)

        Assert.True(pigment.Get_Color(new Vec2D(0.25f, 0.25f)).Is_Close(color1)); 
        Assert.True(pigment.Get_Color(new Vec2D(0.75f, 0.25f)).Is_Close(color2)); 
        Assert.True(pigment.Get_Color(new Vec2D(0.25f, 0.75f)).Is_Close(color2)); 
        Assert.True(pigment.Get_Color(new Vec2D(0.75f, 0.75f)).Is_Close(color1)); 
      }
}

