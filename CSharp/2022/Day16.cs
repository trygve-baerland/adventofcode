using Sprache;
using AoC.Utils;

namespace AoC.Y2022;

public sealed class Day16 : IPuzzle
{
    private Dictionary<string, Valve> TestValves() =>
        "2022/inputdata/day16_test.txt"
        .GetLines()
        .Select( TunnelParsing.ParseValve )
        .ToDictionary(
            valve => valve.Name,
            valve => valve
        );

    private Dictionary<string, Valve> ActualValves() =>
        "2022/inputdata/day16.txt"
        .GetLines()
        .Select( TunnelParsing.ParseValve )
        .ToDictionary(
            valve => valve.Name,
            valve => valve
        );

    public void Part1()
    {
        var valves = ActualValves();
        var shortestPaths = ShortestPath.FloydWarshall(
            valves.Values,
            valve => valve.ConnectedValves.Select( name => valves[name] )
        );

        var path = TunnelHelpers.OptimalRelease(
            source: valves["AA"],
            shortestPathMapping: shortestPaths,
            timeout: 30
        );
        var part1 = TunnelHelpers.EvaluatePath( path, 30 );
        Console.WriteLine( $"Part 1: {part1}" );
        foreach ( var (valve, time) in path )
        {
            Console.Write( $"{valve.Name}[{time}], " );
        }
        Console.WriteLine( "\n-------" );
    }

    public void Part2()
    {
        var valves = ActualValves();
        var shortestPaths = ShortestPath.FloydWarshall(
            valves.Values,
            valve => valve.ConnectedValves.Select( name => valves[name] )
        );
        var part2 = TunnelHelpers.OptimalElephantRelease(
            source: valves["AA"],
            shortestPathMapping: shortestPaths,
            timeout: 26
        );
        Console.WriteLine( part2 );
    }
}

public class Valve
{
    public string Name { get; set; } = string.Empty;
    public int Rate { get; set; }

    public List<string> ConnectedValves { get; } = new List<string>();

    public override string ToString()
    {
        string result = $"('{Name}', ";
        result += $"{Rate}, [";
        result += string.Join( ',', ConnectedValves );
        result += "])";
        return result;
    }
}

public static class TunnelHelpers
{
    public static List<(Valve valve, int time)> OptimalRelease(
        Valve source,
        FloydWarshallMapping<Valve, int> shortestPathMapping,
        int timeout
    )
    {
        // Initialize stuff:
        var stack = new Stack<(Valve node, IEnumerator<(Valve, int)> neighbours, int time, int total, List<(Valve valve, int time)> visited)>();
        stack.Push(
            (
                source,
                GetFrom( shortestPathMapping, source, item => -item.Rate ).Where( item => item.valve.Rate > 0 ).GetEnumerator(),
                0,
                0,
                new List<(Valve valve, int time)>() {
                    (source, 0)
                }
            )
        );
        var totals = new List<List<(Valve valve, int time)>>();
        while ( stack.Count > 0 )
        {
            var (v, neighbours, time, total, visited) = stack.Peek();
            var last = visited.Last();
            if ( v.Name != last.valve.Name )
            {
                throw new Exception( $"Data inconsistency in name {v.Name} != {last.valve.Name}" );
            }
            if ( time != last.time )
            {
                throw new Exception( $"Data inconsistency in time {time} != {last.time}" );
            }
            if ( neighbours.MoveNext() )
            {
                // We are considering going to w next:
                var (w, dist) = neighbours.Current;
                var newTime = time + dist + 1; // Going there and turning it on.
                if ( newTime < timeout && !visited.Select( item => item.valve.Name ).Contains( w.Name ) ) // We only care if can get there in time
                {
                    var newVisited = new List<(Valve valve, int time)>( visited ) // I've been promised this makes a copy...
                    {
                        (w, newTime)
                    };
                    var newTotal = total + (timeout - newTime) * w.Rate;
                    stack.Push(
                        (
                            node: w,
                            neighbours: GetFrom( shortestPathMapping, w, item => -item.Rate ).Where( item => item.valve.Rate > 0 ).GetEnumerator(),
                            time: newTime,
                            total: newTotal,
                            visited: newVisited
                        )
                    );
                }
            }
            else
            {
                var item = stack.Pop();
                totals.Add( item.visited );
            }
        }
        // Get best path:
        Console.WriteLine( $"Searched {totals.Count} paths" );
        var pathVals = totals.Select( ( item, i ) => EvaluatePath( item, timeout ) ).ToList();
        var index = pathVals.IndexOf( pathVals.Max() );
        return totals[index];
    }

    public static int OptimalElephantRelease(
        Valve source,
        FloydWarshallMapping<Valve, int> shortestPathMapping,
        int timeout
    )
    {
        // Initialize stuff:
        var stack = new Stack<(Valve node, IEnumerator<(Valve, int)> neighbours, int time, int total, List<(Valve valve, int time)> visited)>();
        stack.Push(
            (
                source,
                GetFrom( shortestPathMapping, source, item => -item.Rate ).Where( item => item.valve.Rate > 0 ).GetEnumerator(),
                0,
                0,
                new List<(Valve valve, int time)>() {
                    (source, 0)
                }
            )
        );
        var totals = new List<List<(Valve valve, int time)>>();
        while ( stack.Count > 0 )
        {
            var (v, neighbours, time, total, visited) = stack.Peek();
            var last = visited.Last();
            if ( v.Name != last.valve.Name )
            {
                throw new Exception( $"Data inconsistency in name {v.Name} != {last.valve.Name}" );
            }
            if ( time != last.time )
            {
                throw new Exception( $"Data inconsistency in time {time} != {last.time}" );
            }
            if ( neighbours.MoveNext() )
            {
                // We are considering going to w next:
                var (w, dist) = neighbours.Current;
                var newTime = time + dist + 1; // Going there and turning it on.
                if ( newTime < timeout && !visited.Select( item => item.valve.Name ).Contains( w.Name ) ) // We only care if can get there in time
                {
                    var newVisited = new List<(Valve valve, int time)>( visited ) // I've been promised this makes a copy...
                    {
                        (w, newTime)
                    };
                    var newTotal = total + (timeout - newTime) * w.Rate;
                    stack.Push(
                        (
                            node: w,
                            neighbours: GetFrom( shortestPathMapping, w, item => -item.Rate ).Where( item => item.valve.Rate > 0 ).GetEnumerator(),
                            time: newTime,
                            total: newTotal,
                            visited: newVisited
                        )
                    );
                }
            }
            else
            {
                var item = stack.Pop();
                totals.Add( item.visited );
            }
        }
        // Get best path:
        Console.WriteLine( $"Searched {totals.Count} paths" );
        var pathVals = totals.Select( ( item, i ) => EvaluatePath( item, timeout ) ).ToList();
        return totals.CrossProduct( totals )
            .Where( item => NoOverlap( item.Item1, item.Item2 ) )
            .Select( item => EvaluatePath( item.Item1, timeout ) + EvaluatePath( item.Item2, timeout ) )
            .Max();
    }

    public static int EvaluatePath( IEnumerable<(Valve valve, int time)> visited, int timeout )
    {
        return visited.Select( item => (timeout - item.time) * item.valve.Rate ).Sum();
    }

    public static bool NoOverlap( IEnumerable<(Valve valve, int time)> path1, IEnumerable<(Valve valve, int time)> path2 )
    {
        return !path1.Skip( 1 ).Select( item => item.valve.Name )
            .Intersect( path2.Skip( 1 ).Select( item => item.valve.Name ) )
            .Any();
    }

    private static IEnumerable<(Valve valve, int dist)> GetFrom<TKey>(
        FloydWarshallMapping<Valve, int> mapping,
        Valve valve,
        Func<Valve, TKey> selector
    )
    {
        var i = mapping.Index[valve];
        return Enumerable.Range( 0, mapping.Index.Count )
            .Where( j => j != i )
            .Select( j => (mapping.Index.Find( j ), mapping.Distances[i, j]) )
            .OrderBy( item => selector( item.Item1 ) );
    }
}
public static class TunnelParsing
{
    public static Valve CreateValve( string name, int rate, IEnumerable<string> connections )
    {
        var result = new Valve {
            Name = name,
            Rate = rate
        };
        foreach ( var conn in connections )
        {
            result.ConnectedValves.Add( conn );
        }
        return result;
    }
    public static readonly Parser<int> Int =
        from number in Parse.Number
        select int.Parse( number );

    public static readonly Parser<string> Name =
        from c1 in Parse.Upper
        from c2 in Parse.Upper
        select new string( new char[] { c1, c2 } );

    public static readonly Parser<Valve> Valve =
        from _1 in Parse.String( "Valve " )
        from name in Name
        from _2 in Parse.String( " has flow rate=" )
        from rate in Int
        from _3 in Parse.String( "; tunnel" )
        from _4 in Parse.Char( 's' ).Optional()
        from _5 in Parse.String( " lead" )
        from _6 in Parse.Char( 's' ).Optional()
        from _7 in Parse.String( " to valve" )
        from _8 in Parse.Char( 's' ).Optional()
        from _9 in Parse.WhiteSpace
        from connections in Name.Token().DelimitedBy( Parse.Char( ',' ) )
        select CreateValve( name, rate, connections );

    public static Valve ParseValve( string text ) => Valve.Parse( text );
}
