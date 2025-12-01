using System.Text;
using AoC.Utils;
namespace AoC.Y2023;

public sealed class Day16 : IPuzzle
{
    private MirrorMap TestMap { get; } = MirrorMap.FromChars( "2023/inputdata/day16_test.txt".Stream().GetChars().Split( '\n' ) );
    private MirrorMap ActualMap { get; } = MirrorMap.FromChars( "2023/inputdata/day16.txt".Stream().GetChars().Split( '\n' ) );
    public void Part1()
    {
        var map = ActualMap;
        var result = map.EnergizeFrom(
            new LaserHead( new Node2D<int>( 0, 0 ), (0, 1) )
        );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = ActualMap;
        // Left side:
        var result = Enumerable.Range( 0, map.Height )
            .Select( i => new LaserHead( new Node2D<int>( i, 0 ), (0, 1) ) )
            .Then(
                Enumerable.Range( 0, map.Height )
                .Select( i => new LaserHead( new Node2D<int>( i, map.Width - 1 ), (0, -1) ) )
            )
            .Then(
                Enumerable.Range( 0, map.Width )
                .Select( i => new LaserHead( new Node2D<int>( 0, i ), (1, 0) ) )
            )
            .Then(
                Enumerable.Range( 0, map.Width )
                .Select( i => new LaserHead( new Node2D<int>( map.Height - 1, i ), (-1, 0) ) )
            )
            .Select( map.EnergizeFrom )
            .Max();

        Console.WriteLine( result );
    }
}

public struct LaserHead( Node2D<int> Position, (int x, int y) Direction )
: IEquatable<LaserHead>
{
    public Node2D<int> Position { get; } = Position;
    public (int x, int y) Direction { get; } = Direction;

    public bool Equals( LaserHead other )
    {
        return Position.Equals( other.Position ) && Direction.Equals( other.Direction );
    }

    public override string ToString()
    {
        return $"{Position} {Direction}";
    }
}

internal record class MirrorMap( char[][] map ) : CharMap( map )
{
    public IEnumerable<LaserHead> GetNext( LaserHead head )
    {
        foreach ( var dir in Split( head.Direction, this[head.Position] ) )
        {
            var nextPos = head.Position + dir;
            if ( Contains( nextPos ) )
            {
                yield return new LaserHead( nextPos, dir );
            }
        }
    }

    public long EnergizeFrom( LaserHead initial )
    {
        var queue = new Queue<LaserHead>();
        queue.Enqueue( initial );
        var visited = new HashSet<LaserHead>();

        while ( queue.Count > 0 )
        {
            var current = queue.Dequeue();
            visited.Add( current );
            foreach ( var next in GetNext( current ) )
            {
                if ( !visited.Contains( next ) )
                {
                    queue.Enqueue( next );
                }
            }
        }
        return visited.GroupBy( head => head.Position )
            .Count();
    }

    public static MirrorMap FromChars( IEnumerable<IEnumerable<char>> chars )
    {
        return new MirrorMap( chars.Select( row => row.ToArray() ).ToArray() );
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach ( var row in Data )
        {
            builder.AppendLine( string.Concat( row ) );
        }
        return builder.ToString();
    }
    public static IEnumerable<(int x, int y)> Split( (int x, int y) direction, char mirror )
    {
        switch ( mirror )
        {
            case '/':
                yield return (-direction.y, -direction.x);
                break;
            case '\\':
                yield return (direction.y, direction.x);
                break;
            case '-':
                if ( direction.x == 0 )
                {
                    yield return direction;
                }
                else
                {
                    yield return (0, direction.x);
                    yield return (0, -direction.x);
                }
                break;
            case '|':
                if ( direction.y == 0 )
                {
                    yield return direction;
                }
                else
                {
                    yield return (direction.y, 0);
                    yield return (-direction.y, 0);
                }
                break;
            case '.':
                yield return direction;
                break;
            default:
                throw new Exception( $"Unknown mirror: {mirror}" );
        }
    }
}
