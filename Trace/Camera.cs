namespace Trace;

/// <summary>
/// Interface representing a generic observer. The view position can be moved through a transformation. Backward ray-tracing oriented (rays propagate from observer's eye to the light source) 
/// </summary>
public interface ICamera
{
    Transformation T { get; set; }
    /// <summary>member <c>Fire_Ray</c> </summary> Shoots a ray through the camera's screen, coordinates (u,v) represent the point of the screen were the ray passes through
    Ray Fire_Ray(float u, float v);
}

/// <summary>
/// Implementation of ICamera, axonometric view, all the rays come from infinity with the same direction
/// </summary>
public class OrthogonalCamera : ICamera
{
    /// <summary>
    /// This parameter defines how larger than the height is the image. For fullscreen images, you should probably set `AspectRatio` to 16/9.
    /// </summary>
    public float AspectRatio { get; set; }

    public Transformation T { get; set; }

    public OrthogonalCamera(float aspectRatio = 1.0f, Transformation? t = null)
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

/// <summary>Class <c>ICamera</c> Implementation of ICamera, perspective view, rays diverge from viewer's eye 
/// </summary>
public class PerspectiveCamera : ICamera
{
    /// <summary>member <c>Distance:</c> distance eye-screen (it determines the aperture of the scene)</summary>
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