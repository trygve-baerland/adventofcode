using Utils;
using Sprache;

namespace Day15;

public static class Parsers
{
    public static readonly Parser<int> Int =
        from op in Parse.Optional(Parse.Char('-').Token())
        from number in Parse.Number
        select int.Parse(number) * (op.IsDefined ? -1 : 1);

    public static readonly Parser<Node> Node =
        from _1 in Parse.String("x=")
        from x in Int
        from _2 in Parse.Char(',').Token()
        from _3 in Parse.String("y=")
        from y in Int
        select new Node { X = x, Y = y };

    public static readonly Parser<(Node sensor, Node beacon)> Pair =
        from header in Parse.String("Sensor at ")
        from sensor in Node
        from mid in Parse.String(": closest beacon is at ")
        from beacon in Node
        select (sensor, beacon);

    public static (Node sensor, Node beacon) FromText(string text)
    {
        return Pair.Parse(text);
    }
}