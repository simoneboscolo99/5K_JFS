namespace Trace;

[Serializable]
public class InvalidPfmFileFormat : Exception   
{
    public InvalidPfmFileFormat() { }
    
    public InvalidPfmFileFormat(string message)
        : base(message) { }
}

[Serializable]
public class RuntimeException : Exception   
{
    public RuntimeException() { }
    
    public RuntimeException(string message)
        : base(message) { }
}