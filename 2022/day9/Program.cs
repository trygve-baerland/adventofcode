using Day9;
using Utils;
// Read file:
var lines = File.OpenText("input.txt")
    .GetLines();

// Initialize stuff:
Rope rope = new(1);
rope.Visited.Add((0, 0));
// Go through each line:
foreach (var line in lines)
{
    // Get direction and number of times:
    var items = line.Split(" ");
    if (!Enum.TryParse(items[0], out Directions dir))
    {
        throw new Exception($"Unable to parse direction {items[0]}");
    };
    int num = int.Parse(items[1]);
    // Repeat moves a number of times:
    Enumerable.Range(0, num).ForEach(arg => rope.Move(dir));
}

// Part 1:
Console.WriteLine($"Part 1: {rope.Visited.Count}");

// Part 2:
Rope rope2 = new(9);
rope2.Visited.Add((0, 0));
// Go through each line:
lines = File.OpenText("input.txt")
    .GetLines();
foreach (var line in lines)
{
    // Get direction and number of times:
    var items = line.Split(" ");
    if (!Enum.TryParse(items[0], out Directions dir))
    {
        throw new Exception($"Unable to parse direction {items[0]}");
    };
    int num = int.Parse(items[1]);
    // Repeat moves a number of times:
    Enumerable.Range(0, num).ForEach(arg => rope2.Move(dir));
}

Console.WriteLine($"Part 2: {rope2.Visited.Count}");
