using Utils;
using Day21;
// Parse input:
File.OpenText("input.txt")
    .GetLines()
    .ForEach(line =>
    {
        ShoutingMonkeyParser.FromText(line);
    });

var root = ShoutingMonkey.AllMonkeys["root"];
// Part 1:
var part1 = root.Value;
Console.WriteLine($"Part 1: {part1}");

// Part 2:
// Get trace to "humn":
var trace = root.Find("humn");
Console.WriteLine(string.Join(", ", trace));

root.Operation = Op.Compare;
var (monkey, part2) = trace.Aggregate(
    (root, (decimal)0),
    (acc, dir) => (acc.root.GetChild(dir), acc.root.Invert(dir, acc.Item2))
);
Console.WriteLine(monkey);
Console.WriteLine($"Part 2: {part2}");
