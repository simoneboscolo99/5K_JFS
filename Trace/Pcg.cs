namespace Trace;

// uint: 32-bit unsigned integer 
// long: 64-bit signed integer 
// ulong: 64-bit unsigned integer 

/// <summary>
/// PCG Uniform Pseudo-random Number Generator
/// </summary>
public class Pcg
{
    public ulong State;
    public ulong Inc;

    /// <summary>
    /// Pcg constructor
    /// </summary>
    /// <param name="state"></param>
    /// <param name="seq"></param>
    public Pcg(ulong state = 42, ulong seq = 54)
    {
        State = 0;
        // Left-shift operator <<: shifts its left-hand operand left by the number of bits defined by its right-hand operand
        // Logical OR operator |: computes the bitwise logical OR
        Inc = (seq << 1) | 1;
        Random();
        State += state;
        Random();
    }

    /// <summary>
    /// Return a new random number and advance PCG's internal state
    /// </summary>
    /// <returns> Random 32-bit unsigned integer number </returns>
    public uint Random()
    {
        var oldState = State;
        State = oldState * 6364136223846793005 + Inc;
        
        // Right-shift operator >>: shifts its left-hand operand right by the number of bits defined by its right-hand operand
        // the high-order empty bit positions are always set to zero (when the left-hand operand is of type uint or ulong)
        // Logical exclusive OR (XOR) operator ^: computes the bitwise logical XOR
        var xorShifted = (uint) (((oldState >> 18) ^ oldState) >> 27);
        var rot = (uint) (oldState >> 59);
        
        // uint operator <<(uint x, int count)
        // Right-hand operand in <<  or >> must be casted to int
        return (xorShifted >> (int) rot) | (xorShifted << (int) (-rot & 31));
    }

    /// <summary>
    /// Return a new random number uniformly distributed over [0, 1]
    /// </summary>
    /// <returns> Floating-point number between 0 and 1 </returns>
    public float Random_Float()
        => Random() / (float) uint.MaxValue;
    
}