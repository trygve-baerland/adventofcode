using System.Text;
using AoC.Utils;
using Sprache;

namespace AoC.Y2025;

public sealed class Day11 : IPuzzle
{
    private ServerPark TestData =
        ServerPark.FromServers( "2025/inputdata/day11_test.txt".GetLines()
        .Select( Helpers.ServerDevice.Parse )
        );
    private ServerPark TestData2 =
        ServerPark.FromServers( "2025/inputdata/day11_test2.txt".GetLines()
        .Select( Helpers.ServerDevice.Parse )
        );
    private ServerPark Data =
        ServerPark.FromServers( "2025/inputdata/day11.txt".GetLines()
        .Select( Helpers.ServerDevice.Parse )
        );

    public void Part1()
    {
        var data = Data;
        Console.WriteLine( data.Paths( "you", "out" ) );
    }

    public void Part2()
    {
        var data = Data;
        List<string> stops = ["svr", "fft", "dac", "out"];
        var result = stops.TakeTwo()
            .Select( p => data.Paths( p.Item1, p.Item2 ) )
            .Aggregate( 1L, ( r, c ) => r * c );
        Console.WriteLine( result );

    }
}

record struct ServerDevice( string Name, List<string> Outputs )
{
    public override readonly string ToString() =>
        $"{Name}: {string.Join( ' ', Outputs )}";

    public static ServerDevice New( string name )
    {
        return new ServerDevice( name, new List<string>() );
    }
    public void AddOutput( string name )
    {
        Outputs.Add( name );
    }

    public string ToDot()
    {
        var sb = new StringBuilder();
        foreach ( var s in Outputs )
        {
            sb.AppendLine( $"{Name} -> {s}" );
        }
        return sb.ToString();
    }
}

record class PathCache( ServerPark park, string target, Dictionary<string, long> cache ) : IGraphVisitedSet<string>
{
    public static PathCache New( ServerPark park, string target )
    {
        return new PathCache( park, target, new Dictionary<string, long>() );
    }

    public long this[string name] => cache[name];
    public bool Contains( string name ) => cache.ContainsKey( name );

    public bool Add( string name )
    {
        if ( cache.ContainsKey( name ) ) return true;

        if ( name == target )
        {
            cache[name] = 1;
            return true;
        }

        // If all it's neighbors are in the cache, we add it:
        var connections = park.NeighborsOf( name ).ToList();
        if ( connections.All( cache.ContainsKey ) )
        {
            cache[name] = connections.Sum( cand => cache[cand] );
        }
        return true;
    }
}

record struct ServerPark( Dictionary<string, ServerDevice> Servers )
{
    public static ServerPark FromServers( IEnumerable<ServerDevice> servers )
    {
        return new ServerPark( servers.ToDictionary( s => s.Name ) );
    }

    public IEnumerable<string> NeighborsOf( string name )
    {
        if ( Servers.ContainsKey( name ) )
        {
            return Servers[name].Outputs;
        }
        return Enumerable.Empty<string>();
    }

    public IEnumerable<string> GoesTo( string name )
    {
        return Servers.Where( item => item.Value.Outputs.Contains( name ) )
            .Select( item => item.Key );
    }

    public string ToDot()
    {
        var sb = new StringBuilder();
        foreach ( var s in Servers.Values )
        {
            sb.AppendLine( s.ToDot() );
        }
        return sb.ToString();
    }

    public void Add( ServerDevice server )
    {
        Servers.Add( server.Name, server );
    }

    public long Paths( string source, string target )
    {
        var cache = PathCache.New( this, target );
        var servers = Servers;

        var it = Graph.DFS(
            source,
            name => {
                if ( servers.ContainsKey( name ) )
                {
                    return servers[name].Outputs;
                }
                return Enumerable.Empty<string>();
            },
            cache
        ).GetEnumerator();
        while ( it.MoveNext() )
        {
            cache.Add( it.Current );
        }
        return cache[source];
    }
    public static long Paths( ServerPark park, string source, string target, Dictionary<(string, string), long> cache )
    {
        if ( cache.ContainsKey( (source, target) ) ) return cache[(source, target)];
        if ( source == target ) return 1;
        var result = park.NeighborsOf( source ).Sum( name => Paths( park, name, target, cache ) );
        cache[(source, target)] = result;
        return result;
    }

    public override readonly string ToString() =>
        $"{string.Join( '\n', Servers.Values )}";
}

static partial class Helpers
{
    public static readonly Parser<string> Ident =
        Parse.AnyChar.Except( Parse.WhiteSpace.Or( Parse.Char( ':' ) ) ).AtLeastOnce()
            .Select( c => string.Concat( c ) );

    public static readonly Parser<ServerDevice> ServerDevice =
        Ident.Then( name => Parse.Char( ':' ).Then( _ => Parse.WhiteSpace.Many() ).Select( _ => name ) )
        .Then( name => Ident.DelimitedBy( Parse.WhiteSpace.AtLeastOnce() ).Select( outputs => (name, outputs) ) )
        .Select( p => new Y2025.ServerDevice( p.name, p.outputs.ToList() ) );
}
