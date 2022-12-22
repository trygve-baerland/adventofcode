using Utils;
using Day21;
// Parse input:
File.OpenText("input.txt")
    .GetLines()
    .ForEach(line =>
    {
        ShoutingMonkeyParser.FromText(line);
    });

// Part 1:
var part1 = ShoutingMonkey.AllMonkeys["root"].Value;
Console.WriteLine($"Part 1: {part1}");
