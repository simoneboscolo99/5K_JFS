namespace Trace;

public class SceneFile
{
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
    public class SourceLocation
    {
        public string file_name = "";
        public int line_num = 0;
        public int column_num = 0;
    }
}