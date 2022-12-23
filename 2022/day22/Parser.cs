using Sprache;

namespace Day22;

public static class TracingParser
{
    public static readonly Parser<int> Int =
        from number in Parse.Number
        select int.Parse(number);

    public static readonly Parser<Rotation> RotationParser =
        from c in Parse.Chars('R', 'L')
        select Enum.Parse<Rotation>(c.ToString());
    public static readonly Parser<(int numSteps, Rotation rotation)> Pair =
        from number in Int
        from protation in RotationParser
        select (numSteps: number, rotation: protation);

    public static readonly Parser<(IEnumerable<(int numSteps, Rotation rotation)>, int)> PathSpec =
        from pairs in Pair.Many()
        from last in Int
        select (pairs, last);

    public static (IEnumerable<(int numSteps, Rotation rotation)>, int) FromText(string text) => PathSpec.Parse(text);
}