using AoC.Utils;
using Sprache;

namespace AoC.Y2022;

public sealed class Day13 : IPuzzle
{
    private IEnumerable<IPacketData> TestTrees() =>
        "2022/inputdata/day13_test.txt".GetLines()
        .Where( line => line != "" )
        .Select( TreeHelpers.FromString );

    private IEnumerable<IPacketData> ActualTrees() =>
        "2022/inputdata/day13.txt".GetLines()
        .Where( line => line != "" )
        .Select( TreeHelpers.FromString );
    public void Part1()
    {
        var result = ActualTrees()
            .Clump( 2 )
            .Select( ( pair, index ) => (pair.First().IsSorted( pair.Skip( 1 ).First() ), index) )
            .Where( item => item.Item1 != Sorted.False )
            .Select( item => item.index + 1 )
            .Sum();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var allTrees = ActualTrees().ToList();
        allTrees.Add( TreeHelpers.FromString( "[[2]]" ) );
        allTrees.Add( TreeHelpers.FromString( "[[6]]" ) );

        allTrees.Sort( ( tree1, tree2 ) => ( int ) tree1.IsSorted( tree2 ) );
        // Find index of [[2]] and [[6]]
        var valids = new string[] { "[[2]]", "[[6]]" };
        var result = allTrees.Select( ( tree, index ) => (tree, index) )
            .Where( item => valids.Contains( item.tree.ToString() ) )
            .Select( item => item.index + 1 )
            .Take( 2 )
            .Aggregate( ( acc, item ) => acc * item );
        Console.WriteLine( result );
    }
}

public enum Sorted
{
    True = -1,
    Continue = 0,
    False = 1,
}
public interface IPacketData
{
    public IPacketData? Parent { get; set; }
    public string ToString();
    public Sorted IsSorted( IPacketData node );
    public IPacketData ToTree();
}

public class PacketTree : IPacketData
{
    public IPacketData? Parent { get; set; }

    public List<IPacketData> Children { get; } = new();

    public override string ToString()
    {
        string result = "[";
        result += string.Join( ",", Children );
        result += "]";
        return result;
    }

    public Sorted IsSorted( IPacketData node )
    {
        //Console.WriteLine($"Compare {this} with {node}");
        if ( node is PacketTree tree )
        {
            return Children.Zip( tree.Children,
                ( node1, node2 ) => node1.IsSorted( node2 ) )
                .Where( sorted => sorted != Sorted.Continue )
                .FirstOrDefault(
                    (Children.Count() - tree.Children.Count()) switch {
                        < 0 => Sorted.True,
                        > 0 => Sorted.False,
                        _ => Sorted.Continue
                    }
                );
        }
        else
        {
            return IsSorted( node.ToTree() );
        }
    }
    public IPacketData ToTree() => this;
}

public class PacketLeaf : IPacketData
{
    public IPacketData? Parent { get; set; }
    public int Size { get; set; } = 0;

    public override string ToString()
    {
        return Size.ToString();
    }

    public Sorted IsSorted( IPacketData node )
    {
        // Console.WriteLine($"Compare {this} with {node}");
        if ( node.GetType() == typeof( PacketLeaf ) )
        {
            return (Size - (( PacketLeaf ) node).Size) switch {
                < 0 => Sorted.True,
                > 0 => Sorted.False,
                _ => Sorted.Continue
            };
        }
        else
        {
            return ToTree().IsSorted( node );
        }
    }

    public IPacketData ToTree()
    {
        var temp = new PacketTree { };
        temp.Children.Add( this );
        return temp;
    }
}

internal static class TreeHelpers
{

    #region Parsers
    public static PacketTree FromEnumerable( IEnumerable<IPacketData> nodes )
    {
        var result = new PacketTree();
        foreach ( var node in nodes )
        {
            node.Parent = result;
            result.Children.Add( node );
        }
        return result;
    }
    public static readonly Parser<int> Integer =
        from number in Parse.Number
        select int.Parse( number );

    public static readonly Parser<PacketTree> TreeParser =
        from _1 in Parse.Char( '[' )
        from tree in Parse.Ref( () => Node ).DelimitedBy( Parse.Char( ',' ).Token() ).Optional()
        from _2 in Parse.Char( ']' )
        select FromEnumerable( tree.GetOrElse( new List<IPacketData>() ) );

    public static readonly Parser<IPacketData> PacketLeaf =
        from number in Integer
        select new PacketLeaf { Size = number };

    public static readonly Parser<IPacketData> Node = PacketLeaf.Or( TreeParser );

    public static PacketTree FromString( string text )
    {
        return TreeParser.Parse( text );
    }
    #endregion Parsers
}
