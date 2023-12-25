using Sprache;
using AoC.Utils;

namespace AoC.Y2023;

public sealed class Day8 : IPuzzle
{
    public (IEnumerable<char> directions, NodeMapping mapping) TestData = Helpers.Day8Parser.Parse(
           string.Concat( "2023/inputdata/day8_test.txt".Stream().GetChars() ) );

    public (IEnumerable<char> directions, NodeMapping mapping) ActualData = Helpers.Day8Parser.Parse(
           string.Concat( "2023/inputdata/day8.txt".Stream().GetChars() ) );
    public void Part1()
    {
        var (directions, mapping) = ActualData;
        var result = mapping.CountFrom( directions.Repeat(), "AAA", name => name == "ZZZ" );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var (directions, mapping) = ActualData;

        var result = mapping.Nodes.Keys.Where( key => key.EndsWith( 'A' ) )
            .Select( seed => mapping.CountFrom( directions.Repeat(), seed, name => name.EndsWith( 'Z' ) ) )
            .Aggregate( seed: ( long ) 1, func: Utils.Math.LCM );
        Console.WriteLine( result );
    }
}

public class Node( string name, string left, string right )
{
    public string Name { get; } = name;
    public string Left { get; } = left;
    public string Right { get; } = right;

    public override string ToString()
    {
        return $"{Name} = ({Left}, {Right})";
    }

    public string Go( char direction ) => direction switch {
        'L' => Left,
        'R' => Right,
        _ => throw new Exception( "Invalid direction" )
    };
}

public class NodeMapping( IEnumerable<Node> nodes )
{
    public Dictionary<string, Node> Nodes { get; } = nodes.ToDictionary( n => n.Name );

    public string GoLeft( string name ) => Nodes[name].Left;
    public string GoRight( string name ) => Nodes[name].Right;

    public string Go( string name, char direction ) => Nodes[name].Go( direction );

    public IEnumerable<string> Go( IEnumerable<string> names, char direction, long counter )
    {
        Console.Write( $"{counter}\r" );
        return names.Select( name => Go( name, direction ) );
    }

    public long CountFrom( IEnumerable<char> directions, string start, Func<string, bool> atEnd )
    => directions.Repeat().Accumulate(
        seed: start,
        func: Go
    ).TakeWhile( name => !atEnd( name ) )
    .Count() + 1;
}
public static partial class Helpers
{
    #region parser stuff
    public static readonly Parser<string> NodeName = Parse.Upper.Or( Parse.Digit ).Repeat( 3 ).Text();

    public static readonly Parser<Node> NodeParser =
        NodeName
            .Then( name => Parse.String( " = (" ).Select( _ => name ) )
            .Then( name => NodeName.Select( left => (name, left) ) )
            .Then( tuple => Parse.String( ", " ).Then( _ => NodeName.Select( right => (tuple.name, tuple.left, right) ) ) )
            .Then( tuple => Parse.String( ")" ).Select( _ => tuple ) )
            .Select( tuple => new Node( tuple.name, tuple.left, tuple.right ) );

    public static readonly Parser<(IEnumerable<char> directions, NodeMapping mapping)> Day8Parser =
        Parse.Chars( 'R', 'L' ).Many()
            .Then( dirs => Parse.LineEnd.AtLeastOnce().Select( _ => dirs ) )
            .Then( dirs => NodeParser.DelimitedBy( Parse.LineEnd )
                .Select( nodes =>
                    (dirs, new NodeMapping( nodes ))
                )
            );
    #endregion parser stuff
}
