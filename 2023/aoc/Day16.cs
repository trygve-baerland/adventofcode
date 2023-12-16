using Utils;
using System.Text;
namespace AoC;

public sealed class Day16 : IPuzzle
{
    public MirrorMap TestMap { get; } = MirrorMap.FromChars( "inputdata/day16_test.txt".Stream().GetChars().Split( '\n' ) );
    public MirrorMap ActualMap { get; } = MirrorMap.FromChars( "inputdata/day16.txt".Stream().GetChars().Split( '\n' ) );
    public void Part1()
    {
        var map = ActualMap;
        var result = map.EnergizeFrom(
            new LaserHead( new Point( 0, 0 ), (0, 1) )
        );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = ActualMap;
        // Left side:
        var result = Enumerable.Range( 0, map.Height )
            .Select( i => new LaserHead( new Point( i, 0 ), (0, 1) ) )
            .Then(
                Enumerable.Range( 0, map.Height )
                .Select( i => new LaserHead( new Point( i, map.Width - 1 ), (0, -1) ) )
            )
            .Then(
                Enumerable.Range( 0, map.Width )
                .Select( i => new LaserHead( new Point( 0, i ), (1, 0) ) )
            )
            .Then(
                Enumerable.Range( 0, map.Width )
                .Select( i => new LaserHead( new Point( map.Height - 1, i ), (-1, 0) ) )
            )
            .Select( map.EnergizeFrom )
            .Max();

        Console.WriteLine( result );
    }
}

public struct LaserHead( Point Position, (int x, int y) Direction )
: IEquatable<LaserHead>
{
    public Point Position { get; } = Position;
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

public class MirrorMap( char[][] map )
{
    public char[][] Map { get; } = map;
    public int Height { get; } = map.Length;
    public int Width { get; } = map[0].Length;

    public char this[Point p] => Map[p.X][p.Y];
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

    public bool Contains( Point point ) =>
        point.X >= 0 && point.X < Height &&
        point.Y >= 0 && point.Y < Width;

    public static MirrorMap FromChars( IEnumerable<IEnumerable<char>> chars )
    {
        return new MirrorMap( chars.Select( row => row.ToArray() ).ToArray() );
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach ( var row in Map )
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
