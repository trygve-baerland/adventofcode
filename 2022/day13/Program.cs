using System.Diagnostics;
using Utils;
using Day13;

var sw = new Stopwatch();
// Parse input
sw.Start();
var enumerator = File.OpenText("input.txt")
    .GetLines()
    .GetEnumerator();

int counter = 1;
List<int> correctOrder = new();
List<Tree> allTrees = new();
while (enumerator.MoveNext())
{
    // Get pair of trees
    var lhs = TreeHelpers.FromString(enumerator.Current);
    enumerator.MoveNext();
    var rhs = TreeHelpers.FromString(enumerator.Current);
    var sorted = lhs.IsSorted(rhs);
    if (sorted == Sorted.True || sorted == Sorted.Continue)
    {
        correctOrder.Add(counter);
    }
    enumerator.MoveNext();
    counter++;

    allTrees.Add(lhs);
    allTrees.Add(rhs);
}

// Part 1:
Console.WriteLine($"Part 1: {correctOrder.Sum()} [{sw.ElapsedMilliseconds}]");

// Part 2;
sw.Restart();
// Add [[2]] and [[6]]
allTrees.Add(TreeHelpers.FromString("[[2]]"));
allTrees.Add(TreeHelpers.FromString("[[6]]"));

// Sort
allTrees.Sort((tree1, tree2) => (int)tree1.IsSorted(tree2));
// Find index of [[2]] and [[6]]
var valids = new string[] { "[[2]]", "[[6]]" };
var result = allTrees.Select((tree, index) => (tree, index))
    .Where(item => valids.Contains(item.tree.ToString()))
    .Select(item => item.index + 1)
    .Take(2)
    .Aggregate((acc, item) => acc * item);

Console.WriteLine($"Part 2: {result} [{sw.ElapsedMilliseconds}]");