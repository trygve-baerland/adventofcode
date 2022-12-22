using Sprache;

namespace Day21;

public static class ShoutingMonkeyParser
{
    public static readonly Parser<decimal> Decimal =
        from number in Parse.Number
        select decimal.Parse(number);

    public static readonly Parser<string> Identifier =
        from name in Parse.Lower.Many().Text()
        select name;

    public static readonly Parser<Op> Operation =
        from c in Parse.Chars('+', '-', '*', '/')
        select c switch
        {
            '+' => Op.Plus,
            '-' => Op.Minus,
            '*' => Op.Multiply,
            '/' => Op.Divide,
            _ => throw new ParseException($"Unsupported operation '{c}'")
        };

    public static readonly Parser<ShoutingMonkey> ShoutingMonkey =
        from id in Identifier.Token()
        from _1 in Parse.String(": ")
        from value in Decimal.Token().Optional()
        from id1 in Identifier.Token().Optional()
        from op in Operation.Token().Optional()
        from id2 in Identifier.Token().Optional()
        select new ShoutingMonkey(id, value.IsDefined ? value.Get() : null)
        {
            Lhs = id1.IsDefined ? id1.Get() : null,
            Rhs = id2.IsDefined ? id2.Get() : null,
            Operation = op.IsDefined ? op.Get() : null
        };

    public static ShoutingMonkey FromText(string text) => ShoutingMonkey.Parse(text);

}