using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Transactions;
using Microsoft.VisualBasic;
using Utils;

namespace AoC;

public sealed class Day10 : IPuzzle
{
    public PipeMap TestMap { get; } = new PipeMap( "inputdata/day10_test.txt".Stream().GetLines().Select( line => line.ToCharArray() ).ToArray() );
    public PipeMap ActualMap { get; } = new PipeMap( "inputdata/day10.txt".Stream().GetLines().Select( line => line.ToCharArray() ).ToArray() );
    public void Part1()
    {
        var map = ActualMap;
        var start = map.GetStart();
        var result = (start, start)
            .FixPoint( item => (map.Next( item.Item1, item.Item2 ), item.Item1) )
            .TakeWhile( item => !item.Item1.Equals( start ) || item.Item1.Equals( item.Item2 ) )
            .Count() / 2;
        Console.WriteLine( result );
    }

    public void Part2()
    {
        // Get points constituting the loop:
        var map = ActualMap;
        var start = map.GetStart();
        var (area, circumference) = (start, start, 0L)
            .FixPoint( item => (map.Next( item.Item1, item.Item2 ), item.Item1, item.Item3 + 1) )
            .TakeWhile( item => !item.Item2.Equals( start ) || item.Item1.Equals( item.Item2 ) || item.Item3 < 3 )
            .Skip( 1 )
            .Aggregate(
                seed: (0L, 0L),
                func: ( acc, item ) => {
                    var area = acc.Item1 + item.Item1.Cross( item.Item2 );
                    var circumference = acc.Item2 + 1;
                    return (area, circumference);
                }
            );

        var result = (System.Math.Abs( area ) - circumference) / 2 + 1;
        Console.WriteLine( result );
    }
}

public struct Point( int x, int y ) : IEquatable<Point>
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public Point Down() => new Point( X + 1, Y );
    public Point Up() => new Point( X - 1, Y );
    public Point Left() => new Point( X, Y - 1 );
    public Point Right() => new Point( X, Y + 1 );

    public (int x, int y) ToTuple() => (X, Y);
    public static (int x, int y) operator -( Point p1, Point p2 ) => (p1.X - p2.X, p1.Y - p2.Y);
    public static bool operator ==( Point p1, Point p2 ) => p1.Equals( p2 );
    public static bool operator !=( Point p1, Point p2 ) => !p1.Equals( p2 );
    public int Cross( Point p ) => X * p.Y - Y * p.X;

    public bool Equals( Point other ) => X == other.X && Y == other.Y;

    public override bool Equals( object? obj ) => obj is Point other && Equals( other );

    public override int GetHashCode()
    {
        return X ^ Y;
    }
}

public class PipeMap( char[][] map )
{
    public char[][] Map { get; } = map;
    private int NC { get; } = map[0].Length;
    private int NR { get; } = map.Length;

    public char this[Point p] => Map[p.X][p.Y];

    private char? sVal = null;
    public char SVal
    {
        get {
            if ( sVal == null )
            {
                var start = GetStart();
                sVal = MapS( start );
            }
            return sVal.Value;
        }
    }

    public IEnumerable<Point> Points() =>
        Enumerable.Range( 0, NR )
        .SelectMany(
            i => Enumerable.Range( 0, NC )
                .Select( j => new Point( i, j ) )
        );

    public char MapS( Point p )
    {
        // Assuming the given point is S, find out what letter it has replaced.
        var neighbours = Directions( p ).Where( d => this[d] != '.' && ConnectedPipes( d, this[d] ).Contains( p ) ).ToArray();

        if ( neighbours.Length != 2 )
            throw new Exception( $"Expected 2 connecting neighbours, found {neighbours.Length}" );
        return (neighbours[1] - neighbours[0]) switch {
            (1, 1 ) => 'L',
            (1, -1 ) => 'F',
            (-1, -1 ) => '7',
            (-1, 1 ) => 'J',
            (0, -2 ) => '-',
            (2, 0 ) => '|',
            _ => throw new Exception( $"Unknown pipe type: {neighbours[1] - neighbours[0]}" )
        };
    }

    public Point GetStart()
    {
        return Map.Select(
            ( row, i ) => (i, row.Select(
                ( col, j ) => (col, j)
            )
            .Where( t => t.col == 'S' )
        ) )
        .Where( item => item.Item2.Any() )
        .Select( item => new Point( item.Item1, item.Item2.First().j ) )
        .First();
    }

    public IEnumerable<Point> Directions( Point p )
    {
        if ( p.X > 0 )
            yield return p.Up();
        if ( p.Y < NC - 1 )
            yield return p.Right();
        if ( p.X < NR - 1 )
            yield return p.Down();
        if ( p.Y > 0 )
            yield return p.Left();
    }

    public IEnumerable<Point> ConnectedPipes( Point p, char val )
    {
        switch ( val )
        {
            case 'F':
                if ( p.X < NR - 1 ) yield return p.Down();
                if ( p.Y < NC - 1 ) yield return p.Right();
                break;
            case 'L':
                if ( p.X > 0 ) yield return p.Up();
                if ( p.Y < NC - 1 ) yield return p.Right();
                break;
            case 'J':
                if ( p.X > 0 ) yield return p.Up();
                if ( p.Y > 0 ) yield return p.Left();
                break;
            case '7':
                if ( p.X < NR - 1 ) yield return p.Down();
                if ( p.Y > 0 ) yield return p.Left();
                break;
            case '|':
                if ( p.X > 0 ) yield return p.Up();
                if ( p.X < NR - 1 ) yield return p.Down();
                break;
            case '-':
                if ( p.Y > 0 ) yield return p.Left();
                if ( p.Y < NC - 1 ) yield return p.Right();
                break;
            case 'S':
                foreach ( var pipe in ConnectedPipes( p, SVal ) )
                {
                    yield return pipe;
                }
                break;
            default:
                throw new Exception( $"Unknown pipe type: {val}" );
        }
    }

    public Point Next( Point current, Point prev ) =>
        ConnectedPipes( current, this[current] ).Where( d => d != prev ).First();

}

public static partial class Helpers
{
    public static int WindingNumber( List<Point> loop, Point x )
    {
        // We need to iterate over all edges in the loop.
        return ( int ) System.Math.Round( loop.Zip( loop.Repeat().Skip( 1 ) )
            .Select( edge => Angle( edge.Item1 - x, edge.Item2 - x ) )
            .Sum() / (2 * System.Math.PI) );
    }

    public static (double x, double y) Normal( (int x, int y) p )
    {
        var (x, y) = p;
        var l = System.Math.Sqrt( x * x + y * y );
        return (x / l, y / l);
    }

    public static double Dot( (double x, double y) p1, (double x, double y) p2 ) => p1.x * p2.x + p1.y * p2.y;

    public static double Cross( (double x, double y) p1, (double x, double y) p2 ) => p1.x * p2.y - p1.y * p2.x;

    public static double Angle( (int x, int y) p1, (int x, int y) p2 )
    {
        var n1 = Normal( p1 );
        var n2 = Normal( p2 );

        var theta = System.Math.Sign( Cross( n1, n2 ) ) * System.Math.Acos( Dot( n1, n2 ) );
        // Now, we need to figure out if the angle is positive or negative.
        return theta;
    }

}
