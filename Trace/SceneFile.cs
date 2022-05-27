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