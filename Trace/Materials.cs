namespace Trace;

public abstract class Pigment
{
  public abstract Color Get_Color(Vec2D v);
}

public class UniformPigment : Pigment
{
  private Color _c;
  public UniformPigment(Color c)
  {
    _c = c;
  }
  
  public override Color Get_Color(Vec2D v)
  {
    //mono-chromatic coloration
    return _c;
  }
}

//Let's play chess
public class CheckeredPigment : Pigment
{
//The number of rows/columns in the checkered pattern is tunable, but you cannot have a different number of
//repetitions along the u/v directions."""
  public Color C1;
  public Color C2;
  public int NumOfSteps;

  public CheckeredPigment(Color color1, Color color2, int nOfSteps = 10)
  {
    C1 = color1;
    C2 = color2;
    NumOfSteps = nOfSteps;
  }

  public override Color Get_Color(Vec2D v)
  {
    var u = (int) (Math.Floor(v.U * NumOfSteps));
    var vi = (int) (Math.Floor(v.V * NumOfSteps));

    switch ((u % 2) == (vi % 2))
    {
      case true:
        return C1;

      case false:
        return C2;
    }

  }

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
      {
        col = Img.Width - 1;
      }

      if (row >= Img.Height)
      {
        row = Img.Height - 1;
      }

/* A nicer solution would implement bilinear interpolation to reduce pixelization artifacts
 See https://en.wikipedia.org/wiki/Bilinear_interpolation */ //finita brdf lo faccio
      return Img.Get_Pixel(col, row);
    }
    
  }
  
}

//Class representing bidirectional reflectance distribution function
public abstract class Brdf
{
  public Pigment? Pg;

  protected Brdf(Pigment? pigment)
  {
    if (pigment == new UniformPigment(Color.White))
      Pg = new UniformPigment(Color.White);
    else
      Pg = pigment;
  }

  public virtual Color Eval(Normal n, Vec vIn, Vec vOut, Vec2D uv) => Color.Black;

  public abstract Ray Scatter_Ray(Pcg pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth);
}

public class DiffuseBrdf : Brdf
{
  public DiffuseBrdf(Pigment? p = null) : base(p)
  {
    Pg = p ?? new UniformPigment(Color.White) ;
  }

  public override Color Eval(Normal n, Vec vIn, Vec vOut, Vec2D uv)
  {
    if (Pg != null) return Pg.Get_Color(uv) * (float) (1.0f / Math.PI);
    return default;
  }

  public override Ray Scatter_Ray(Pcg pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
  {
    throw new NotImplementedException();
  }
}

public class Material
{
  public  DiffuseBrdf? BRdf;
  public Pigment EmittedRadiance;
  
  public Material(DiffuseBrdf? brdf = null, Pigment? emittedRadiance = null)
  {
    BRdf = brdf ?? new DiffuseBrdf(new UniformPigment(Color.White));
    EmittedRadiance = emittedRadiance ?? new UniformPigment(Color.Black);
  }
}

