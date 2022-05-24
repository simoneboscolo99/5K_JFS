using System.Numerics;
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
        
        Assert.True(image.Get_Pixel(0, 0).Is_Close(Color.Black), "Test 1");
        Assert.True(image.Get_Pixel(1, 0).Is_Close(Color.Black), "Test 2");
        Assert.True(image.Get_Pixel(2, 0).Is_Close(Color.Black), "Test 3");
        
        Assert.True(image.Get_Pixel(0, 1).Is_Close(Color.Black),"Test 4");
        Assert.True(image.Get_Pixel(1, 1).Is_Close(sphereColor), "Test 5");
        Assert.True(image.Get_Pixel(2, 1).Is_Close(Color.Black),"Test 6");
        
        Assert.True(image.Get_Pixel(0, 2).Is_Close(Color.Black),"Test 7");
        Assert.True(image.Get_Pixel(1, 2).Is_Close(Color.Black), "Test 8");
        Assert.True(image.Get_Pixel(2, 2).Is_Close(Color.Black), "Test 9");
    }

    [Fact]
    public void Furnace_Test()
    {
        var pcg = new Pcg();
        //Run the furnace_test several times using random values for the emitted radiance and reflectance

        for (int i = 0; i < 7; i++)
        {
            var world = new World();
            var emittedRadiance = pcg.Random_Float();
            var reflectance = pcg.Random_Float() * 0.9f; //Be sure to pick a reflectance not too close to 1

            var sphere = new Sphere(null,new Material(new DiffuseBrdf(new UniformPigment(Color.White * reflectance)),
                new UniformPigment(Color.White * emittedRadiance)));
            world.Add(sphere);
            var pathTracer = new PathTracing(world, null, pcg,1, 100, 101);
            var ray = new Ray(new Point(), new Vec(1.0f, 0.0f, 0.0f));
            var color = pathTracer.Tracing(ray);

            var expected = emittedRadiance / (1.0f - reflectance);
            
            Assert.True(Functions.Are_Close(expected, color.R));
            Assert.True(Functions.Are_Close(expected, color.G));
            Assert.True(Functions.Are_Close(expected, color.B));

        }
    }
    
}