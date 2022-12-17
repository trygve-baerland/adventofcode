using Sprache;
using Utils;
namespace Day14;

public static class Parser
{
    public static readonly Parser<int> UInt =
        from number in Parse.Number
        select int.Parse(number);

    public static readonly Parser<Node<int>> IntPair =
        from num1 in UInt
        from _ in Parse.Char(',')
        from num2 in UInt
        select new Node<int> { X = num1, Y = num2 };

    public static readonly Parser<IEnumerable<Node<int>>> RockLine =
        from rocks in IntPair.DelimitedBy(Parse.String(" -> ")).Optional()
        select rocks.GetOrElse(new List<Node<int>>());

    // Helper methods:
    public static IEnumerable<Node<int>> GetRockLine(string text)
    {
        return RockLine.Parse(text);
    }
}