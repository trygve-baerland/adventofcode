using Day14;
using Utils;

// Parse/prepare input:
var rockLines = File.OpenText("input.txt")
    .GetLines()
    .Select(Parser.GetRockLine)
    .Select(rockLine => rockLine.ToList())
    .ToList();

Cave cave = new(rockLines, withFloor: true);

// Part 1:
var source = new Node { X = 500, Y = 0 };
int counter = 0;
while (cave.DropSand(source) != BlockedEnum.Abyss)
{
    counter++;
}
cave.Print();
Console.WriteLine($"Part 1: {counter}");