using Utils;
using Sprache;

namespace Day18;

public static class LavaParser
{
    public static readonly Parser<int> Int =
        from number in Parse.Number
        select int.Parse(number);

    public static readonly Parser<Node3D<int>> Node3D =
        from x in Int
        from _1 in Parse.Char(',')
        from y in Int
        from _2 in Parse.Char(',')
        from z in Int
        select new Node3D<int>
        {
            X = x,
            Y = y,
            Z = z
        };

    public static Node3D<int> FromText(string text)
    {
        return Node3D.Parse(text);
    }
}