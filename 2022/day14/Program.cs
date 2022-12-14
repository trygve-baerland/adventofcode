using System.Diagnostics;
using Day14;
using Utils;

var sw = new Stopwatch();
// Parse/prepare input:
sw.Start();
var rockLines = File.OpenText("input.txt")
    .GetLines()
    .Select(Parser.GetRockLine)
    .Select(rockLine => rockLine.ToList())
    .ToList();

Console.WriteLine($"Parsing took {sw.ElapsedMilliseconds} ms");

// Part 1:
sw.Restart();
Cave cave = new(rockLines, withFloor: false);
var source = new Node { X = 500, Y = 0 };
int counter = 0;
while (cave.DropSand(source) != BlockedEnum.Abyss)
{
    counter++;
}
//cave.Print();
Console.WriteLine($"Part 1: {counter} [{sw.ElapsedMilliseconds}]");

// Part 2:
sw.Restart();
cave = new(rockLines, true);
counter = 0;
while (cave.DropSand(source) != BlockedEnum.Abyss)
{
    counter++;
}
//cave.Print();
Console.WriteLine($"Part 2: {counter} [{sw.ElapsedMilliseconds}]");

