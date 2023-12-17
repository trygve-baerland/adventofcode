using System.Runtime.CompilerServices;
using System.Text;
using Utils;
namespace AoC;

public sealed class Day17 : IPuzzle
{
    public FactoryLayout TestLayout { get; } = new FactoryLayout( "inputdata/day17_test.txt".GetLines()
        .Select( line => line.Select( c => c - '0' ).ToArray() )
        .ToArray()
    );

    public FactoryLayout ActualLayout { get; } = new FactoryLayout( "inputdata/day17.txt".GetLines()
        .Select( line => line.Select( c => c - '0' ).ToArray() )
        .ToArray()
    );
    public void Part1()
    {
        var map = ActualLayout;
        var initial = new StandardCrucible( new Point( 0, 0 ), (0, 1), 0 );
        var result = map.MinimizeHeatLoss( initial, new Point( map.Height - 1, map.Width - 1 ) );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = ActualLayout;
        var initial = new UltraCrucible( new Point( 0, 0 ), (0, 1), 0 );
        var result = map.MinimizeHeatLoss( initial, new Point( map.Height - 1, map.Width - 1 ) );
        Console.WriteLine( result );
    }
}

public class FactoryLayout( int[][] heatMap )
{
    public int[][] HeatMap { get; } = heatMap;
    public int Height => HeatMap.Length;
    public int Width => HeatMap[0].Length;

    public long this[Point p] => HeatMap[p.X][p.Y];

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach ( var row in HeatMap )
        {
            builder.AppendLine( string.Concat( row ) );
        }
        return builder.ToString();
    }

    public long MinimizeHeatLoss<T>( T from, Point to )
    where T : struct, ICrucible<T>
    {
        var queue = new PriorityQueue<T, long>();
        //var seen = new HashSet<T>();
        var seen = new VisitedMap( Height, Width );
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

    public bool Contains( Point p ) => p.X >= 0 && p.X < Height && p.Y >= 0 && p.Y < Width;

    private IEnumerable<T> GetNext<T>( T crucible )
    where T : ICrucible<T>
    => crucible.GetNext().Where( c => Contains( c.Point ) );
}

public class VisitedMap( int height, int width )
{
    private bool[][][][] Map { get; } = Enumerable.Range( 0, height )
        .Select( _ => Enumerable.Range( 0, width )
            .Select( _ => Enumerable.Range( 0, 4 )
                .Select( _ => new bool[11] )
                .ToArray()
            .ToArray()
        )
        .ToArray()
    ).ToArray();

    public bool this[Point p, (int x, int y) direction, int steps]
    {
        get => Map[p.X][p.Y][DirIndex( direction )][steps];
        set {
            Map[p.X][p.Y][DirIndex( direction )][steps] = value;
        }
    }

    private static int DirIndex( (int x, int y) direction )
    {
        if ( direction == (0, 1) ) return 0;
        if ( direction == (0, -1) ) return 1;
        if ( direction == (1, 0) ) return 2;
        if ( direction == (-1, 0) ) return 3;
        throw new Exception( "Invalid direction" );
    }

    public bool Contains<T>( T crucible )
    where T : ICrucible<T>
    {
        return this[crucible.Point, crucible.Direction, crucible.Steps];
    }

    public bool Add<T>( T crucible )
    where T : ICrucible<T>
    {
        var result = !Contains( crucible );
        this[crucible.Point, crucible.Direction, crucible.Steps] = true;
        return result;
    }
}

public interface ICrucible<T>
where T : ICrucible<T>
{
    public Point Point { get; }
    public (int x, int y) Direction { get; }
    public int Steps { get; }
    public IEnumerable<T> GetNext();

    public bool CanStop();
}
public struct StandardCrucible( Point point, (int x, int y) direction, int steps )
: ICrucible<StandardCrucible>
{
    public Point Point { get; } = point;
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
}

public struct UltraCrucible( Point point, (int x, int y) direction, int steps )
: ICrucible<UltraCrucible>
{
    public Point Point { get; } = point;
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
}
