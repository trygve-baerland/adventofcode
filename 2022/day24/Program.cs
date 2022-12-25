using Utils;
using Day24;

// Read input
var valley = new Valley(File.OpenText("input.txt").GetLines());

// Set start and end points:
var source = new Node<int>
{
    X = 1,
    Y = 0
};
var target = new Node<int>
{
    X = valley.Width - 2,
    Y = valley.Height - 1
};

//valley.PrintValley();
// Part 1:
var part1 = valley.FindShortestPath(source, target);
Console.WriteLine("");
Console.WriteLine($"Part 1: {part1}");

// Part 2:
var temp = valley.FindShortestPath(target, source, startTime: part1);
var part2 = valley.FindShortestPath(source, target, startTime: temp);

Console.WriteLine($"Part 2: {part2}");
