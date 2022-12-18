using Utils;
using Day18;

// Parse input:
var points = File.OpenText("test_input.txt")
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

// Look at bounding box:
Console.WriteLine($"X: [{clump.Points.Select(point => point.X).Min()},{clump.Points.Select(point => point.X).Max()}]");
Console.WriteLine($"Y: [{clump.Points.Select(point => point.Y).Min()},{clump.Points.Select(point => point.Y).Max()}]");
Console.WriteLine($"Z: [{clump.Points.Select(point => point.Z).Min()},{clump.Points.Select(point => point.Z).Max()}]");

var wX = points.Select(point => point.X).Max() - points.Select(point => point.X).Min();
var wY = points.Select(point => point.Y).Max() - points.Select(point => point.Y).Min();
var wZ = points.Select(point => point.Z).Max() - points.Select(point => point.Z).Min();

Console.WriteLine($"Part 2: {2 * (wX * wY + wX * wZ + wY * wZ)}");