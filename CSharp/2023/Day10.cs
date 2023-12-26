using AoC.Utils;

namespace AoC.Y2023;

public sealed class Day10 : IPuzzle
{
    private PipeMap TestMap { get; } = new PipeMap( "2023/inputdata/day10_test.txt".Stream().GetLines().Select( line => line.ToCharArray() ).ToArray() );
    private PipeMap ActualMap { get; } = new PipeMap( "2023/inputdata/day10.txt".Stream().GetLines().Select( line => line.ToCharArray() ).ToArray() );
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
            .FixPoint( item =>
                (map.Next( item.Item1, item.Item2 ), item.Item1, item.Item3 + 1)
            )
            .TakeWhile(
                item => !item.Item2.Equals( start ) ||
                item.Item1.Equals( item.Item2 ) ||
                item.Item3 < 3
            )
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

internal record class PipeMap( char[][] chars ) : CharMap( chars )
{
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

    public char MapS( Node2D<int> p )
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

    public Node2D<int> GetStart()
    {
        return Data.Select(
            ( row, i ) => (i, row.Select(
                ( col, j ) => (col, j)
            )
            .Where( t => t.col == 'S' )
        ) )
        .Where( item => item.Item2.Any() )
        .Select( item => new Node2D<int>( item.Item1, item.Item2.First().j ) )
        .First();
    }

    public IEnumerable<Node2D<int>> Directions( Node2D<int> p ) =>
        p.Neighbours().Where( Contains );


    public IEnumerable<Node2D<int>> ConnectedPipes( Node2D<int> p, char val )
    {
        switch ( val )
        {
            case 'F':
                if ( p.X < Height - 1 ) yield return p.Down();
                if ( p.Y < Width - 1 ) yield return p.Right();
                break;
            case 'L':
                if ( p.X > 0 ) yield return p.Up();
                if ( p.Y < Width - 1 ) yield return p.Right();
                break;
            case 'J':
                if ( p.X > 0 ) yield return p.Up();
                if ( p.Y > 0 ) yield return p.Left();
                break;
            case '7':
                if ( p.X < Height - 1 ) yield return p.Down();
                if ( p.Y > 0 ) yield return p.Left();
                break;
            case '|':
                if ( p.X > 0 ) yield return p.Up();
                if ( p.X < Width - 1 ) yield return p.Down();
                break;
            case '-':
                if ( p.Y > 0 ) yield return p.Left();
                if ( p.Y < Width - 1 ) yield return p.Right();
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

    public Node2D<int> Next( Node2D<int> current, Node2D<int> prev ) =>
        ConnectedPipes( current, this[current] ).Where( d => d != prev ).First();

}

public static partial class Helpers
{
    public static int WindingNumber( List<Node2D<int>> loop, Node2D<int> x )
    {
        // We need to iterate over all edges in the loop.
        return ( int ) System.Math.Round( loop.Zip( loop.Repeat().Skip( 1 ) )
            .Select( edge => Angle( edge.Item1 - x, edge.Item2 - x ) )
            .Sum() / (2 * System.Math.PI) );
    }

    public static double Angle( Tangent2D<int> p1, Tangent2D<int> p2 )
    {
        var n1 = p1.Normalize();
        var n2 = p2.Normalize();

        var theta = System.Math.Sign( n1.Cross( n2 ) ) * System.Math.Acos( n1.Dot( n2 ) );
        // Now, we need to figure out if the angle is positive or negative.
        return theta;
    }

}
