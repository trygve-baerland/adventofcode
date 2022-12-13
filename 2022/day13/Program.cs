using System.Diagnostics;
using Utils;
using Day13;

var sw = new Stopwatch();
// Parse input
sw.Start();
var allTrees = File.OpenText("input.txt")
    .GetLines()
    .Where(line => line != "")
    .Select(TreeHelpers.FromString)
    .ToList();

Console.WriteLine($"Parsing took {sw.ElapsedMilliseconds} ms");

// Part 1:
sw.Restart();
var part1 = allTrees
    .Clump(2)
    .Select((pair, index) => (pair.First().IsSorted(pair.Skip(1).First()), index))
    .Where(item => item.Item1 != Sorted.False)
    .Select(item => item.index + 1)
    .Sum();

Console.WriteLine($"Part 1: {part1} [{sw.ElapsedMilliseconds}]");

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