using Utils;
using Utils.Graphs;
using Day18;

// Parse input:
var points = File.OpenText("input.txt")
    .GetLines()
    .Select(LavaParser.FromText)
    .ToList();


// Part 1:
var clump = new LavaClump();

foreach (var point in points)
{
    clump.AddPoint(point);
}

Console.WriteLine($"Part 1: {clump.FreeSides}");

// Part 2:
var outside = new OutsideLavaClump(clump);
outside.FillOutside();
Console.WriteLine($"Part 2: {outside.FreeSides}");