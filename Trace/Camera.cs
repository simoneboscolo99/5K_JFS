namespace Trace;

/// <summary>
/// An interface representing a generic observer. The view position can be moved through a transformation. <br/>
/// Concrete subclasses are <see cref="OrthogonalCamera"/> and <see cref="PerspectiveCamera"/>. 
/// </summary>
public interface ICamera
{
    /// <summary>
    /// The transformation of the camera. It changes the view position.
    /// </summary>
    Transformation T { get; set; }
    
    /// <summary>
    /// Fire a ray through the camera's screen. Fire a ray that goes through the screen at the position (u, v). The exact meaning
    /// of these coordinates depend on the projection used by the camera. <br/>
    /// This is an abstract method. You should redefine it in derived classes.
    /// </summary>
    Ray Fire_Ray(float u, float v);
}

/// <summary>
/// A camera implementing an orthogonal 3D → 2D projection. <br/>
/// This class implements an axonometric view: all the rays come from infinity with the same direction.
/// </summary>
public class OrthogonalCamera : ICamera
{
    /// <summary>
    /// This parameter defines how larger than heighter is the image.
    /// For fullscreen images, you should probably set <see cref="AspectRatio"/> to 16/9.
    /// </summary>
    public float AspectRatio { get; }

    /// <summary>
    /// The transformation of the camera. It changes the view position.
    /// </summary>
    public Transformation T { get; set; }

    /// <summary>
    /// OrthogonalCamera Constructor. Initialize a new instance of the <see cref="OrthogonalCamera"/> camera that has the specified colors.
    /// </summary>
    /// <param name="aspectRatio"> Defines how larger than the height is the image. </param>
    /// <param name="t"> Instance of the <see cref="Transformation"/> class: it changes the view direction. </param>
    public OrthogonalCamera(float aspectRatio = 1.0f, Transformation? t = null)
    {
        AspectRatio = aspectRatio;
        T = t ?? Transformation.Identity();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public Ray Fire_Ray(float u, float v)
    {
        var origin = new Point(-1.0f, (1.0f - 2.0f * u) * AspectRatio, 2 * v - 1);
        var direction = new Vec(1.0f, 0.0f, 0.0f);
        return T * new Ray(origin, direction, 1.0f);
    }
}

/// <summary>
/// A camera implementing a perspective 3D → 2D projection. <br/>
/// This class implements an observer seeing the world through a perspective projection.
/// In a perspective view, rays diverge from viewer's eye.
/// </summary>
public class PerspectiveCamera : ICamera
{
    /// <summary>
    /// member <c>Distance:</c> distance eye-screen (it determines the aperture of the scene)
    /// </summary>
    public float Distance { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public float AspectRatio { get; }
    
    /// <summary>
    /// The transformation of the camera. It changes the view position.
    /// </summary>
    public Transformation T { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="aspectRatio"></param>
    /// <param name="t"></param>
    public PerspectiveCamera(float distance = 1.0f, float? aspectRatio = null, Transformation? t = null)
    {
        Distance = distance;
        if (aspectRatio != null) AspectRatio = (float) aspectRatio;
        else AspectRatio = 1.0f;
        T = t ?? Transformation.Identity();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public Ray Fire_Ray(float u, float v)
    {
        var origin = new Point(-Distance, 0.0f, 0.0f);
        var direction = new Vec(Distance, (1.0f - 2.0f * u) * AspectRatio, 2 * v - 1);
        return T * new Ray(origin, direction, 1.0f);
    }
}