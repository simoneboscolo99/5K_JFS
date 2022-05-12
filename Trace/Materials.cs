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
  public UniformPigment(Color c)
  {
    C = c;
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
  }

  public virtual Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
  {
    return Color.Black;
  }

  //public abstract Ray Scatter_Ray(Pcg pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth);
}

/// <summary>
/// A class representing an ideal diffuse BRDF (also called «Lambertian»)
/// </summary>
public class DiffuseBrdf : Brdf
{

  public DiffuseBrdf(Pigment? p = null) : base(p) { }

  public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
    => Pg.Get_Color(uv) * (float) (1.0f / Math.PI);
}

/// <summary>
/// A material
/// </summary>
public class Material
{
  public Brdf Brdf;
  public Pigment Pg;

  public Material(Brdf brdf, Pigment pg)
  {
    Brdf = brdf;
    Pg = pg;
  }
}


