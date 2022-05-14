using Xunit;

namespace Trace.Tests;

public class SolverTests
{
    [Fact]
    public void TestOnOffRenderer()
    {
        var sphere = new Sphere(
            Transformation.Translation(new Vec(2.0f, 0.0f, 0.0f)) * Transformation.Scale(new Vec(0.2f, 0.2f, 0.2f)),
             new Material(new DiffuseBrdf(new UniformPigment(Color.White))));
        var image = new HdrImage(3, 3);
        var camera = new OrthogonalCamera();
        var tracer = new ImageTracer(image, camera);
        var world = new World();
        world.Add(sphere);
        var renderer = new OnOffTracing(world);
        tracer.Fire_All_Rays(renderer);
        
        Assert.True(image.Get_Pixel(0, 0).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(1, 0).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(2, 0).Is_Close(Color.Black));

        Assert.True(image.Get_Pixel(0, 1).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(1, 1).Is_Close(Color.White));
        Assert.True(image.Get_Pixel(2, 1).Is_Close(Color.Black));

        Assert.True(image.Get_Pixel(0, 2).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(1, 2).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(2, 2).Is_Close(Color.Black));
    }

    [Fact]
    public void TestFlatRenderer()
    {
        var sphereColor = new Color(1.0f, 2.0f, 3.0f);
        var sphere = new Sphere(Transformation.Translation(new Vec(2.0f, 0.0f, 0.0f))
                                * Transformation.Scale(new Vec(0.2f, 0.2f, 0.2f)),
            new Material(new DiffuseBrdf(new UniformPigment(sphereColor))));
        var image = new HdrImage(3, 3);
        var camera = new OrthogonalCamera();
        var tracer = new ImageTracer(image, camera);
        var world = new World();
        world.Add(sphere);
        var renderer = new FlatTracing(world);
        tracer.Fire_All_Rays(renderer);
        
        Assert.True(image.Get_Pixel(0, 0).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(1, 0).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(2, 0).Is_Close(Color.Black));
        
        Assert.True(image.Get_Pixel(0, 1).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(1, 1).Is_Close(sphereColor));
        Assert.True(image.Get_Pixel(2, 1).Is_Close(Color.Black));
        
        Assert.True(image.Get_Pixel(0, 2).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(1, 2).Is_Close(Color.Black));
        Assert.True(image.Get_Pixel(2, 2).Is_Close(Color.Black));
    }
    
}