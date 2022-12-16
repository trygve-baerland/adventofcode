using Sprache;
using Utils;

namespace Day16;

public static class TunnelParsing
{
    public static Valve CreateValve(string name, int rate, IEnumerable<string> connections)
    {
        var result = new Valve
        {
            Name = name,
            Rate = rate
        };
        foreach (var conn in connections)
        {
            result.ConnectedValves.Add(conn);
        }
        return result;
    }
    public static readonly Parser<int> Int =
        from number in Parse.Number
        select int.Parse(number);

    public static readonly Parser<string> Name =
        from c1 in Parse.Upper
        from c2 in Parse.Upper
        select new string(new char[] { c1, c2 });

    public static readonly Parser<Valve> Valve =
        from _1 in Parse.String("Valve ")
        from name in Name
        from _2 in Parse.String(" has flow rate=")
        from rate in Int
        from _3 in Parse.String("; tunnel")
        from _4 in Parse.Char('s').Optional()
        from _5 in Parse.String(" lead")
        from _6 in Parse.Char('s').Optional()
        from _7 in Parse.String(" to valve")
        from _8 in Parse.Char('s').Optional()
        from _9 in Parse.WhiteSpace
        from connections in Name.Token().DelimitedBy(Parse.Char(','))
        select CreateValve(name, rate, connections);

    public static Valve ParseValve(string text) => Valve.Parse(text);
}