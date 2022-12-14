using System.Diagnostics;
using Day14;
using Utils;
using Utils.Graphs;

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

// Let's do the exercise with DFS (recursive):
sw.Restart();
counter = 0;
cave = new(rockLines, withFloor: false);
GraphSearch.DFSRecursive(
    source: source,
    adjacentNodes: cave.GetNextNodes,
    postVisitor: node =>
    {
        cave.Blockers.Add(node);
        //cave.ColorNode(node, 'o');
        if (Cave.DropDirections
            .Select(dir => node + dir)
            .Where(node => cave.NotBlocked(node) == BlockedEnum.Abyss)
            .Any())
        {
            return true;
        }
        counter++;
        return false;
    }
);
//cave.Print();
Console.WriteLine($"Part 1 (DFS recursive): {counter} [{sw.ElapsedMilliseconds}]");
// Part 2:
sw.Restart();
counter = 0;
cave = new(rockLines, withFloor: true);
GraphSearch.DFSRecursive(
    source: source,
    adjacentNodes: cave.GetNextNodes,
    postVisitor: node =>
    {
        //cave.Blockers.Add(node);
        //cave.ColorNode(node, 'o');
        counter++;
        return false;
    }
);
//cave.Print();
Console.WriteLine($"Part 2 (DFS recursive): {counter} [{sw.ElapsedMilliseconds}]");

// Let's do the exercise with DFS (iterative):
sw.Restart();
counter = 0;
cave = new(rockLines, withFloor: false);
GraphSearch.DFSIterative(
    source: source,
    adjacentNodes: cave.GetNextNodes,
    postVisitor: node =>
    {
        cave.Blockers.Add(node);
        //cave.ColorNode(node, 'o');
        if (Cave.DropDirections
            .Select(dir => node + dir)
            .Where(node => cave.NotBlocked(node) == BlockedEnum.Abyss)
            .Any())
        {
            return true;
        }
        counter++;
        return false;
    }
);
//cave.Print();
Console.WriteLine($"Part 1 (DFS iterative): {counter} [{sw.ElapsedMilliseconds}]");
// Part 2:
sw.Restart();
counter = 0;
cave = new(rockLines, withFloor: true);
GraphSearch.DFSIterative(
    source: source,
    adjacentNodes: cave.GetNextNodes,
    postVisitor: node =>
    {
        //cave.Blockers.Add(node);
        //cave.ColorNode(node, 'o');
        counter++;
        return false;
    }
);
//cave.Print();
Console.WriteLine($"Part 2 (DFS iterative): {counter} [{sw.ElapsedMilliseconds}]");