using Sprache;

namespace Day19;

public static class BlueprintParser
{
    public static readonly Parser<int> Int =
        from number in Parse.Number
        select int.Parse(number);

    public static readonly Parser<Cost> Cost =
        from _1 in Parse.String("costs ")
        from ore in Int
        from _2 in Parse.String(" ore")
        from clay in (
            from _3 in Parse.String(" and ")
            from clay in Int
            from _4 in Parse.String(" clay")
            select clay
            ).Optional()
        from obsidian in (
            from _5 in Parse.String(" and ")
            from obisidian in Int
            from _6 in Parse.String(" obsidian")
            select obisidian
        ).Optional()
        select new Cost
        {
            Ore = ore,
            Clay = clay.GetOrElse(0),
            Obsidian = obsidian.GetOrElse(0)
        };

    public static readonly Parser<BluePrint> BluePrint =
        from _1 in Parse.String("Blueprint ")
        from id in Int
        from _2 in Parse.String(": Each ore robot ")
        from oreCost in Cost
        from _3 in Parse.String(". Each clay robot ")
        from clayCost in Cost
        from _4 in Parse.String(". Each obsidian robot ")
        from obsidianCost in Cost
        from _5 in Parse.String(". Each geode robot ")
        from geodeCost in Cost
        from _6 in Parse.String(".")
        select new BluePrint
        {
            Id = id,
            OreRobot = oreCost,
            ClayRobot = clayCost,
            ObsidianRobot = obsidianCost,
            GeodeRobot = geodeCost
        };

    public static BluePrint FromText(string text) => BluePrint.Parse(text);


}