using AoC.Utils;
using Sprache;

namespace AoC.Y2022;

public sealed class Day15 : IPuzzle
{
    private IEnumerable<(Node2D<int> sensor, Node2D<int> beacon, int dist)> TestInput() =>
        "2022/inputdata/day15_test.txt"
        .GetLines()
        .Select( Day15Parsers.FromText )
        .Select( item => (item.sensor, item.beacon, dist: (item.sensor - item.beacon).L1()) )
        .ToList();

    private IEnumerable<(Node2D<int> sensor, Node2D<int> beacon, int dist)> ActualInput() =>
        "2022/inputdata/day15.txt"
        .GetLines()
        .Select( Day15Parsers.FromText )
        .Select( item => (item.sensor, item.beacon, dist: (item.sensor - item.beacon).L1()) )
        .ToList();

    public void Part1()
    {
        var pairs = ActualInput().ToList();
        var sensors = pairs.Select( item => (item.sensor, item.dist) ).ToList();
        var beacons = new HashSet<Node2D<int>>();
        foreach ( var (sensor, beacon, dist) in pairs )
        {
            beacons.Add( beacon );
            beacons.Add( sensor );
        }

        int row = 2000000;
        var result = Day15Helpers.FeasibleNodes( sensors, row, beacons )
            .Distinct()
            .Where( node => Day15Helpers.CanNotHaveBeacon( node, sensors ) )
            .Count();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var pairs = ActualInput().ToList();
        var sensors = pairs.Select( item => (item.sensor, item.dist) ).ToList();
        var beacons = new HashSet<Node2D<int>>();
        foreach ( var (sensor, beacon, dist) in pairs )
        {
            beacons.Add( beacon );
            beacons.Add( sensor );
        }
        int maxCoord = 4000000;
        var part2 = Day15Helpers.SearchSignal( new Interval<int>( 0, maxCoord ), sensors, beacons )
            .First();
        Console.WriteLine( $"Possible candidate: {part2}: {Day15Helpers.TuningFrequency( part2 )}" );
    }
}

public class Day15Helpers
{

    public static bool CanNotHaveBeacon( Node2D<int> node, IEnumerable<(Node2D<int> sensor, int dist)> sensors )
    {
        return sensors
            .Where( item => (node - item.sensor).L1() <= item.dist )
            .Any();
    }

    public static IEnumerable<Node2D<int>> FeasibleNodes(
        IEnumerable<(Node2D<int> sensor, int dist)> pairs,
        HashSet<Node2D<int>> beacons
    )
    {
        int minY = pairs.Select( pair => pair.sensor.Y - pair.dist ).Min();
        int maxY = pairs.Select( pair => pair.sensor.Y + pair.dist ).Max();
        foreach ( var y in Enumerable.Range( minY, maxY - minY + 1 ) )
        {
            foreach ( var node in FeasibleNodes( pairs, y, beacons ) )
            {
                yield return node;
            }
        }
    }
    public static IEnumerable<Node2D<int>> FeasibleNodes(
        IEnumerable<(Node2D<int> sensor, int dist)> pairs,
        int y,
        HashSet<Node2D<int>> beacons )
    {

        foreach ( var (sensor, dist) in pairs )
        {
            var dy = System.Math.Abs( sensor.Y - y );
            if ( dy <= dist )
            {
                var D = dist - dy;
                for ( int x = sensor.X - D; x <= sensor.X + D; x++ )
                {
                    var candidate = new Node2D<int> { X = x, Y = y };
                    if ( !beacons.Contains( candidate ) )
                        yield return candidate;
                }
            }
        }
    }

    public static IEnumerable<Node2D<int>> SearchSignal(
        Interval<int> box,
        IEnumerable<(Node2D<int> sensor, int dist)> pairs,
        HashSet<Node2D<int>> beacons
    )
    {
        // Go though each sensor
        HashSet<Node2D<int>> visited = new();
        foreach ( var (sensor, dist) in pairs )
        {
            foreach ( var node in EquiDistantNodes( sensor, dist + 1 ) )
            {
                if ( box.Contains( node.X ) && box.Contains( node.Y ) && !CanNotHaveBeacon( node, pairs ) )
                {
                    if ( !visited.Contains( node ) )
                    {
                        visited.Add( node );
                        yield return node;
                    }
                }
            }
        }
    }

    public static long TuningFrequency( Node2D<int> node )
    {
        return 4000000 * (( long ) node.X) + node.Y;
    }

    public static IEnumerable<Node2D<int>> EquiDistantNodes( Node2D<int> center, int dist )
    {
        var node = new Node2D<int>( center.X - dist, center.Y );
        foreach ( var dir in Directions )
        {
            foreach ( var _ in Enumerable.Range( 0, dist ) )
            {
                yield return node;
                node += dir;
            }
        }
    }

    public static readonly List<(int dx, int dy)> Directions = new() { (1, -1), (1, 1), (-1, 1), (-1, -1) };
}
public static class Day15Parsers
{
    public static readonly Parser<int> Int =
        from op in Parse.Optional( Parse.Char( '-' ).Token() )
        from number in Parse.Number
        select int.Parse( number ) * (op.IsDefined ? -1 : 1);

    public static readonly Parser<Node2D<int>> Node =
        from _1 in Parse.String( "x=" )
        from x in Int
        from _2 in Parse.Char( ',' ).Token()
        from _3 in Parse.String( "y=" )
        from y in Int
        select new Node2D<int> { X = x, Y = y };

    public static readonly Parser<(Node2D<int> sensor, Node2D<int> beacon)> Pair =
        from header in Parse.String( "Sensor at " )
        from sensor in Node
        from mid in Parse.String( ": closest beacon is at " )
        from beacon in Node
        select (sensor, beacon);

    public static (Node2D<int> sensor, Node2D<int> beacon) FromText( string text )
    {
        return Pair.Parse( text );
    }
}
