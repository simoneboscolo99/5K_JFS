namespace Trace;

// ===========================================================================
// === VEC === VEC === VEC === VEC === VEC === VEC === VEC === VEC === VEC ===
// ===========================================================================

/// <summary>
/// A 3D vector, represented by three floating-point values: <see cref="X"/>, <see cref="Y"/>, and <see cref="Z"/>.
/// </summary>
public struct Vec
{
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public float X { get; }
    
    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public float Y { get; }
    
    /// <summary>
    /// The Z component of the vector.
    /// </summary>
    public float Z { get; }
    
    /// <summary>
    /// Vec constructor. Creates a new <see cref="Vec"/> object whose elements have the specified values.
    /// </summary>
    /// <param name="x"> The value to assign to the <see cref="X"/> field. </param>
    /// <param name="y"> The value to assign to the <see cref="Y"/> field. </param>
    /// <param name="z"> The value to assign to the <see cref="Z"/> field. </param>
    public Vec(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Sums two vectors. <br/>
    /// </summary>
    /// <param name="v"> The first vector to add. <see cref="Vec"/> </param>
    /// <param name="w"> The second vector to add. <see cref="Vec"/> </param>
    /// <returns> The summed vector. </returns>
    public static Vec operator +(Vec v, Vec w)
        => new(v.X + w.X, v.Y + w.Y, v.Z + w.Z);

    /// <summary>
    /// Subtracts the second vector from the first. <br/>
    /// </summary>: Overloading operator '-'
    /// <param name="v"> The first vector. </param>
    /// <param name="w"> The second vector. </param>
    /// <returns> The difference vector. </returns>
    public static Vec operator -(Vec v, Vec w)
        => new(v.X - w.X, v.Y - w.Y, v.Z - w.Z);

    /// <summary>
    /// Multiplies the specified vector by the specified scalar value.
    /// </summary>
    /// <param name="a" > The scalar value. </param> 
    /// <param name="v"> The vector. </param> Color
    /// <returns> The scaled vector. </returns>
    public static Vec operator *(float a, Vec v)
        => new(a * v.X, a * v.Y, a * v.Z);

    /// <summary>
    /// Multiplies the specified vector by the specified scalar value.
    /// </summary> 
    /// <param name="v"> The vector. </param>
    /// <param name="a"> The scalar value. </param>
    /// <returns> The scaled vector. </returns>
    public static Vec operator *(Vec v, float a)
        => new(a * v.X, a * v.Y, a * v.Z);

    /// <summary>
    /// Negates the specified vector.
    /// </summary>
    /// <param name="v"> The vector to negate. </param>
    /// <returns> The negated vector. </returns>
    public static Vec operator -(Vec v)
        => new(-v.X, -v.Y, -v.Z);

    /// <summary>
    /// Returns the dot product between the current instance and the specified vector.
    /// </summary>
    /// <param name="v"> The vector. </param>
    /// <returns> The dot product. </returns>
    public float Dot(Vec v)
        => X * v.X + Y * v.Y + Z * v.Z;
    
    //dot product <vec,normal>
    /// <summary>
    /// Returns the dot product between the current instance and the specified normal.
    /// </summary>
    /// <param name="m"> The normal. </param>
    /// <returns> The dot product. </returns>
    public float Dot(Normal m)
        => X * m.X + Y * m.Y + Z * m.Z;

    /// <summary>
    /// Computes the cross product between the current instance and the specified vector.
    /// </summary>
    /// <param name="v"> The vector. </param>
    /// <returns> The cross product. </returns>
    public Vec Cross(Vec v)
        => new(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);

    /// <summary>
    /// Computes the cross product of two vectors.
    /// </summary>
    /// <param name="v"> The first vector. </param>
    /// <param name="w"> The second vector. </param>
    /// <returns> The cross product. </returns>
    public static Vec Cross(Vec v, Vec w)
        => new(v.Y * w.Z - v.Z * w.Y, v.Z * w.X - v.X * w.Z, v.X * w.Y - v.Y * w.X);

    /// <summary>
    /// Returns the squared norm (Euclidean length) of the current instance. <br/>
    /// This is faster than <see cref="Vec.Norm"/> if you just need the squared norm.
    /// </summary>
    /// <returns> The vector's squared norm. </returns>
    public float Squared_Norm()
        => X * X + Y * Y + Z * Z;

    /// <summary>
    /// Returns the norm (Euclidean length) of the current instance.
    /// </summary>
    /// <returns> The vector's norm. </returns>
    public float Norm()
        => (float)Math.Sqrt(Squared_Norm());

    /// <summary>
    /// Returns a vector with the same direction as the current instance, but with a length of one.
    /// </summary>
    /// <returns> The normalized vector. </returns>
    public Vec Normalize()
    {
        var mod = Norm();
        return new Vec(X / mod, Y / mod, Z / mod);
    }

    /// <summary>
    /// Converts the current <see cref="Vec"/> instance into a <see cref="Normal"/> object with the same elements.
    /// </summary>
    /// <returns> The normal. </returns>
    public Normal ToNormal()
        => new(X, Y, Z);

    /// <summary>
    /// Converts the current <see cref="Vec"/> instance into a <see cref="Point"/> object with the same elements.
    /// </summary>
    /// <returns> The point. </returns>
    public Point ToPoint() => new(X, Y, Z);

    /// <summary>
    /// Returns a value indicating whether this instance and a specified <see cref="Vec"/> object represent roughly the same vector.
    /// </summary>
    /// <param name="v"> The vector to compare to this instance. </param>
    /// <param name="eps"> The precision. </param>
    /// <returns> true if the <see cref="X"/>, <see cref="Y"/> and <see cref="Z"/> elements of the specified vector and of the current instance differ by less than <paramref name="eps"/>; otherwise, false. </returns>
    public bool Is_Close(Vec v, float eps = 1e-5f)
        => Functions.Are_Close(X, v.X, eps) && Functions.Are_Close(Y, v.Y, eps) && Functions.Are_Close(Z, v.Z, eps);

    /// <summary>
    /// Returns the string representation of the current <see cref="Vec"/> instance.
    /// </summary>
    public override string ToString() => $"({X}, {Y}, {Z})";
}


// ===========================================================================
// ==== POINT === POINT === POINT === POINT === POINT === POINT === POINT ====
// ===========================================================================

/// <summary>
/// A point in 3D space. <br/>
/// This struct has three floating-point fields: <see cref="X"/>, <see cref="Y"/>, and <see cref="Z"/>.
/// </summary>
public struct Point
{
    /// <summary>
    /// The X component of the point.
    /// </summary>
    public float X { get; }
    
    /// <summary>
    /// The Y component of the point.
    /// </summary>
    public float Y { get; }
    
    /// <summary>
    /// The Z component of the point.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Point Constructor. Creates a new <see cref="Point"/> object whose elements have the specified values.
    /// </summary>
    /// <param name="x"> The value to assign to the <see cref="X"/> field. </param>
    /// <param name="y"> The value to assign to the <see cref="Y"/> field. </param>
    /// <param name="z"> The value to assign to the <see cref="Z"/> field. </param>
    public Point(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Returns the string representation of the current <see cref="Point"/> instance.
    /// </summary>
    public override string ToString() => $"({X}, {Y}, {Z})";

    /// <summary>
    /// Returns a value indicating whether this instance and a specified <see cref="Point"/> object represent roughly the same point.
    /// </summary>
    /// <param name="b"> The point to compare to this instance. </param>
    /// <param name="eps"> The precision. </param>
    /// <returns> true if the <see cref="X"/>, <see cref="Y"/> and <see cref="Z"/> elements of the specified point and of the current instance are close by less than <paramref name="eps"/>; otherwise, false. </returns>
    public bool Is_Close(Point b,  float eps = 1e-5f)
        => Functions.Are_Close(X, b.X, eps) && Functions.Are_Close(Y, b.Y, eps) && Functions.Are_Close(Z, b.Z, eps);

    /// <summary>
    /// Moves the specified point by the specified vector.
    /// </summary>
    /// <param name="a"> The point. </param>
    /// <param name="b"> The vector. </param>
    /// <returns> The translated point. </returns>
    public static Point operator +(Point a, Vec b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    /// <summary>
    /// Sum two points.
    /// </summary>
    /// <param name="a"> The first point. </param>
    /// <param name="b"> The second point. </param>
    /// <returns> The summed point. </returns>
    public static Point operator +(Point a, Point b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    /// <summary>
    /// Subtracts the second point from the first.
    /// </summary>
    /// <param name="a" > The first point. </param> 
    /// <param name="b" > The second point. </param>
    /// <returns> The difference vector. </returns>
    public static Vec operator -(Point a, Point b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    /// <summary>
    /// Subtracts the vector from the point.
    /// </summary>
    /// <param name="a" > The point. </param> 
    /// <param name="b" > The vector. </param>
    /// <returns> The new point. </returns>
    public static Point operator -(Point a, Vec b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    /// <summary>
    /// Multiplies the specified point by the specified scalar value.
    /// </summary>
    /// <param name="b"> The point. </param>
    /// <param name="a"> The scalar value. </param>
    /// <returns> The scaled point. </returns>
    public static Point operator *(Point b, float a)
        => new(a * b.X, a * b.Y, a * b.Z);

    /// <summary>
    /// Multiplies the specified point by the specified scalar value.
    /// </summary>
    /// <param name="a" > The scalar value. </param> 
    /// <param name="b"> The point. </param> 
    /// <returns> The scaled point. </returns>
    public static Point operator *(float a, Point b)
        => new(a * b.X, a * b.Y, a * b.Z);

    /// <summary>
    /// Converts the current <see cref="Point"/> instance into a <see cref="Vec"/> object with the same elements.
    /// </summary>
    /// <returns> The vector. </returns>
    public Vec To_Vec()
        => new (X, Y, Z);
}


// ============================================================================
// ==== NORMAL ==== NORMAL ==== NORMAL ==== NORMAL ==== NORMAL ==== NORMAL ====
// ============================================================================

/// <summary>
/// A normal vector in 3D space. <br/>
/// This struct has three floating-point fields: <see cref="X"/>, <see cref="Y"/>, and <see cref="Z"/>.
/// </summary>
public struct Normal
{
    /// <summary>
    /// The X component of the normal.
    /// </summary>
    public float X { get; }
    
    /// <summary>
    /// The Y component of the normal.
    /// </summary>
    public float Y { get; }
    
    /// <summary>
    /// The Z component of the normal.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Normal constructor. Creates a new <see cref="Normal"/> object whose elements have the specified values.
    /// </summary>
    /// <param name="x"> The value to assign to the <see cref="X"/> field. </param>
    /// <param name="y"> The value to assign to the <see cref="Y"/> field. </param>
    /// <param name="z"> The value to assign to the <see cref="Z"/> field. </param>
    public Normal(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Returns the string representation of the current <see cref="Normal"/> instance.
    /// </summary>
    public override string ToString() => $"({X}, {Y}, {Z})";

    /// <summary>
    /// Returns a value indicating whether this instance and a specified <see cref="Normal"/> object represent roughly the same normal.
    /// </summary>
    /// <param name="m"> The normal to compare to this instance. </param>
    /// <param name="eps"> The precision. </param>
    /// <returns> true if the <see cref="X"/>, <see cref="Y"/> and <see cref="Z"/> elements of the specified normal and of the current instance are close by less than <paramref name="eps"/>; otherwise, false. </returns>
    public bool Is_Close(Normal m, float eps = 1e-5f)
        => Functions.Are_Close(X, m.X, eps) && Functions.Are_Close(Y, m.Y, eps) && Functions.Are_Close(Z, m.Z, eps);

    //negation (-n)
    /// <summary>
    /// Negates the specified normal.
    /// </summary>
    /// <param name="n"> The normal. </param>
    /// <returns> The negated normal. </returns>
    public static Normal operator -(Normal n)
        => new(-n.X, -n.Y, -n.Z);

    //product with a scalar 
    /// <summary>
    /// Multiples the specified normal by the specified scalar value.
    /// </summary>
    /// <param name="n"> The normal. </param>
    /// <param name="a"> The scalar value. </param>
    /// <returns> The scaled normal. </returns>
    public static Normal operator *(Normal n, float a)
        => new(a * n.X, a * n.Y, a * n.Z);

    /// <summary>
    /// Multiples the specified normal by the specified scalar value.
    /// </summary>
    /// <param name="a"> The scalar value. </param>
    /// <param name="n"> The normal. </param>
    /// <returns> The scaled normal. </returns>
    public static Normal operator *(float a, Normal n)
        => new(a * n.X, a * n.Y, a * n.Z);

    //dot product normal*vec
    /// <summary>
    /// Returns the dot product between the current instance and the specified vector.
    /// </summary>
    /// <param name="m"> The vector. </param>
    /// <returns> The dot product. </returns>
    public float Dot(Vec m)
        => X * m.X + Y * m.Y + Z * m.Z;

    /// <summary>
    /// Returns the dot product between the current instance and the specified normal.
    /// </summary>
    /// <param name="m"> The normal. </param>
    /// <returns> The dot product. </returns>
    public float Dot(Normal m)
        => X * m.X + Y * m.Y + Z * m.Z;

    //vector product
    /// <summary>
    /// Computes the cross product between the current instance and the specified vector.
    /// </summary>
    /// <param name="v"> The vector. </param>
    /// <returns> The cross product. </returns>
    public Normal Cross(Vec v)
        => new(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);

    /// <summary>
    /// Computes the cross product between the current instance and the specified normal.
    /// </summary>
    /// <param name="m"> The normal. </param>
    /// <returns> The cross product. </returns>
    public Normal Cross(Normal m)
        => new(Y * m.Z - Z * m.Y, Z * m.X - X * m.Z, X * m.Y - Y * m.X);

    /// <summary>
    /// Returns the squared norm (Euclidean length) of the current instance. <br/>
    /// This is faster than <see cref="Normal.Norm"/> if you just need the squared norm.
    /// </summary>
    /// <returns> The normal's squared norm. </returns>
    public float SquaredNorm()
        => (float) (Math.Pow(X, 2f) + Math.Pow(Y, 2f) + Math.Pow(Z, 2f));

    /// <summary>
    /// Returns the norm (Euclidean length) of the current instance.
    /// </summary>
    /// <returns> The normal's norm. </returns>
    public float Norm()
        => (float) Math.Sqrt(SquaredNorm());

    /// <summary>
    /// Returns a normal with the same direction as the current instance, but with a length of one.
    /// </summary>
    /// <returns> The normalized normal. </returns>
    public Normal Normalize()
        => new(X / Norm(), Y / Norm(), Z / Norm());
    
    /// <summary>
    /// Converts the current <see cref="Normal"/> instance into a <see cref="Vec"/> object with the same elements.
    /// </summary>
    /// <returns> The vector. </returns>
    public Vec To_Vec()
        => new(X, Y, Z);
    
    /// <summary>
    /// Creates a orthonormal basis (ONB) from a normal representing the z axis. The normal must be previously normalized when this method is invoked.
    /// </summary>
    /// <param name="normal"> The normal. </param>
    /// <returns> tuple containing the three vectors (e1, e2, e3) of the basis. The result is such that e3 = <paramref name="normal"/>. </returns>
    public static (Vec, Vec, Vec) Create_ONB_From_Z(Normal normal)
    {
        var sign = (float) Math.CopySign(1.0f, normal.Z);
        var a = -1.0f / (sign + normal.Z);
        var b = normal.X * normal.Y * a;
        var e1 = new Vec(1.0f + sign * normal.X * normal.X * a, sign * b, -sign * normal.X);
        var e2 = new Vec(b, sign + normal.Y * normal.Y * a, -normal.Y);
        var onb = (E1: e1, E2: e2, E3: new Vec(normal.X, normal.Y, normal.Z));
        return onb;
    }
}


// ===========================================================================
// ==== VEC2D === VEC2D === VEC2D === VEC2D === VEC2D === VEC2D === VEC2D ====
// ===========================================================================

/// <summary>
/// A 2D vector used to represent a point on a surface. <br/>
/// The fields are named <see cref="U"/> and <see cref="V"/> to distinguish them from the usual 3D coordinates <see cref="Vec.X"/>, <see cref="Vec.Y"/>, <see cref="Vec.Z"/>.
/// </summary>
public struct Vec2D
{
    /// <summary>
    /// The first component od the 2D vec.
    /// </summary>
    public float U = 0.0f;
    
    /// <summary>
    /// The second component od the 2D vec.
    /// </summary>
    public float V = 0.0f;

    public Vec2D(float u, float v)
    {
        U = u;
        V = v;
    }

    /// <summary>
    /// Returns a value indicating whether this instance and the specified <see cref="Vec2D"/> object represent roughly the same 2D vector.
    /// </summary>
    /// <param name="v"> The 2D vector. </param>
    /// <param name="eps"> The precision. </param>
    /// <returns> true if the <see cref="U"/> and <see cref="V"/> elements of the specified 2D vector and of the current instance differ by less than <paramref name="eps"/>; otherwise, false. </returns>
    public bool Is_Close(Vec2D v, float eps = 1e-5f)
        => Functions.Are_Close(U, v.U, eps) && Functions.Are_Close(V, v.V, eps);

    /// <summary>
    /// Returns the string representation of the current <see cref="Vec2D"/> instance.
    /// </summary>
    public override string ToString() => $"({U}, {V})";

}