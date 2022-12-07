using Utils;
using Day7;

// Get lines in file as enumerator
var enumerator = File.OpenText("input.txt")
                    .GetLines()
                    .GetEnumerator();

// Parse into tree structure:
var tree = Tree.ParseFromLines(enumerator);

// Part 1:
// For all directories, get file size.
var smallDirs = new List<Tree>();
tree.Visit(node =>
{
    Console.WriteLine($"Size of {node.Name} is {node.Size}");
    if (node.Size <= 100000)
    {
        smallDirs.Add(node);
    }
});

Console.WriteLine($"Part 1: {smallDirs.Select(tree => tree.Size).Sum()}");

// Part 2:
int unused = 70000000 - tree.Size;
Console.WriteLine($"There is a total of {unused}");
int toBeFreed = 30000000 - unused;
Console.WriteLine($"We need to free up {toBeFreed}");
var canBeFreed = new List<int>();
tree.Visit(node =>
{
    if (node.Size >= toBeFreed)
    {
        canBeFreed.Add(node.Size);
    }
});

Console.WriteLine($"Part 2: {canBeFreed.Min()}");