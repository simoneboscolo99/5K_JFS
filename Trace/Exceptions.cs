namespace Trace;

/// <summary>
/// The exception that is thrown when the format of the pfm file is not valid.
/// </summary>
[Serializable]
public class InvalidPfmFileFormatException : Exception
{
    /// <summary>
    /// Initialize a new instance of the <see cref="InvalidPfmFileFormatException"/> class.
    /// </summary>
    public InvalidPfmFileFormatException() { }

    /// <summary>
    /// Initialize a new instance of the <see cref="InvalidPfmFileFormatException"/> class with a specified error message.
    /// </summary>
    /// <param name="message"> The error message that explains the reason for the exception. </param>
    public InvalidPfmFileFormatException(string message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when
/// </summary>
[Serializable]
public class RuntimeException : Exception
{
    /// <summary>
    /// Initialize a new instance of the <see cref="RuntimeException"/> class.
    /// </summary>
    public RuntimeException() { }

    /// <summary>
    /// Initialize a new instance of the <see cref="RuntimeException"/> class with a specified error message.
    /// </summary>
    /// <param name="message"> The message that describes the error. </param>
    public RuntimeException(string message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when there is a syntax or lexical error in the scene file. <br/>
/// The error is found by the lexer/parser while reading the file.
/// </summary>
[Serializable]
public class GrammarErrorException : Exception
{
    /// <summary>
    /// Specific position of the error. See <see cref="SourceLocation"/> for more information.
    /// </summary>
    public SourceLocation Location;

    /// <summary>
    /// Initialize a new instance of the <see cref="GrammarErrorException"/> class.
    /// </summary>
    /// <param name="location"> Specific position of the error: name of the file, line, column. </param>
    public GrammarErrorException(SourceLocation location)
    {
        Location = location;
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="GrammarErrorException"/> class.
    /// </summary>
    /// <param name="message"> The message that describes the error. </param>
    /// <param name="location"> Specific position of the error: name of the file, line, column. </param>
    public GrammarErrorException(string? message, SourceLocation location) : base(message)
    {
        Location = location;
    }
}