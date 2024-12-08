using System.Numerics;
namespace AoC.Utils;

public static class Math
{
    /// Greatest common factor
    public static T GCF<T>( T a, T b )
    where T : INumber<T>
    {
        while ( b != T.Zero )
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /// Least common multiple
    public static T LCM<T>( T a, T b )
    where T : INumber<T>
    {
        return a / GCF( a, b ) * b;
    }
    public static T MathMod<T>( T a, T b )
    where T : INumber<T>
    {
        return ((a % b) + b) % b;
    }

    public static bool IsEven( this int number )
    {
        return number % 2 == 0;
    }

    public static long ManhattanRadius( long x ) => 4 * x;

    public static T CheckedSum<T>( T a, T b )
    where T : INumber<T>, IMinMaxValue<T>
    {
        try
        {
            return checked(a + b);
        }
        catch ( OverflowException )
        {
            return T.MaxValue;
        }
    }

    public static long Concat( long a, long b )
    {
        return long.Parse( a.ToString() + b.ToString() );
    }
}
