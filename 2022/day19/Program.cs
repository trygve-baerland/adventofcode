using Utils;
using Day19;

var blueprints = File.OpenText("input.txt")
    .GetLines()
    .Select(BlueprintParser.FromText)
    .ToList();


// Part 1:
var part1 = blueprints
    .Select(bp => (bp.Id, Scheduling.MostGeodes(bp, 24 + 1, new SearchState
    {
        Ore = bp.OreRobot.Ore,
        NextConstruction = Construction.OreRobot
    })))
    .Select(item => item.Id * item.Item2)
    .Sum();

Console.WriteLine($"Part 1: {part1}");

// Part2:
var part2 = blueprints.Take(3)
    .Select(bp => Scheduling.MostGeodes(bp, 32 + 1, new SearchState
    {
        Ore = bp.OreRobot.Ore,
        NextConstruction = Construction.OreRobot
    }))
    .Aggregate((acc, item) => acc * item);

Console.WriteLine($"Part 2: {part2}");