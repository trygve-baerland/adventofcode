using Day12;
using Utils;
// Read height map:
var array = File.OpenText("input.txt")
    .GetLines()
    .Select(line => line.ToCharArray())
    .ToArray();

// Find source and target:
var S = BFS.IndexOf(array, 'S');
var E = BFS.IndexOf(array, 'E');

Console.WriteLine($"'S' is at ({S.row}, {S.col})");
Console.WriteLine($"'E' is at ({E.row}, {E.col})");

// Part 1:
var result = BFS.ShortestPath(array, S, E);
// Print distances:
Console.WriteLine($"Part 1: {result}");

// Part 2:

// Now we need to calculate all distance to 'E' where starting point is 'a':
var result2 = BFS.GetIndices(array)
    .Where(idx => array[idx.row][idx.col] == 'a')
    .Select(idx => BFS.ShortestPath(array, idx, E))
    .Min();

Console.WriteLine($"Part 2: {result2}");