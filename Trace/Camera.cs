namespace Trace;

public interface ICamera
{
    float AspectRatio { get; set; }
    Transformation T { get; set; }
    Ray Fire_Ray(float u, float v);
}

public class OrthogonalCamera : ICamera
{
    public float AspectRatio { get; set; }
    public Transformation T { get; set; }

    public OrthogonalCamera(float aspectRatio= 1.0f, Transformation? t = null)
    {
        AspectRatio = aspectRatio;
        T = t ?? Transformation.Identity();
    }
    
    public Ray Fire_Ray(float u, float v)
    {
        var origin = new Point(-1.0f, (1.0f - 2.0f * u) * AspectRatio, 2 * v - 1);
        var direction = new Vec(1.0f, 0.0f, 0.0f);
        return T * new Ray(origin, direction, 1.0f);
    }
}

public class PerspectiveCamera : ICamera
{
    public float Distance { get; set; }
    public float AspectRatio { get; set; }
    public Transformation T { get; set; }

    public PerspectiveCamera(float distance = 1.0f, float aspectRatio = 1.0f, Transformation? t = null)
    {
        Distance = distance;
        AspectRatio = aspectRatio;
        T = t ?? Transformation.Identity();
    }

    public Ray Fire_Ray(float u, float v)
    {
        var origin = new Point(-Distance, 0.0f, 0.0f);
        var direction = new Vec(Distance, (1.0f - 2.0f * u) * AspectRatio, 2 * v - 1);
        return T * new Ray(origin, direction, 1.0f);
    }
}