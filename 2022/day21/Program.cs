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

// Part 2:
// Get trace to "humn":
var root = ShoutingMonkey.AllMonkeys["root"];
var trace = root.Find("humn");

Console.WriteLine(string.Join(", ", trace));

// Main loop:
var monkey = root;
monkey.Operation = Op.Compare;
decimal target = 0;
foreach (var dir in trace)
{
    // Invert current monkey:
    target = monkey.Invert(dir, target);
    monkey = monkey.GetChild(dir);
}
Console.WriteLine(monkey);
Console.WriteLine($"Part 2: {target}");
