using Sprache;
using Utils.Parsers;
namespace Day13;

public static class TreeHelpers
{

    #region Parsers
    public static Tree FromEnumerable(IEnumerable<INode> nodes)
    {
        var result = new Tree();
        foreach (var node in nodes)
        {
            node.Parent = result;
            result.Children.Add(node);
        }
        return result;
    }
    public static readonly Parser<int> Integer =
        from number in Parse.Number
        select int.Parse(number);

    public static readonly Parser<Tree> TreeParser =
        from _1 in Parse.Char('[')
        from tree in Parse.Ref(() => Node).DelimitedBy(Parse.Char(',').Token()).Optional()
        from _2 in Parse.Char(']')
        select FromEnumerable(tree.GetOrElse(new List<INode>()));

    public static readonly Parser<INode> Leaf =
        from number in Integer
        select new Leaf { Size = number };

    public static readonly Parser<INode> Node = Leaf.Or(TreeParser);

    public static Tree FromString(string text)
    {
        return TreeParser.Parse(text);
    }
    #endregion Parsers
}