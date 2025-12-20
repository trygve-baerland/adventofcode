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

    public static T Concat<T>( T a, T b )
    where T : INumber<T>
    {
        return T.Parse( a.ToString() + b.ToString(), null );
    }

    /// <summary>
    ///  Get number of digits in base 10 of a number
    /// </summary>
    /// <typeparam name="T">Number type</typeparam>
    /// <param name="num">a number</param>
    /// <returns></returns>
    public static int NumberOfDigits<T>( this T num )
    where T : INumber<T>
    {
        return ( int ) System.Math.Log10( double.CreateChecked( num ) ) + 1;
    }

    public static int DigitAt<T>( this T num, int pos )
    where T : INumber<T>
    {
        if ( pos < 0 )
        {
            return 0;
        }
        var numDigs = num.NumberOfDigits();
        if ( pos > numDigs )
        {
            return 0;
        }
        return ( int ) (double.CreateChecked( num ) / System.Math.Pow( 10, numDigs - pos - 1 )) % 10;
    }

    public static bool IsInteger( this double num )
    {
        return double.Abs( num - double.Floor( num ) ) < 1E-8 || double.Abs( double.Ceiling( num ) - num ) < 1E-8;
    }
}
