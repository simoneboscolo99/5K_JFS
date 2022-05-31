using System.Diagnostics;
using System.Globalization;

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

    public InputStream(Stream stream, string filename = "", int tabulations = 8)
    {
        Stream = stream;
        //Note that we start counting lines/columns from 1, not from 0
        Location = new SourceLocation(filename, 1, 1);
        SavedChar = "";
        SavedLocation = Location;
        Tabulations = tabulations;
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
        Debug.Assert(SavedChar == "");
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
                while (ReadChar() is not "\r" or "\n" or "")
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
            if (ch == "''") break;

            if (ch == "");

            token += ch;
        }

        return StringToken(token_location, token);
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

    public IDictionary<string, KeywordEnum> Dict = new Dictionary<string, KeywordEnum>
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


/*
def _parse_float_token(self, first_char: str, token_location: SourceLocation) -> LiteralNumberToken:
token = first_char
while True:
ch = self.read_char()

if not (ch.isdigit() or ch == "." or ch in ["e", "E"]):
self.unread_char(ch)
break

token += ch

try:
value = float(token)
except ValueError:
raise GrammarError(token_location, f"'{token}' is an invalid floating-point number")

return LiteralNumberToken(token_location, value)

def _parse_keyword_or_identifier_token(
    self,
    first_char: str,
token_location: SourceLocation,
    ) -> Union[KeywordToken, IdentifierToken]:
token = first_char
while True:
ch = self.read_char()
# Note that here we do not call "isalpha" but "isalnum": digits are ok after the first character
if not (ch.isalnum() or ch == "_"):
self.unread_char(ch)
break

token += ch

try:
# If it is a keyword, it must be listed in the KEYWORDS dictionary
return KeywordToken(token_location, KEYWORDS[token])
except KeyError:
# If we got KeyError, it is not a keyword and thus it must be an identifier
return IdentifierToken(token_location, token)