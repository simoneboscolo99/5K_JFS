namespace Trace;

/// <summary>
/// A specific position in a source file. <br/>
/// This class has the following fields: <see cref="FileName"/>, <see cref="LineNum"/>, <see cref="ColumnNum"/>.
/// </summary>
public class SourceLocation
{
    /// <summary>
    /// The name of the file, or the empty string if there is no file associated with this location
    /// (e.g., because the source code was provided as a memory stream, or through a network connection).
    /// </summary>
    public string FileName = "";
    
    /// <summary>
    /// The number of the line (starting from 1).
    /// </summary>
    public int LineNum = 0;
    
    /// <summary>
    /// The number of the column (starting from 1).
    /// </summary>
    public int ColumnNum = 0;
}

/// <summary>
/// A lexical token, used when parsing a scene file.
/// </summary>
public abstract class Token
{
    public SourceLocation Location;

    protected Token(SourceLocation location)
    {
        Location = location;
    }
}

/// <summary>
/// A token signalling the end of a file.
/// </summary>
public class StopToken : Token
{
    public StopToken(SourceLocation location) : base(location) { }
}

/// <summary>
/// Enumeration for all the possible keywords recognized by the lexer.
/// </summary>
// Enumeration type: more efficient
public enum KeywordEnum 
{
    New = 1, 
    Material, 
    Plane, 
    Sphere, 
    Diffuse, 
    Specular, 
    Uniform, 
    Checkered, 
    Image,
    Identity, 
    Translation, 
    RotationX, 
    RotationY, 
    RotationZ, 
    Scaling, 
    Camera, 
    Orthogonal,
    Perspective, 
    Float
}

/// <summary>
/// A token containing a keyword.
/// </summary>
public class KeywordToken : Token
{
    public KeywordEnum Keyword;
    
    public KeywordToken(SourceLocation location, KeywordEnum keyword) : base(location)
    {
        Keyword = keyword;
    }
}