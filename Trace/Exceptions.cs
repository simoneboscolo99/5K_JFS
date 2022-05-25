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