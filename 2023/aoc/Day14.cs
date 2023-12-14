using Utils;
using Sprache;
using System.Text;
namespace AoC;

public sealed class Day14 : IPuzzle
{
    public (TiltablePlatform platform, List<Point> rollables) TestData { get; } = TiltablePlatform.FromLines( "inputdata/day14_test.txt".GetLines().ToList() );
    public (TiltablePlatform platform, List<Point> rollables) ActualData { get; } = TiltablePlatform.FromLines( "inputdata/day14.txt".GetLines().ToList() );
    public void Part1()
    {
        var (platform, rollables) = ActualData;
        // Roll north:
        var rolled = platform.RollNorth( rollables );
        var result = platform.ScoreNorth( rolled );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var (platform, rollables) = ActualData;
        var visited = (
            platform,
            rollables,
            loads: new List<long>() { platform.ScoreNorth( rollables ) },
            seen: new List<long>() { platform.ScorePosition( rollables ) }
        )
            .FixPoint( item => {
                var rolled = item.platform.Cycle( item.rollables );
                var load = item.platform.ScoreNorth( rolled );
                item.loads.Add( load );
                var score = item.platform.ScorePosition( rolled );
                item.seen.Add( score );
                return (item.platform, rolled, item.loads, item.seen);
            } )
            .TakeWhile( item => !item.seen.GroupBy( score => score ).Any( g => g.Count() > 1 ) )
            .Last();
        // We found a repeat score, should probably find at what indices
        var repeatScore = visited.seen.GroupBy( score => score ).First( g => g.Count() > 1 ).Key;
        var firstRepeatIndex = visited.seen.IndexOf( repeatScore );
        var lastRepeatIndex = visited.seen.LastIndexOf( repeatScore );
        var indexOfDesiredScore = (1_000_000_000 - firstRepeatIndex) % (lastRepeatIndex - firstRepeatIndex) + firstRepeatIndex;
        Console.WriteLine( visited.loads[indexOfDesiredScore] );
    }
}

public class TiltablePlatform( IEnumerable<Point> rocks, int height, int width )
{

    public int Height { get; } = height;
    public int Width { get; } = width;
    public List<Point> Rocks { get; } = rocks.ToList();

    public static (TiltablePlatform platform, List<Point> rollables) FromLines( List<string> lines )
    {
        var rocks = lines.WhereIsChar( '#' );
        var rollables = lines.WhereIsChar( 'O' ).ToList();

        return (new TiltablePlatform( rocks, lines.Count, lines[0].Length ), rollables);
    }

    public override string ToString()
    {
        var result = $"Height: {Height}, Width: {Width}\n";
        result += string.Join( ", ", Rocks );
        return result;
    }

    public void Print( List<Point> rollables )
    {
        for ( int i = 0; i < Height; i++ )
        {
            for ( int j = 0; j < Width; j++ )
            {
                var point = new Point( i, j );
                if ( Rocks.Contains( point ) )
                {
                    Console.Write( '#' );
                }
                else if ( rollables.Contains( point ) )
                {
                    Console.Write( 'O' );
                }
                else
                {
                    Console.Write( '.' );
                }
            }
            Console.WriteLine();
        }
    }

    public long ScorePosition( List<Point> rollables ) =>
        rollables.Select( r => ( long ) Width * r.X * r.Y ).Sum();

    public List<Point> RollNorth( List<Point> rollables )
    {
        // Going north, we sort every rollable by X,
        List<Point> result = new();
        foreach ( var rock in rollables.OrderBy( p => p.X ) )
        {
            var x = Rocks.Where( r => r.Y == rock.Y && r.X < rock.X )
                .Select( r => r.X )
                .DefaultIfEmpty( -1 )
                .Max();
            // But it can also be blocked by other rocks:
            var otherX = result.Where( r => r.Y == rock.Y && r.X < rock.X )
                .Select( r => r.X )
                .DefaultIfEmpty( -1 )
                .Max();

            x = System.Math.Max( x, otherX );

            result.Add( new Point( x + 1, rock.Y ) );
        }
        return result;
    }

    public List<Point> RollSouth( List<Point> rollables )
    {
        // Going north, we sort every rollable by X in descending order,
        List<Point> result = new();
        foreach ( var rock in rollables.OrderBy( p => -p.X ) )
        {
            var x = Rocks.Where( r => r.Y == rock.Y && r.X > rock.X )
                .Select( r => r.X )
                .DefaultIfEmpty( Height )
                .Min();
            // But it can also be blocked by other rocks:
            var otherX = result.Where( r => r.Y == rock.Y && r.X > rock.X )
                .Select( r => r.X )
                .DefaultIfEmpty( Height )
                .Min();

            x = System.Math.Min( x, otherX );

            result.Add( new Point( x - 1, rock.Y ) );
        }
        return result;
    }
    public List<Point> RollWest( List<Point> rollables )
    {
        // Going north, we sort every rollable by Y,
        List<Point> result = new();
        foreach ( var rock in rollables.OrderBy( p => p.Y ) )
        {
            var x = Rocks.Where( r => r.X == rock.X && r.Y < rock.Y )
                .Select( r => r.Y )
                .DefaultIfEmpty( -1 )
                .Max();
            // But it can also be blocked by other rocks:
            var otherX = result.Where( r => r.X == rock.X && r.Y < rock.Y )
                .Select( r => r.Y )
                .DefaultIfEmpty( -1 )
                .Max();

            x = System.Math.Max( x, otherX );

            result.Add( new Point( rock.X, x + 1 ) );
        }
        return result;
    }
    public List<Point> RollEast( List<Point> rollables )
    {
        // Going north, we sort every rollable by Y,
        List<Point> result = new();
        foreach ( var rock in rollables.OrderBy( p => -p.Y ) )
        {
            var x = Rocks.Where( r => r.X == rock.X && r.Y > rock.Y )
                .Select( r => r.Y )
                .DefaultIfEmpty( Width )
                .Min();
            // But it can also be blocked by other rocks:
            var otherX = result.Where( r => r.X == rock.X && r.Y > rock.Y )
                .Select( r => r.Y )
                .DefaultIfEmpty( Width )
                .Min();

            x = System.Math.Min( x, otherX );

            result.Add( new Point( rock.X, x - 1 ) );
        }
        return result;
    }

    public List<Point> Cycle( List<Point> rollables )
    {
        return RollEast( RollSouth( RollWest( RollNorth( rollables ) ) ) );
    }

    public long ScoreNorth( List<Point> rollables )
    {
        return rollables.Select( r => ( long ) Height - r.X ).Sum();
    }
}

public static partial class Helpers
{
    public static IEnumerable<Point> WhereIsChar( this IEnumerable<string> source, char c ) =>
        source.SelectMany( ( line, i ) =>
            line.Select( ( c, j ) => (c, j) )
                .Where( t => t.c == c )
                .Select( t => new Point( i, t.j ) )
        );
}
