using System;
using System.IO;
using System.Text.Json;
using System.Transactions;
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

    public static IEnumerable<(T, T)> Pairs<T>( this IEnumerable<T> source )
    {
        var asList = source.ToList();
        foreach ( var i in Enumerable.Range( 0, asList.Count ) )
        {
            foreach ( var j in Enumerable.Range( i + 1, asList.Count - i - 1 ) )
            {
                yield return (asList[i], asList[j]);
            }
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

    public static IEnumerable<T> Then<T>( this IEnumerable<T> source, IEnumerable<T> other )
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

    public static IEnumerable<T> FixPoint<T>( this T source, Func<T, T> func )
    where T : IEquatable<T>
    {
        var current = source;
        yield return current;

        var next = func( current );
        while ( true )
        {
            current = next;
            yield return current;
            next = func( current );
        }
    }

    public static IEnumerable<T> TakeWhile<T>( this IEnumerable<T> source, Func<T, bool> predicate, bool includeLast )
    {
        foreach ( var item in source )
        {
            if ( predicate( item ) )
            {
                yield return item;
            }
            else
            {
                if ( includeLast )
                    yield return item;
                yield break;
            }
        }
    }

    public static IEnumerable<T> OtherWay<T>( this List<T> source )
    {
        for ( int i = source.Count - 1; i >= 0; i-- )
        {
            yield return source[i];
        }
    }

    public static IEnumerable<IEnumerable<T>> Split<T>( this IEnumerable<T> source, T separator )
    where T : IEquatable<T>
    {
        var iterator = source.GetEnumerator();

        while ( iterator.MoveNext() )
        {
            yield return GetNextPartiotion( iterator, separator );
        }
    }

    public static IEnumerable<T> GetNextPartiotion<T>( IEnumerator<T> enumerator, T separator )
    where T : IEquatable<T>
    {
        do
        {
            var item = enumerator.Current;
            if ( item.Equals( separator ) )
            {
                yield break;
            }
            yield return item;
        } while ( enumerator.MoveNext() );
    }
}
