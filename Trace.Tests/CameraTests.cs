using Xunit;

namespace Trace.Tests;

public class CameraTests
{
    [Fact]
    public void TestOrthogonalCamera()
    {
        var cam = new OrthogonalCamera(aspectRatio: 2.0f);
        var ray1 = cam.Fire_Ray(0.0f, 0.0f);
        var ray2 = cam.Fire_Ray(1.0f, 0.0f);
        var ray3 = cam.Fire_Ray(0.0f, 1.0f);
        var ray4 = cam.Fire_Ray(1.0f, 1.0f);

        // Verify that the rays are parallel by verifying that cross-products vanish
        Assert.True(Functions.Are_Close(ray1.Dir.Cross(ray2.Dir).Squared_Norm(), 0.0f), "Test ray1-ray2");
        Assert.True(Functions.Are_Close(ray1.Dir.Cross(ray3.Dir).Squared_Norm(), 0.0f), "Test ray1-ray3");
        Assert.True(Functions.Are_Close(ray1.Dir.Cross(ray4.Dir).Squared_Norm(), 0.0f), "Test ray1-ray4");

        // Verify that the ray hitting the corners have the right coordinates
        Assert.True(ray1.At(1.0f).Is_Close(new Point(0.0f, 2.0f, -1.0f)), "Test point ray1");
        Assert.True(ray2.At(1.0f).Is_Close(new Point(0.0f, -2.0f, -1.0f)), "Test point ray2");
        Assert.True(ray3.At(1.0f).Is_Close(new Point(0.0f, 2.0f, 1.0f)), "Test point ray3");
        Assert.True(ray4.At(1.0f).Is_Close(new Point(0.0f, -2.0f, 1.0f)), "Test point ray4");
    }

    [Fact]
    public void TestOrthogonalCameraTransform()
    {
        var cam = new OrthogonalCamera(t: Transformation.Translation(2.0f * new Vec(0.0f, -1.0f, 0.0f)) *
                                          Transformation.Rotation_Z(90));
        var ray = cam.Fire_Ray(0.5f, 0.5f);
        Assert.True(ray.At(1.0f).Is_Close(new Point(0.0f, -2.0f, 0.0f)), "Test camera transformation");
    }

    [Fact]
    public void TestPerspectiveCamera()
    {
        var cam = new PerspectiveCamera(aspectRatio: 2.0f);
        var ray1 = cam.Fire_Ray(0.0f, 0.0f);
        var ray2 = cam.Fire_Ray(1.0f, 0.0f);
        var ray3 = cam.Fire_Ray(0.0f, 1.0f);
        var ray4 = cam.Fire_Ray(1.0f, 1.0f);

        // Verify that all the rays depart from the same point
        Assert.True(ray1.Origin.Is_Close(ray2.Origin), "Test origin ray1-ray2");
        Assert.True(ray1.Origin.Is_Close(ray3.Origin), "Test origin ray1-ray3");
        Assert.True(ray1.Origin.Is_Close(ray4.Origin), "Test origin ray1-ray4");

        // Verify that the ray hitting the corners have the right coordinates
        Assert.True(ray1.At(1.0f).Is_Close(new Point(0.0f, 2.0f, -1.0f)), "Test point ray1");
        Assert.True(ray2.At(1.0f).Is_Close(new Point(0.0f, -2.0f, -1.0f)), "Test point ray2");
        Assert.True(ray3.At(1.0f).Is_Close(new Point(0.0f, 2.0f, 1.0f)), "Test point ray3");
        Assert.True(ray4.At(1.0f).Is_Close(new Point(0.0f, -2.0f, 1.0f)), "Test point ray4");
    }
}

public class ImageTracerTests
{
    static HdrImage image = new(4, 2);
    static PerspectiveCamera camera = new(aspectRatio: 2.0f);
    ImageTracer tracer = new(image, camera);

    [Fact]
    public void TestOrientation()
    {
        // Fire a ray against top-left corner of the screen
        var topLeftRay = tracer.Fire_Ray(0, 0, 0.0f, 0.0f);
        Assert.True(topLeftRay.At(1.0f).Is_Close(new Point(0.0f, 2.0f, 1.0f)), "Test ray top-left corner");

        // Fire a ray against bottom-right corner of the screen
        var bottomRightRay = tracer.Fire_Ray(3, 1, 1.0f, 1.0f);
        Assert.True(bottomRightRay.At(1.0f).Is_Close(new Point(0.0f, -2.0f, -1.0f)), "Test ray bottom-right corner");
    }

    [Fact]
    public void Test_uv_SubMapping()
    {
        var ray1 = tracer.Fire_Ray(0, 0, 2.5f, 1.5f);
        var ray2 = tracer.Fire_Ray(2, 1);
        Assert.True(ray1.Is_Close(ray2), "Test Fire Ray ray1-ray2");
    }
    
    [Fact]
    public void TestImageCoverage() 
    {
        var solve = new SameColor();
        tracer.Fire_All_Rays(solve);
        for (int row = 0; row < image.Height; row++)
        {
            for (int col = 0; col < image.Width; col++)
                Assert.True(image.Get_Pixel(col, row).Is_Close(new Color(1.0f, 2.0f, 3.0f)), "Test solver");
        }
    }
}