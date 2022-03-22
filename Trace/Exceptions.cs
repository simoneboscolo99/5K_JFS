namespace Trace;

[Serializable]
public class InvalidPfmFileFormat : Exception
{
    public InvalidPfmFileFormat() { }

    public InvalidPfmFileFormat(string message)
        : base(message) { }
}