using System.Numerics;
namespace AoC.Utils;

public record struct Interval<T>( T A, T B )
where T : INumber<T>
{
    public bool Contains( T x ) => x >= A && x <= B;

    public bool Intersects( Interval<T> other ) =>
        A <= other.B &&
        other.A <= B;

    public T Length() => B - A;
    public T Count() => Length() + T.One;

    public override string ToString() => $"[{A}, {B}]";
}

public record struct Rectangle<T>( Interval<T> X, Interval<T> Y )
where T : INumber<T>
{
    public override string ToString() => $"({X}, {Y})";

    public bool Overlaps( Rectangle<T> other ) =>
        X.Intersects( other.X ) && Y.Intersects( other.Y );
}
