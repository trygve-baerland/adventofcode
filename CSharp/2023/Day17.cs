using System.Text;
using AoC.Utils;
namespace AoC.Y2023;

public sealed class Day17 : IPuzzle
{
    private FactoryLayout TestLayout { get; } = new FactoryLayout( "2023/inputdata/day17_test.txt".GetLines()
        .Select( line => line.Select( c => c - '0' ).ToArray() )
        .ToArray()
    );

    private FactoryLayout ActualLayout { get; } = new FactoryLayout( "2023/inputdata/day17.txt".GetLines()
        .Select( line => line.Select( c => c - '0' ).ToArray() )
        .ToArray()
    );
    public void Part1()
    {
        var map = ActualLayout;
        var initial = new StandardCrucible( new Node2D<int>( 0, 0 ), (0, 1), 0 );
        var result = map.MinimizeHeatLoss( initial, new Node2D<int>( map.Height - 1, map.Width - 1 ) );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = ActualLayout;
        var initial = new UltraCrucible( new Node2D<int>( 0, 0 ), (0, 1), 0 );
        var result = map.MinimizeHeatLoss( initial, new Node2D<int>( map.Height - 1, map.Width - 1 ) );
        Console.WriteLine( result );
    }
}

internal record class FactoryLayout( int[][] heatMap ) : IntMap( heatMap )
{

    public long MinimizeHeatLoss<T>( T from, Node2D<int> to )
    where T : struct, ICrucible<T>
    {
        var queue = new PriorityQueue<T, long>();
        var seen = new HashSet<T>();
        queue.Enqueue( from, 0L );

        while ( queue.TryDequeue( out var crucible, out var heatLoss ) )
        {
            if ( crucible.Point == to && crucible.CanStop() )
            {
                return heatLoss;
            }
            foreach ( var next in GetNext( crucible ) )
            {
                if ( !seen.Contains( next ) )
                {
                    seen.Add( next );
                    queue.Enqueue( next, heatLoss + this[next.Point] );
                }
            }
        }
        throw new Exception( "No path found" );
    }

    private IEnumerable<T> GetNext<T>( T crucible )
    where T : ICrucible<T>
    => crucible.GetNext().Where( c => Contains( c.Point ) );
}

public interface ICrucible<T>
where T : ICrucible<T>
{
    public Node2D<int> Point { get; }
    public IEnumerable<T> GetNext();

    public bool CanStop();
}
public struct StandardCrucible( Node2D<int> point, (int x, int y) direction, int steps )
: ICrucible<StandardCrucible>
{
    public Node2D<int> Point { get; } = point;
    public (int x, int y) Direction { get; } = direction;
    public int Steps { get; } = steps;

    public override string ToString()
    {
        return $"({Point}, {Direction}, {Steps})";
    }

    public IEnumerable<StandardCrucible> GetNext()
    {
        if ( Steps < 3 )
        {
            yield return new StandardCrucible( Point + Direction, Direction, Steps + 1 );
        }
        yield return new StandardCrucible( Point + (Direction.y, -Direction.x), (Direction.y, -Direction.x), 1 );
        yield return new StandardCrucible( Point + (-Direction.y, Direction.x), (-Direction.y, Direction.x), 1 );
    }

    public bool CanStop() => true;

    public override int GetHashCode() => HashCode.Combine( Point, Direction, Steps );
}

public struct UltraCrucible( Node2D<int> point, (int x, int y) direction, int steps )
: ICrucible<UltraCrucible>
{
    public Node2D<int> Point { get; } = point;
    public (int x, int y) Direction { get; } = direction;
    public int Steps { get; } = steps;

    public override string ToString()
    {
        return $"({Point}, {Direction}, {Steps})";
    }

    public IEnumerable<UltraCrucible> GetNext()
    {
        if ( Steps < 10 )
        {
            yield return new UltraCrucible( Point + Direction, Direction, Steps + 1 );
        }
        if ( Steps >= 4 || Steps == 0 )
        {
            yield return new UltraCrucible( Point + (Direction.y, -Direction.x), (Direction.y, -Direction.x), 1 );
            yield return new UltraCrucible( Point + (-Direction.y, Direction.x), (-Direction.y, Direction.x), 1 );
        }
    }

    public bool CanStop() => Steps >= 4;

    public override int GetHashCode() => HashCode.Combine( Point, Direction, Steps );
}
