namespace Trace;

/// <summary>
/// A «pigment»
/// This abstract class represents a pigment, i.e., a function that associates a color with
/// each point on a parametric surface (u,v). Call the method :meth:`.Pigment.get_color` to
/// retrieve the color of the surface given a :class:`.Vec2d` object.
/// </summary>
public abstract class Pigment
{
  /// <summary>
  /// Return the color of the pigment at the specified coordinates
  /// </summary>
  /// <param name="uv"></param>
  /// <returns></returns>
  public abstract Color Get_Color(Vec2D uv);
}

/// <summary>
/// A uniform pigment
/// This is the most boring pigment: a uniform hue over the whole surface.
/// </summary>
public class UniformPigment : Pigment
{
  public Color C;
  
  public UniformPigment(Color? c = null)
  {
    C = c ?? Color.White;
  }
  
  public override Color Get_Color(Vec2D uv)
  {
    //mono-chromatic coloration
    return C;
  }
}

//Let's play chess
/// <summary>
/// A checkered pigment
/// The number of rows/columns in the checkered pattern is tunable, but you cannot have a different number of
/// repetitions along the u/v directions.
/// </summary>
public class CheckeredPigment : Pigment
{
  public Color C1;
  public Color C2;
  public int NumOfSteps;

  public CheckeredPigment(Color color1, Color color2, int nOfSteps = 10)
  {
    C1 = color1;
    C2 = color2;
    NumOfSteps = nOfSteps;
  }

  public override Color Get_Color(Vec2D uv)
  {
    var u = (int) Math.Floor(uv.U * NumOfSteps);
    var v = (int) Math.Floor(uv.V * NumOfSteps);

    return (u % 2 == v % 2) switch
    {
      true => C1,
      false => C2
    };
  }

  /// <summary>
  /// A textured pigment
  /// The texture is given through a PFM image.
  /// </summary>
  public class ImagePigment : Pigment
  {
    //texture given by an image
    public HdrImage Img;

    public ImagePigment(HdrImage img)
    {
      Img = img;
    }

    public override Color Get_Color(Vec2D v)
    {
      //"casting" (u,v) coordinates on image points
      var col = (int) (v.U * Img.Width);
      var row = (int) (v.V * Img.Height);

      if (col >= Img.Width)
        col = Img.Width - 1;

      if (row >= Img.Height)
        row = Img.Height - 1;

      /* A nicer solution would implement bilinear interpolation to reduce pixelization artifacts
      See https://en.wikipedia.org/wiki/Bilinear_interpolation */ //finita brdf lo faccio
      return Img.Get_Pixel(col, row);
    }
    
  }
  
}

/// <summary>
/// An abstract class representing a Bidirectional Reflectance Distribution Function
/// </summary>
public abstract class Brdf
{
  public Pigment Pg;

  protected Brdf(Pigment? pigment = null)
  {
    Pg = pigment ?? new UniformPigment(Color.White);
    //if (pigment == new UniformPigment(Color.White))
    //Pg = new UniformPigment(Color.White);
    //else
    //Pg = pigment;
  }

  public virtual Color Eval(Normal n, Vec vIn, Vec vOut, Vec2D uv) => Color.Black;
  public abstract Ray Scatter_Ray(Pcg pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth);
}

/// <summary>
/// A class representing an ideal diffuse BRDF (also called «Lambertian»)
/// </summary>
public class DiffuseBrdf : Brdf
{
  public DiffuseBrdf(Pigment? p = null) : base(p) { }

  public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
    => Pg.Get_Color(uv) * (float) (1.0f / Math.PI);
  
  public override Ray Scatter_Ray(Pcg pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
  {
    // Cosine-weighted distribution around the z (local) axis
    var (e1, e2, e3) = Normal.Create_ONB_From_Z(normal);
    var phi = (float) 2.0 * Math.PI * pcg.Random_Float();
    var cosThetaSq = pcg.Random_Float();
    var cosTheta = (float) Math.Sqrt(cosThetaSq);
    var sinTheta = (float) Math.Sqrt(1.0f - cosThetaSq);

    return new Ray(
      interactionPoint,
      e1 * (float) Math.Cos(phi) * cosTheta + e2 * (float) Math.Sin(phi) * cosTheta + e3 * sinTheta,
      (float?) 1.0e-3,
      float.PositiveInfinity,
      depth);
  }
}

/// <summary>
/// A class representing an ideal mirror BRDF
/// </summary>
public class SpecularBrdf : Brdf
{
  public float ThresholdAngleRad;

  public SpecularBrdf(Pigment? p = null, float? thresholdAngleRad = null) : base(p)
  {
    ThresholdAngleRad = (float) (thresholdAngleRad ??  Math.PI / 1800.0f);
  }

  public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
  {
    // We provide this implementation for reference, but we are not going to use it (neither in the path tracer nor in the point-light tracer)
    var thetaIn = (float) Math.Acos(normal.Normalize().Dot(inDir.Normalize()));
    var thetaOut = (float) Math.Acos(normal.Normalize().Dot(outDir.Normalize()));
    // if (Math.Abs(thetaIn - thetaOut) < ThresholdAngleRad) return Pg.Get_Color(uv) \n else return Color.Black
    return Math.Abs(thetaIn - thetaOut) < ThresholdAngleRad ? Pg.Get_Color(uv) : Color.Black;
  }
  
  public override Ray Scatter_Ray(Pcg pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
  {
    // There is no need to use the PCG here, as the reflected direction is always completely deterministic for a perfect mirror
    var rayDir = new Vec(incomingDir.X, incomingDir.Y, incomingDir.Z).Normalize();
    var norm = normal.To_Vec().Normalize();
    var dotProd = norm.Dot(rayDir);

    return new Ray(
      interactionPoint,
      rayDir - norm * 2 * dotProd,
      (float) 1e-5,
      float.PositiveInfinity,
      depth
    );
  }
}

/// <summary>
/// A material
/// </summary>
public class Material
{
  public Brdf Brdf;
  public Pigment EmittedRadiance;

  public Material(Brdf? brdf = null, Pigment? emittedRadiance = null)
  {
    Brdf = brdf ?? new DiffuseBrdf();
    EmittedRadiance = emittedRadiance ?? new UniformPigment(Color.Black);
  }
}
