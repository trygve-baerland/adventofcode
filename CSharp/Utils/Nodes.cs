using MathNet.Numerics.LinearAlgebra;
using System.Globalization;
using System.Numerics;

namespace AoC.Utils;

public record struct Node2D<T>( T X, T Y )
where T : INumber<T>
{
    public static Node2D<T> operator +( Node2D<T> lhs, Tangent2D<T> rhs ) =>
        new( lhs.X + rhs.X, lhs.Y + rhs.Y );

    public static Tangent2D<T> operator -( Node2D<T> lhs, Node2D<T> rhs ) =>
        new( lhs.X - rhs.X, lhs.Y - rhs.Y );

    public T Cross( Node2D<T> other ) => ToTangent().Cross( other.ToTangent() );

    public IEnumerable<Node2D<T>> Neighbours()
    {
        foreach ( var dir in Tangent2D<T>.Directions() )
        {
            yield return this + dir;
        }
    }
    public Tangent2D<T> ToTangent() => new( X, Y );

    public override string ToString() => $"({X}, {Y})";

    public Node2D<T> Up() => this + Tangent2D<T>.Up;
    public Node2D<T> Down() => this + Tangent2D<T>.Down;
    public Node2D<T> Left() => this + Tangent2D<T>.Left;
    public Node2D<T> Right() => this + Tangent2D<T>.Right;
}

public record struct Tangent2D<T>( T X, T Y )
where T : INumber<T>
{
    public static IEnumerable<Tangent2D<T>> Directions()
    {
        yield return new Tangent2D<T>( T.Zero, T.One );
        yield return new Tangent2D<T>( -T.One, T.Zero );
        yield return new Tangent2D<T>( T.Zero, -T.One );
        yield return new Tangent2D<T>( T.One, T.Zero );
    }

    public override string ToString() => $"[{X}, {Y}]";

    public static Tangent2D<T> operator +( Tangent2D<T> lhs, Tangent2D<T> rhs ) =>
        new( lhs.X + rhs.X, lhs.Y + rhs.Y );
    public static Tangent2D<T> operator -( Tangent2D<T> lhs, Tangent2D<T> rhs ) =>
        new( lhs.X - rhs.X, lhs.Y - rhs.Y );
    public static Tangent2D<T> operator *( Tangent2D<T> lhs, T rhs ) =>
        new( lhs.X * rhs, lhs.Y * rhs );
    public static Tangent2D<T> operator *( T lhs, Tangent2D<T> rhs ) =>
        rhs * lhs;

    public static readonly Tangent2D<T> Up = new( -T.One, T.Zero );
    public static readonly Tangent2D<T> Down = new( T.One, T.Zero );
    public static readonly Tangent2D<T> Left = new( T.Zero, -T.One );
    public static readonly Tangent2D<T> Right = new( T.Zero, T.One );

    // Make convertible to tuple
    public static implicit operator (T x, T y)( Tangent2D<T> t ) => (t.X, t.Y);
    public static implicit operator Tangent2D<T>( (T x, T y) t ) => new( t.x, t.y );

    // Some vector operations
    public T Dot( Tangent2D<T> other ) => X * other.X + Y * other.Y;
    public T Cross( Tangent2D<T> other ) => X * other.Y - Y * other.X;
    public Tangent2D<double> Normalize()
    {
        var l = System.Math.Sqrt( double.CreateChecked( X * X + Y * Y ) );
        return new Tangent2D<double>(
            double.CreateChecked( X ) / l,
            double.CreateChecked( Y ) / l
        );
    }

    // Conversion to other formats:
    public Tangent2D<TOther> To<TOther>()
    where TOther : INumber<TOther> =>
        new( TOther.CreateChecked( X ), TOther.CreateChecked( Y ) );


}

public record struct Node3D<T>( T X, T Y, T Z )
where T : INumber<T>
{
    public static Node3D<T> operator +( Node3D<T> lhs, Tangent3D<T> rhs ) =>
        new( lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z );
    public static Tangent3D<T> operator -( Node3D<T> lhs, Node3D<T> rhs ) =>
        new( lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z );

    public override string ToString() => $"({X}, {Y}, {Z})";

    public Tangent3D<T> ToTangent() => new( X, Y, Z );
    public static Node3D<T> FromString( string s )
    {
        var parts = s.Split( ',' );
        var provider = CultureInfo.InvariantCulture;
        return new Node3D<T>(
            T.Parse( parts[0].TrimEnd(), provider ),
            T.Parse( parts[1].TrimEnd(), provider ),
            T.Parse( parts[2].TrimEnd(), provider )
        );
    }
}

public record struct Tangent3D<T>( T X, T Y, T Z )
where T : INumber<T>
{
    public override string ToString() => $"[{X}, {Y}, {Z}]";

    // Usefule operators:
    public static Tangent3D<T> operator +( Tangent3D<T> lhs, Tangent3D<T> rhs ) =>
        new( lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z );
    public static Tangent3D<T> operator -( Tangent3D<T> lhs, Tangent3D<T> rhs ) =>
        new( lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z );
    public static Tangent3D<T> operator *( Tangent3D<T> lhs, T rhs ) =>
        new( lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs );
    public static Tangent3D<T> operator *( T lhs, Tangent3D<T> rhs ) =>
        rhs * lhs;
    public static Tangent3D<T> operator -( Tangent3D<T> lhs ) =>
        new( -lhs.X, -lhs.Y, -lhs.Z );

    // Vector operations:
    public T Dot( Tangent3D<T> other ) => X * other.X + Y * other.Y + Z * other.Z;
    public Tangent3D<T> Cross( Tangent3D<T> other ) =>
        new(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X
        );

    public Tangent2D<T> To2D() => new( X, Y );
    // Make it interchangable with tuple
    public static implicit operator (T x, T y, T z)( Tangent3D<T> t ) => (t.X, t.Y, t.Z);
    public static implicit operator Tangent3D<T>( (T x, T y, T z) t ) => new( t.x, t.y, t.z );

    // Parsing
    public static Tangent3D<T> FromString( string s )
    {
        var parts = s.Split( ',' );
        var provider = CultureInfo.InvariantCulture;
        return new Tangent3D<T>(
            T.Parse( parts[0].TrimEnd(), provider ),
            T.Parse( parts[1].TrimEnd(), provider ),
            T.Parse( parts[2].TrimEnd(), provider )
        );
    }

    // Some linear algebra
    public Matrix<double> CrossMatrix()
    {
        var builder = Matrix<double>.Build;
        var x = double.CreateChecked( X );
        var y = double.CreateChecked( Y );
        var z = double.CreateChecked( Z );
        return builder.DenseOfArray( new double[,] {
            {0, -z, y},
            {z, 0, -x},
            {-y, x, 0}
        } );
    }

    public MathNet.Numerics.LinearAlgebra.Vector<double> Concatenate( Tangent3D<T> other ) =>
        MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(
            new double[] {
                double.CreateChecked(X),
                double.CreateChecked(Y),
                double.CreateChecked(Z),
                double.CreateChecked(other.X),
                double.CreateChecked(other.Y),
                double.CreateChecked(other.Z)
            }
        );
}
