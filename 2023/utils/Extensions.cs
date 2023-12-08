using System;
using System.IO;
namespace Utils;
public static class Extensions
{
    public static IEnumerable<string> GetLines( this string filename )
    {
        return File.OpenText( filename ).GetLines();
    }
    public static IEnumerable<string> GetLines( this StreamReader reader )
    {
        string? line;
        while ( (line = reader.ReadLine()) != null )
        {
            yield return line;
        }
    }

    public static IEnumerable<IEnumerable<T>> Clump<T>( this IEnumerable<T> lines, int n )
    {
        int N = lines.Count();
        int counter = 0;
        while ( counter < N )
        {
            yield return lines.Skip( counter ).Take( n );
            counter += n;
        }
    }

    public static StreamReader Stream( this string filename )
    {
        return File.OpenText( filename );
    }

    public static IEnumerable<char> GetChars( this StreamReader reader )
    {
        while ( reader.Peek() >= 0 )
        {
            yield return ( char ) reader.Read();
        }
    }

    public static void ForEach<T>( this IEnumerable<T> source, Action<T> action )
    {
        foreach ( var item in source )
        {
            action( item );
        }
    }

    public static IEnumerable<(T, T)> CrossProduct<T>( this IEnumerable<T> source, IEnumerable<T> other )
    {
        int counter = 0;
        foreach ( var t1 in source )
        {
            Console.Write( $"{counter}\r" );
            foreach ( var t2 in other )
            {
                yield return (t1, t2);
            }
            counter++;
        }
    }

    public static IEnumerable<T> Repeat<T>( this IEnumerable<T> source )
    {
        while ( true )
        {
            foreach ( var item in source )
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> Chain<T>( this IEnumerable<T> source, IEnumerable<T> other )
    {
        foreach ( var x in source ) yield return x;
        foreach ( var y in other ) yield return y;
    }

    public static IEnumerable<T> Flatten<T>( this IEnumerable<IEnumerable<T>> source ) => source.SelectMany( x => x );

    public static IEnumerable<TResult> Accumulate<TSource, TResult>( this IEnumerable<TSource> source, TResult seed, Func<TResult, TSource, TResult> func )
    {
        var result = seed;
        foreach ( var item in source )
        {
            result = func( result, item );
            yield return result;
        }
    }

    public static long gcf( long a, long b )
    {
        while ( b != 0 )
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static long lcm( long a, long b )
    {
        return (a / gcf( a, b )) * b;
    }

}
