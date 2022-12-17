using Sprache;

namespace Day17;

public static class TetrisParser
{
    public static readonly Parser<Move> MoveParser =
        from letter in Parse.Char('>').Or(Parse.Char('<'))
        select letter switch
        {
            '<' => Move.Left,
            '>' => Move.Right,
            _ => throw new Exception("Invalid direction '{letter}'")
        };

    public static IEnumerable<Move> MovesFromText(string text) => MoveParser.Many().Parse(text).ToList();
}