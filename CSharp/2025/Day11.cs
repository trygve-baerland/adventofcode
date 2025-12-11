using System.ComponentModel;
using AoC.Utils;
using AoC.Y2024;
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
        var source = "you";
        var result = Graph.BFS(
            source,
            name => {
                if ( data.Servers.ContainsKey( name ) )
                {
                    return data.Servers[name].Outputs;
                }
                return Enumerable.Empty<string>();
            },
            new EmptySet<string>()
        ).Where( node => node.node == "out" )
        .Count();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var data = Data;
        (string name, bool dac, bool fft) source = ("svr", false, false);
        var result = Graph.DFS(
            source,
            tup => {
                //if ( tup.dac || tup.fft ) Console.WriteLine( tup );

                if ( data.Servers.ContainsKey( tup.name ) )
                {
                    return data.Servers[tup.name].Outputs
                    .Select( s =>
                        (s, tup.dac || s == "dac", tup.fft || s == "fft")
                    );
                }
                return Enumerable.Empty<(string, bool, bool)>();
            },
            new EmptySet<(string name, bool dac, bool fft)>()
        ).Where( node => node.name == "out" && node.dac && node.fft )
        .Count();
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
}

record struct ServerPark( Dictionary<string, ServerDevice> Servers )
{
    public static ServerPark FromServers( IEnumerable<ServerDevice> servers )
    {
        return new ServerPark( servers.ToDictionary( s => s.Name ) );
    }

    public void Add( ServerDevice server )
    {
        Servers.Add( server.Name, server );
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
