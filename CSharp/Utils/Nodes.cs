using System.Numerics;
using AoC.Y2023;

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

}

public record struct Node3D<T>( T X, T Y, T Z )
where T : INumber<T>
{

}

public record struct Tangent3D<T>( T X, T Y, T Z )
where T : INumber<T>
{

}
