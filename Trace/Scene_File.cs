using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Trace;

/// <summary>
/// A specific position in a source file
/// This class has the following fields:
///- file_name: the name of the file,
/// or the empty string if there is no file associated with this location
/// (e.g., because the source code was provided as a memory stream,
/// or through a network connection)
/// - line_num: number of the line (starting from 1)
///- col_num: number of the column (starting from 1)
/// </summary>
public struct SourceLocation
{
    public string FileName = "";
    public int LineNum = 0;
    public int ColumnNum = 0;

    public SourceLocation(string filename, int linenum, int columnnum)
    {
        FileName = filename;
        LineNum = linenum;
        ColumnNum = columnnum;
    }
}

/// <summary>
/// A high-level wrapper around a stream, used to parse scene files
///This class implements a wrapper around a stream, with the following additional capabilities:
///- It tracks the line number and column number;
///- It permits to "un-read" characters and tokens.
/// </summary>
public class InputStream
{
    public Stream Stream;
    public string SavedChar = "";
    public SourceLocation SavedLocation;
    public SourceLocation Location;
    public int Tabulations = 8;

    public InputStream(Stream stream, string filename, int tabulations)
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
            Location.ColumnNum += 1;
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
}

