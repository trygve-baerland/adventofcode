using System.Collections;
using System.Text;
using Sprache;
using Utils;
namespace AoC;

public sealed class Day25 : IPuzzle
{
    public ModuleGraph TestGraph() => ModuleGraph.FromFile( "inputdata/day25_test.txt" );
    public ModuleGraph ActualGraph() => ModuleGraph.FromFile( "inputdata/day25.txt" );
    public void Part1()
    {
        var graph = ActualGraph();
        graph.PrintStats();
        var (cut, nodes) = graph.GlobalMinCut();
        Console.WriteLine( $"Cut: {cut}, nodes: {nodes}" );

        var result = (nodes.Count()) * (graph.Modules.Count - nodes.Count());

        Console.WriteLine( result );
    }
}

public record class ModuleGraph()
{
    public List<string> Modules { get; } = new();
    public HashSet<(string, string)> Connections { get; } = new();

    public void PrintStats()
    {
        Console.WriteLine( $"Modules: {Modules.Count}" );
        Console.WriteLine( $"Connections: {Connections.Count}" );
    }
    public void Add( string module )
    {
        if ( !Modules.Contains( module ) )
        {
            Modules.Add( module );
        }
    }
    public void Add( string module1, string module2 ) => Connections.Add( (module1, module2) );

    public static ModuleGraph FromFile( string filename )
    {
        var graph = new ModuleGraph();
        foreach ( var (module, connections) in filename.GetLines().Select( Helpers.ModuleWithConnections.Parse ) )
        {
            graph.Add( module );
            foreach ( var connection in connections )
            {
                graph.Add( connection );
                graph.Add( module, connection );
            }
        }
        return graph;
    }

    public long Connectivity( string a, string b ) =>
        Connections.Contains( (a, b) ) || Connections.Contains( (b, a) ) ? 1L : 0L;

    public long Connectivity( string a, VertexSet b )
    {
        if ( b.Contains( a ) ) return 0L;
        return b.Select( x => Connectivity( a, x ) ).Sum();
    }
    public long Connectivity( VertexSet a, VertexSet b ) =>
        a.SelectMany( x => b.Select( y => Connectivity( x, y ) ) ).Sum();


    public IEnumerable<string> Neighbours( string module ) =>
        Connections
            .Where( kvp => kvp.Item1 == module || kvp.Item2 == module )
            .Select( kvp => kvp.Item1 == module ? kvp.Item2 : kvp.Item1 );

    public (long, VertexSet) GlobalMinCut()
    {
        // Initialize stuff:
        // Vertex sets:
        var (sets, weights) = AdjacencyGraph();

        var best = (cut: long.MaxValue, node: new VertexSet());

        // Main loop:
        var counter = 0;
        while ( sets.Count > 1 )
        {
            var (cut, newSets, newWeights) = MinimumCutPhase( sets, weights );
            weights = newWeights;
            if ( cut < best.cut )
            {
                best = (cut, newSets);
                if ( cut == 3L ) break;
            }
        }
        return best;
    }

    public (long, VertexSet, long[][]) MinimumCutPhase( List<VertexSet> allNodes, long[][] weights )
    {
        // We assume all vertex sets in allNodes are disjoint. And we must keep it that way.
        // We assume length of allNodes is at least 2.
        // Start with an empty set:
        var A = new VertexSet();
        // Add the first element:
        var N = allNodes.Count;
        var w = new long[N];
        // We've taken the zero'th element:
        Array.Copy( weights[0], w, N );
        // w no contains the the weights of the edges from A to every node in allNodes.

        var counter = 1;

        var si = 0;
        var ti = 0;
        long cut = 0L;
        // Main loop:
        while ( counter < allNodes.Count )
        {
            w[ti] = long.MinValue;
            si = ti;
            // Find the index of the most connected node to A:
            ti = Array.IndexOf( w, w.Max() );
            cut = w[ti];
            // Add the new element to the weights of A:
            for ( int i = 0; i < N; i++ )
            {
                w[i] += weights[ti][i];
            }
            counter++; ;
        }
        // We can now merge s and t, and put it into our allNodes:
        var t = allNodes[ti];
        allNodes[si].Union( allNodes[ti] );
        allNodes.RemoveAt( ti );
        // Don't forget to update the weights as well.
        return (cut, t, weights.Merge( si, ti ));
    }

    public (List<VertexSet> nodes, long[][] weights) AdjacencyGraph()
    {
        var nodes = Modules.Select( module => new VertexSet { module } ).ToList();

        var weights = Enumerable.Range( 0, nodes.Count )
        .Select( _ => Enumerable.Repeat( 0L, nodes.Count ).ToArray() )
        .ToArray();

        foreach ( var (a, b) in Connections )
        {
            var ai = nodes.FindIndex( set => set.Contains( a ) );
            var bi = nodes.FindIndex( set => set.Contains( b ) );
            weights[ai][bi] = 1L;
            weights[bi][ai] = 1L;
        }
        return (nodes, weights);
    }
}

public class VertexSet : IEnumerable<string>
{
    private HashSet<string> Vertices { get; } = new();

    public VertexSet Copy()
    {
        var copy = new VertexSet();
        foreach ( var vertex in this )
        {
            copy.Add( vertex );
        }
        return copy;
    }

    public void Add( string vertex ) => Vertices.Add( vertex );
    public bool Contains( string vertex ) => Vertices.Contains( vertex );

    public void Union( VertexSet other ) => Vertices.UnionWith( other.Vertices );

    public IEnumerator<string> GetEnumerator() => Vertices.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        var st = string.Join( ", ", Vertices );
        return $"{{{st}}}";
    }

    public bool IsSubsetOf( VertexSet other ) => Vertices.IsSubsetOf( other.Vertices );
}


public static partial class Helpers
{
    #region parsers
    public static readonly Parser<string> ModuleName =
        Parse.Lower.AtLeastOnce().Text();

    public static readonly Parser<(string, IEnumerable<string>)> ModuleWithConnections =
        ModuleName
            .Then( name => Parse.String( ": " ).Select( _ => name ) )
            .Then( name => ModuleName.DelimitedBy( Parse.String( " " ) ).Select( connections => (name, connections) ) );
    #endregion parsers

    public static long[][] Merge( this long[][] weights, int s, int t )
    {
        var N = weights.Length;
        // Initialize the new array:
        var newWeights = Enumerable.Range( 0, N )
            .Select( _ => Enumerable.Repeat( 0L, N ).ToArray() )
            .ToArray();
        // Start with a basic copy:
        for ( int i = 0; i < N; i++ )
        {
            for ( int j = 0; j < N; j++ )
            {
                newWeights[i][j] = weights[i][j];
            }
        }
        // Now merge s and t:
        for ( int i = 0; i < N; i++ )
        {
            newWeights[s][i] += newWeights[t][i];
            newWeights[i][s] += newWeights[i][t];
        }
        // Remove column t and row t:
        return newWeights.Select( ( row, i ) => (row, i) )
            .Where( item => item.i != t )
            .Select( item => item.row.Select( ( col, j ) => (col, j) )
                .Where( item => item.j != t )
                .Select( item => item.col )
                .ToArray()
            )
            .ToArray();
    }
}
