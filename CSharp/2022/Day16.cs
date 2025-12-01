using AoC.Utils;
using AoC.Y2023;
using Sprache;

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

        var result = TunnelHelpers.OptimalRelease(
            source: valves["AA"],
            shortestPathMapping: shortestPaths,
            timeout: 30
        );

        Console.WriteLine( result );
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
    private record struct SearchState(
        Valve current,
        int time,
        int pressureRelease,
        ulong visited
    )
    {
        public IEnumerable<SearchState> Next( FloydWarshallMapping<Valve, int> mapping, int timeout )
        {
            //Console.WriteLine( $"Considering {this}" );
            // Thread.Sleep( 500 );
            if ( time >= timeout )
            {
                yield break;
            }
            // First, if we haven't turned the current valve:
            var currentBit = GetBit( mapping.Index[current] );
            if ( (visited & currentBit) == 0 && current.Rate > 0 )
            {
                var newTime = time + 1;
                var release = (timeout - newTime) * current.Rate;
                yield return new SearchState(
                    current: current,
                    time: time + 1,
                    pressureRelease: pressureRelease + release,
                    visited: visited | currentBit
                );
            }
            // Go through each neighbour:
            foreach ( var (valve, index) in mapping.Index )
            {
                // We only consider it if it hasn't been visited
                var nextBit = GetBit( index );
                if ( (visited & nextBit) == 0 && valve.Rate > 0 )
                {
                    var newTime = time + mapping.Distances[mapping.Index[current], index] + 1;
                    if ( newTime < timeout )
                    {
                        var release = (timeout - newTime) * valve.Rate;
                        yield return new SearchState(
                            current: valve,
                            time: newTime,
                            pressureRelease: pressureRelease + release,
                            visited: visited | nextBit
                        );
                    }
                }
            }
        }

        private static ulong GetBit( int index ) => 1UL << (index + 1);

        public override string ToString() => $"({current.Name}, {time}, {pressureRelease}, {Convert.ToString( ( long ) visited, 2 )})";
    }
    public static long OptimalRelease(
        Valve source,
        FloydWarshallMapping<Valve, int> shortestPathMapping,
        int timeout
    )
    {
        var initial = new SearchState( source, 0, 0, 0L );
        // We'll do it as a DFS search:
        return Graph.DFS(
            source: initial,
            adjacentNodes: state => state.Next( mapping: shortestPathMapping, timeout: timeout )
        ).Select( item => item.pressureRelease ).Max();
    }

    public static int OptimalElephantRelease(
        Valve source,
        FloydWarshallMapping<Valve, int> shortestPathMapping,
        int timeout
    )
    {
        var initial = new SearchState( source, 0, 0, 0L );
        var candidates = Graph.DFS(
            source: initial,
            adjacentNodes: state => state.Next( mapping: shortestPathMapping, timeout: timeout )
        )
        .Select( state => (state.visited, state.pressureRelease) )
        .ToList();

        // Get best path:
        Console.WriteLine( $"Searched {candidates.Count} paths" );
        return candidates
            .CrossProduct( candidates )
            .Where( item => (item.Item1.visited & item.Item2.visited) == 0 )
            .Select( item => item.Item1.pressureRelease + item.Item2.pressureRelease )
            .Max();
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
