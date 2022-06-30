using System.Globalization;
using Xunit;

//using static Microsoft.VisualBasic.CompilerServices.CharType;
namespace Trace;

/// <summary>
/// A specific position in a source file. <br/>
/// This class has the following fields: <see cref="FileName"/>, <see cref="LineNum"/>, <see cref="ColumnNum"/>.
/// </summary>
public struct SourceLocation
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="lineNum"></param>
    /// <param name="columnNum"></param>
    public SourceLocation(string filename, int lineNum, int columnNum)
    {
        FileName = filename;
        LineNum = lineNum;
        ColumnNum = columnNum;
    }
}

/// <summary>
/// A high-level wrapper around a stream, used to parse scene files. <br/>
/// This class implements a wrapper around a stream, with the following additional capabilities: <br/>
/// - It tracks the line number and column number; <br/>
/// - It permits to "un-read" characters and tokens.
/// </summary>
public class InputStream
{
    /// <summary>
    /// 
    /// </summary>
    public Stream Stream;

    public string SavedChar;
    public SourceLocation SavedLocation;
    public SourceLocation Location;
    public int Tabulations;
    public Token? SavedToken;

    public InputStream(Stream stream, string filename = "", int tabulations = 8)
    {
        Stream = stream;
        //Note that we start counting lines/columns from 1, not from 0
        Location = new SourceLocation(filename, 1, 1);
        SavedChar = "";
        SavedLocation = Location;
        Tabulations = tabulations;
        SavedToken = null;
    }

    ///Update `location` after having read `ch` from the stream
    public void UpdatePos(string ch)
    {
        if (ch == "")
        {
            //Nothing to do!
            return;
        }

        if (ch == "\n")
        {
            Location.LineNum += 1;
            Location.ColumnNum = 1;
        }
        else if (ch == "\t")
        {
            Location.ColumnNum += Tabulations;
        }

        else Location.ColumnNum += 1;
    }

    /// <summary>
    /// Read a new character from the stream
    /// </summary>
    /// <returns></returns>
    public string ReadChar()
    {
        string ch = "";
        if (SavedChar != "")
        {
            //Recover the «unread» character and return it
            ch += SavedChar;
            SavedChar = "";
        }
        else
        {
            //Read a new character from the stream
            var curByte = Stream.ReadByte();
            if (curByte != -1) ch += (char) curByte;
        }

        SavedLocation = Location;
        UpdatePos(ch);
        return ch;
    }

    /// <summary>
    /// Push a character back to the stream
    /// </summary>
    /// <returns></returns>
    public void UnreadChar(string ch)
    {
        Assert.True(SavedChar == "");
        //Debug.Assert(SavedChar == "");
        SavedChar = ch;
        Location = SavedLocation;
    }

    /// <summary>
    /// Keep reading characters until a non-whitespace/non-comment character is found.
    /// </summary>
    public void SkipWhitespacesAndComments()
    {
        var ch = ReadChar();
        while (ch is "\t" or "\r" or "\n" or "#" or " ")
        {
            if (ch == "#")
            {
                // It's a comment! Keep reading until the end of the line (include the case "", the end-of-file)
                while (ReadChar() is not ("\r" or "\n" or ""))
                {
                }
            }

            ch = ReadChar();
            if (ch == "") return;
        }

        // Put the non-whitespace character back
        UnreadChar(ch);
    }

    public StringToken ParseStringToken(SourceLocation location)
    {
        var token = "";
        while (true)
        {
            var ch = ReadChar();

            if (ch == "\"") break;

            if (ch == "") throw new GrammarErrorException("unterminated string", location);

            token += ch;
        }

        return new StringToken(location, token);
    }

    public LiteralNumberToken ParseFloatToken(string firstChar, SourceLocation tokenLocation)
    {
        var tkn = firstChar;
        float val;
        while (true)
        {
            var ch = ReadChar();
            bool all = Char.IsDigit(Convert.ToChar(ch, CultureInfo.InvariantCulture)) | ch == "." | ch == "e" | ch == "E";
            if (all != true)
            {
                UnreadChar(ch);
                break;
            }

            tkn += ch;
        }

        try
        {
            var value = float.Parse(tkn, CultureInfo.InvariantCulture);
            val = value;
        }

        catch (ArgumentException)
        {
            throw new GrammarErrorException("'{token}' is an invalid floating-point number", tokenLocation);
        }

        return new LiteralNumberToken(tokenLocation, val);
    }

    public Token ParseKeywordOrIdentifierToken(string firstChar, SourceLocation location)
    {
        var tkn = firstChar;
        while (true)
        {
            var ch = ReadChar();
            // Note that here we do not call "isalpha" but "isalnum": digits are ok after the first character
            if ((Char.IsLetterOrDigit(Convert.ToChar(ch, CultureInfo.InvariantCulture)) | ch == "_") != true)
            {
                UnreadChar(ch);
                break;
            }

            tkn += ch;
        }

        try
        {
            // If it is a keyword, it must be listed in the KEYWORDS dictionary
            return new KeywordToken(location, KeywordToken.Dict[tkn]);
        }

        catch (KeyNotFoundException)
        {
            // If we got KeyError, it is not a keyword and thus it must be an identifier
            return new IdentifierToken(location, tkn);
        }
    }



    /// <summary>
    /// Read a token from the stream.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="GrammarErrorException"> a lexical error is found.</exception>
    public Token ReadToken()
    {

        if (SavedToken != null)
        {
            var result = SavedToken;
            SavedToken = null;
            return result;
        }

        SkipWhitespacesAndComments();
        // At this point we're sure that ch does *not* contain a whitespace character
        var ch = ReadChar();

        switch (ch)
        {
            case "":
                // No more characters in the file, so return a StopToken
                return new StopToken(Location);


            // At this point we must check what kind of token begins with the "ch" character (which has been
            // put back in the stream with self.unread_char)
            case "(" or ")" or "<" or ">" or "[" or "]" or "," or "*":
                //One-character symbol, like '(' or ','
                return new SymbolToken(Location, ch);
            case "\"":
                //A literal string (used for file names)
                return ParseStringToken(Location);
            default:
            {
                if (Char.IsDigit(Convert.ToChar(ch, CultureInfo.InvariantCulture)) | ch is "+" or "-" or ".")
                {
                    //A floating-point number
                    return ParseFloatToken(ch, Location);
                }

                if (Char.IsLetter(Convert.ToChar(ch, CultureInfo.InvariantCulture)) | ch == "_")
                {
                    //Since it begins with an alphabetic character, it must either be a keyword 
                    // or a identifier
                    return ParseKeywordOrIdentifierToken(ch, Location);
                }

                //We got some weird character, like '@` or `&`
                throw new GrammarErrorException("Invalid character {ch}", Location);
            }
        }
    }


    /// <summary>
    /// Make as if `token` were never read from `input_file`
    /// </summary>
    public void UnreadToken(Token token)
    {
        Assert.True(SavedToken == null);
        SavedToken = token;
    }
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

    public static IDictionary<string, KeywordEnum> Dict = new Dictionary<string, KeywordEnum>
    {
        {"new", KeywordEnum.New},
        {"material", KeywordEnum.Material},
        {"plane", KeywordEnum.Plane},
        {"sphere", KeywordEnum.Sphere},
        {"diffuse", KeywordEnum.Diffuse},
        {"specular", KeywordEnum.Specular},
        {"uniform", KeywordEnum.Uniform},
        {"checkered", KeywordEnum.Checkered},
        {"image", KeywordEnum.Image},
        {"identity", KeywordEnum.Identity},
        {"translation", KeywordEnum.Translation},
        {"rotation_x", KeywordEnum.RotationX},
        {"rotation_y", KeywordEnum.RotationY},
        {"rotation_z", KeywordEnum.RotationZ},
        {"scaling", KeywordEnum.Scaling},
        {"camera", KeywordEnum.Camera},
        {"orthogonal", KeywordEnum.Orthogonal},
        {"perspective", KeywordEnum.Perspective},
        {"float", KeywordEnum.Float}
    };
        
    
    public KeywordToken(SourceLocation location, KeywordEnum keyword) : base(location)
    {
        Keyword = keyword;
    }
    
    public override string ToString() => Keyword.ToString();
}

/// <summary>
/// A token containing an identifier.
/// </summary>
public class IdentifierToken : Token
{ 
    public string Identifier;

    public IdentifierToken(SourceLocation location, string identifier) : base(location)
    {
        Identifier = identifier;
    }
    
    public override string ToString() => Identifier;
}

/// <summary>
/// A token containing a literal string.
/// </summary>
public class StringToken : Token
{
    public string Str;

    public StringToken(SourceLocation location, string s) : base(location)
    {
        Str = s;
    }

    public override string ToString() => Str;
}

/// <summary>
/// A token containing a literal number.
/// </summary>
public class LiteralNumberToken : Token
{
    public float Value;

    public LiteralNumberToken(SourceLocation location, float value) : base(location) 
    { 
        Value = value;
    }

    public override string ToString() => Convert.ToString(Value, CultureInfo.InvariantCulture);
}

/// <summary>
/// A token containing a symbol (i.e., a variable name).
/// </summary>
public class SymbolToken : Token
{
    public string Symbol;

    public SymbolToken(SourceLocation location, string symbol) : base(location)
    {
        Symbol = symbol;
    }

    public override string ToString() => Symbol;
}



/// <summary>
/// A scene read from a scene file.
/// </summary>
public class Scene
{
    public World Wd;

    public ICamera? Camera;

    /// <summary>
    /// List of materials
    /// </summary>
    public IDictionary<string, Material> Materials;
    
    /// <summary>
    /// List of floating variables.
    /// </summary>
    public IDictionary<string, float> FloatVariables;

    /// <summary>
    /// 
    /// </summary>
    public HashSet<string> OverriddenVariables;

    public Scene(World? wd = null, ICamera? camera = null, IDictionary<string, Material>? materials = null, IDictionary<string, float>? floatVariables = null, HashSet<string>? overriddenVariables = null)
    {
        Wd = wd ?? new World();
        Camera = camera;
        Materials = materials ?? new Dictionary<string, Material>();
        FloatVariables = floatVariables ?? new Dictionary<string, float>();
        OverriddenVariables = overriddenVariables ?? new HashSet<string>();
    }
    
    /// <summary>
    /// Read a token from <paramref name="inputFile"/> and check that it matches <paramref name="symbol"/>.
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="symbol"> the symbol. </param>
    /// <exception cref="GrammarErrorException"> a lexical error is found. </exception>
    public static void ExpectSymbol(InputStream inputFile, string symbol)
    {
        var token = inputFile.ReadToken();
        if (token is not SymbolToken || ((SymbolToken) token).Symbol != symbol)
            throw new GrammarErrorException($"Got {token} instead of {symbol}",token.Location);
    }

    /// <summary>
    /// Read a token from <paramref name="inputFile"/> and check that it is one of the keywords in <paramref name="keywords"/>.
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="keywords"></param>
    /// <returns> the keyword.  </returns>
    /// <exception cref="GrammarErrorException"> a lexical error is found. </exception>
    public static KeywordEnum ExpectKeywords(InputStream inputFile, List<KeywordEnum> keywords)
    {
        var token = inputFile.ReadToken();
        if (token is not KeywordToken)
            throw new GrammarErrorException($"Expected a keyword instead of {token}",token.Location);
        if (!keywords.Contains(((KeywordToken) token).Keyword))
            throw new GrammarErrorException("Expected one of the keywords:\n" + string.Join("\n", keywords) + $"\ninstead of {token}", token.Location);

        return ((KeywordToken) token).Keyword;

    }

    /// <summary>
    /// Read a token from <paramref name="inputFile"/> and check that it is either a literal number or a variable in <paramref name="scene"/>.
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="scene"></param>
    /// <returns> the number as a float. </returns>
    /// <exception cref="GrammarErrorException"> a lexical error is found. </exception>
    public static float ExpectNumber(InputStream inputFile, Scene scene)
    {
        var token = inputFile.ReadToken();
        switch (token)
        {
            case LiteralNumberToken numberToken:
                return numberToken.Value;
            case IdentifierToken identifierToken:
            {
                var variableName = identifierToken.Identifier;
                if (!scene.FloatVariables.ContainsKey(variableName))
                    throw new GrammarErrorException($"Unknown variable '{token}'", identifierToken.Location);
                return scene.FloatVariables[variableName];
            }
            default:
                throw new GrammarErrorException($"Got '{token}' instead of a number", token.Location);
        }
    }

    /// <summary>
    /// Read a token from <paramref name="inputFile"/> and check that it is an identifier.
    /// </summary>
    /// <param name="inputFile"></param>
    /// <returns> the name of the identifier. </returns>
    /// <exception cref="GrammarErrorException"> a lexical error is found. </exception>
    public static string ExpectIdentifier(InputStream inputFile)
    {
 
        var token = inputFile.ReadToken();
        if (token is not IdentifierToken)
            throw new GrammarErrorException($"got '{token}' instead of an identifier", token.Location);
        return ((IdentifierToken) token).Identifier;
    }


    /// <summary>
    /// Read a token from <paramref name="inputFile"/> and check that it is a literal string.
    /// </summary>
    /// <param name="inputFile"></param>
    /// <returns> the value of the string (a string). </returns>
    /// <exception cref="GrammarErrorException"> a lexical error is found. </exception>
    public static string ExpectString(InputStream inputFile)
    {
        var token = inputFile.ReadToken();
        if (token is not StringToken) 
            throw new GrammarErrorException($"got '{token}' instead of a string", token.Location);
        return ((StringToken) token).Str;
    }

    

    public static Vec ParseVector(InputStream inputFile, Scene scene)
    {
        ExpectSymbol(inputFile, "[");
        var x = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var y = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var z = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, "]");

        return new Vec(x, y, z);
    }
    
    public static Color ParseColor(InputStream inputFile, Scene scene)
    {
        ExpectSymbol(inputFile, "<");
        var r = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var g = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var b = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ">");

        return new Color(r, g, b);
    }

    public static Pigment ParsePigment(InputStream inputFile, Scene scene)
    {
        var keyword = ExpectKeywords(inputFile, new List<KeywordEnum> {KeywordEnum.Uniform, KeywordEnum.Checkered, KeywordEnum.Image});

        Pigment result = new UniformPigment();
        ExpectSymbol(inputFile, "(");
        switch (keyword)
        {
            case KeywordEnum.Uniform:
            {
                var color = ParseColor(inputFile, scene);
                result = new UniformPigment(color);
                break;
            }
            case KeywordEnum.Checkered:
            {
                var color1 = ParseColor(inputFile, scene);
                ExpectSymbol(inputFile, ",");
                var color2 = ParseColor(inputFile, scene);
                ExpectSymbol(inputFile, ",");
                var numOfSteps = (int) ExpectNumber(inputFile, scene);
                result = new CheckeredPigment(color1, color2, numOfSteps);
                break;
            }
            case KeywordEnum.Image:
            {
                var fileName = ExpectString(inputFile);
                var image = new HdrImage(fileName);
                result = new ImagePigment(image);
                break;
            }
            default:
                Assert.False(true, "This line should be unreachable.");
                break;
        }
        
        ExpectSymbol(inputFile, ")");
        return result;
    }

    public static Brdf ParseBrdf(InputStream inputFile, Scene scene)
    {
        var keyword = ExpectKeywords(inputFile, new List<KeywordEnum> {KeywordEnum.Diffuse, KeywordEnum.Specular});
        ExpectSymbol(inputFile, "(");
        var pigment = ParsePigment(inputFile, scene);
        ExpectSymbol(inputFile, ")");

        Brdf result = new DiffuseBrdf();
        switch (keyword)
        {
            case KeywordEnum.Diffuse:
            {
                result = new DiffuseBrdf(pigment);
                break;
            }
            case KeywordEnum.Specular:
            {
                result = new SpecularBrdf(pigment);
                break;
            }
            default:
                Assert.False(true, "This line should be unreachable.");
                break;
        }

        return result;
    }

    public static (string, Material) ParseMaterial(InputStream inputFile, Scene scene)
    {
        var name = ExpectIdentifier(inputFile);
        ExpectSymbol(inputFile, "(");
        var brdf = ParseBrdf(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var emittedRadiance = ParsePigment(inputFile, scene);
        ExpectSymbol(inputFile, ")");
        
        return (name, new Material(brdf, emittedRadiance));
    }

    public static Transformation ParseTransformation(InputStream inputFile, Scene scene)
    {
        var result = Transformation.Identity();

        while (true)
        {
            var keyword = ExpectKeywords(inputFile, new List<KeywordEnum> {KeywordEnum.Identity, KeywordEnum.Translation, KeywordEnum.RotationX, KeywordEnum.RotationY, KeywordEnum.RotationZ, KeywordEnum.Scaling});

            switch (keyword)
            {
                case KeywordEnum.Identity:
                    break;
                case KeywordEnum.Translation:
                    ExpectSymbol(inputFile, "(");
                    result *= Transformation.Translation(ParseVector(inputFile, scene));
                    ExpectSymbol(inputFile, ")");
                    break;
                case KeywordEnum.RotationX:
                    ExpectSymbol(inputFile, "(");
                    result *= Transformation.Rotation_X(ExpectNumber(inputFile, scene));
                    ExpectSymbol(inputFile, ")");
                    break;
                case KeywordEnum.RotationY:
                    ExpectSymbol(inputFile, "(");
                    result *= Transformation.Rotation_Y(ExpectNumber(inputFile, scene));
                    ExpectSymbol(inputFile, ")");
                    break;
                case KeywordEnum.RotationZ:
                    ExpectSymbol(inputFile, "(");
                    result *= Transformation.Rotation_Z(ExpectNumber(inputFile, scene));
                    ExpectSymbol(inputFile, ")");
                    break;
                case KeywordEnum.Scaling:
                    ExpectSymbol(inputFile, "(");
                    result *= Transformation.Scale(ParseVector(inputFile, scene));
                    ExpectSymbol(inputFile, ")");
                    break;
            }
            
            // We must peek the next token to check if there is another transformation that is being
            // chained or if the sequence ends. Thus, this is a LL(1) parser.
            var nextKw = inputFile.ReadToken();
            if (nextKw is SymbolToken {Symbol: "*"}) continue;
            
            // Pretend you never read this token and put it back!
            inputFile.UnreadToken(nextKw);
            break;
        }

        return result;
    }

    public static Sphere ParseSphere(InputStream inputFile, Scene scene)
    {
        ExpectSymbol(inputFile, "(");
        var materialName = ExpectIdentifier(inputFile);
        // We raise the exception here because input_file is pointing to the end of the wrong identifier
        if (!scene.Materials.ContainsKey(materialName)) throw new GrammarErrorException($"unknown material {materialName}", inputFile.Location);
        
        ExpectSymbol(inputFile, ",");
        var transformation = ParseTransformation(inputFile, scene);
        ExpectSymbol(inputFile, ")");

        return new Sphere(transformation, scene.Materials[materialName]);
    }
    
    public static Plane ParsePlane(InputStream inputFile, Scene scene)
    {
        ExpectSymbol(inputFile, "(");
        var materialName = ExpectIdentifier(inputFile);
        // We raise the exception here because input_file is pointing to the end of the wrong identifier
        if (!scene.Materials.ContainsKey(materialName)) throw new GrammarErrorException($"unknown material {materialName}", inputFile.Location);
        
        ExpectSymbol(inputFile, ",");
        var transformation = ParseTransformation(inputFile, scene);
        ExpectSymbol(inputFile, ")");

        return new Plane(transformation, scene.Materials[materialName]);
    }

    public static ICamera ParseCamera(InputStream inputFile, Scene scene)
    {
        ICamera result = new PerspectiveCamera();
        ExpectSymbol(inputFile, "(");
        var keyword = ExpectKeywords(inputFile, new List<KeywordEnum> {KeywordEnum.Perspective, KeywordEnum.Orthogonal});
        ExpectSymbol(inputFile, ",");
        var transformation = ParseTransformation(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var aspectRatio = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var distance = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ")");

        switch (keyword)
        {
            case KeywordEnum.Perspective:
                result = new PerspectiveCamera(distance, aspectRatio, transformation);
                break;
            case KeywordEnum.Orthogonal:
                result = new OrthogonalCamera(aspectRatio, transformation);
                break;
            default:
                Assert.False(true, "This line should be unreachable.");
                break;
        }

        return result;
    }

    /// <summary>
    /// Read a scene description from a stream and return a <see cref="Scene"/> object.
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    public static Scene ParseScene(InputStream inputFile, IDictionary<string, float>? variables = null)
    {
        var scene = new Scene();
        scene.FloatVariables = variables ?? new Dictionary<string, float>();
        if (variables != null) scene.OverriddenVariables = new HashSet<string>(variables.Keys);

        while (true)
        {
            var what = inputFile.ReadToken();
            if (what is StopToken) break;
            if (what is not KeywordToken token) throw new GrammarErrorException($"expected a keyword instead of {what}", inputFile.Location);

            if (token.Keyword == KeywordEnum.Float)
            {
                var variableName = ExpectIdentifier(inputFile);
                
                // Save this for the error message
                var variableLocation = inputFile.Location;
                ExpectSymbol(inputFile, "(");
                var variableValue = ExpectNumber(inputFile, scene);
                ExpectSymbol(inputFile, ")");

                if (scene.FloatVariables.ContainsKey(variableName) && !scene.OverriddenVariables.Contains(variableName)) throw new GrammarErrorException($"variable «{variableName}» cannot be redefined", variableLocation);
                if (!scene.OverriddenVariables.Contains(variableName))
                    // Only define the variable if it was not defined by the user outside the scene file (e.g., from the command line)
                    scene.FloatVariables[variableName] = variableValue;
            }     
            else if (token.Keyword == KeywordEnum.Sphere) scene.Wd.Add(ParseSphere(inputFile, scene));
            else if (token.Keyword == KeywordEnum.Plane) scene.Wd.Add(ParsePlane(inputFile, scene));
            else if (token.Keyword == KeywordEnum.Camera)
            {
                if (scene.Camera != null) throw new GrammarErrorException("You cannot define more than one camera", what.Location);
                scene.Camera = ParseCamera(inputFile, scene);
            }
            else if (token.Keyword == KeywordEnum.Material)
            {
                var (name, material) = ParseMaterial(inputFile, scene);
                scene.Materials[name] = material;
            }
        }
        
        return scene;
    }
}