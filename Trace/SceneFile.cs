using System.Globalization;
using System.Runtime.CompilerServices;
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
        float val = 0;
        while (true)
        {
            var ch = ReadChar();
            bool all = Char.IsDigit(Convert.ToChar(ch)) | ch == "." | ch == "e" | ch == "E";
            if (all != true)
            {
                UnreadChar(ch);
                break;
            }

            tkn += ch;

            try
            {
                var value = float.Parse(tkn);
                val = value;
            }

            catch (ArgumentException)
            {
                throw new GrammarErrorException("'{token}' is an invalid floating-point number", tokenLocation);
            }
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
            if ((Char.IsLetterOrDigit(Convert.ToChar(ch)) | ch == "_") != true)
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
                if (Char.IsDigit(Convert.ToChar(ch)) | ch is "+" or "-" or ".")
                {
                    //A floating-point number
                    return ParseFloatToken(ch, Location);
                }

                if (Char.IsLetter(Convert.ToChar(ch)) | ch == "_")
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
    Cylinder,
    Disk,
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
        {"cylinder", KeywordEnum.Cylinder},
        {"disk", KeywordEnum.Disk},
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



    public ICamera? Camera = null;

    public IDictionary<string, Material> Materials;
    
    public IDictionary<string, float> FloatVariables;

    public Scene(World wd, ICamera? camera, IDictionary<string, Material> materials, IDictionary<string, float> floatVariables)
    {
        Wd = wd;
        Camera = camera;
        Materials = materials;
        FloatVariables = floatVariables;
    }
    
    /// Read a token from <paramref name="inputFile"/> and check that it matches <paramref name="symbol"/>`.
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="symbol"></param>
    public static void ExpectSymbol(InputStream inputFile, string symbol)
    {
        var token = inputFile.ReadToken();
        if (token is not LiteralNumberToken || ((SymbolToken) token).Symbol != symbol)
            throw new GrammarErrorException($"Got {token} instead of {symbol}",token.Location);
    }

    public static KeywordEnum ExpectKeywords(InputStream inputFile, List<KeywordEnum> keyword)
    {
        var token = inputFile.ReadToken();
        if (token is not KeywordToken)
            throw new GrammarErrorException($"Expected a keyword instead of {token}",token.Location);
        if (!keyword.Contains(((KeywordToken) token).Keyword))
            throw new GrammarErrorException("Expected one of the keywords:\n" + string.Join("\n", keyword) + $"\ninstead of {token}", token.Location);

        return ((KeywordToken) token).Keyword;

    }

    /// <summary>
    /// Read a token from `input_file`
    /// and check that it is either a literal number or a variable in `scene`
    /// Return the number as a ``float``."""
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="scene"></param>
    /// <returns></returns>
    public float ExpectedNumber(InputStream inputFile, Scene scene)
    {
        var token = inputFile.ReadToken();
        if (token is LiteralNumberToken a) return a.Value;
        else if (token is IdentifierToken b)
        {
            var variableName = b.Identifier;
            if (!scene.FloatVariables.ContainsKey(variableName))
                throw new GrammarErrorException($"unknown variable '{token}'", token.Location);
            return scene.FloatVariables[variableName];
        }

        throw new GrammarErrorException($"got '{token}' instead of a number", token.Location);
    }

    public string ExpectIdentifier(InputStream inputFile, Scene scene)
    {
        //Read a token from `input_file` and check that it is an identifier.
        //Return the name of the identifier.
        var token = inputFile.ReadToken();
        if (token is not IdentifierToken a)
            throw new GrammarErrorException($"got '{token}' instead of an identifier", token.Location);
        return a.Identifier;
    }

    /// <summary>
    /// Read a token from `input_file` and check that it is a literal string.
    ///Return the value of the string (a ``str``)."""
    /// </summary>
    /// <param name="inputFile"></param>
    /// <returns></returns>
    public string ExpectedString(InputStream inputFile)
    {
        var token = inputFile.ReadToken();
        if (token is not StringToken a) 
            throw new GrammarErrorException($"got '{token}' instead of a string", token.Location);
        return a.Str;
    }
}

