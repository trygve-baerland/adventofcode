using System.Diagnostics;
using Day16;
using Utils;
using Utils.Graphs;

// Parse input:
var sw = new Stopwatch();
sw.Start();
var valves = File.OpenText("input.txt")
    .GetLines()
    .Select(TunnelParsing.ParseValve)
    .ToList();

var valveDict = valves.ToDictionary(
    valve => valve.Name,
    valve => valve
);

Console.WriteLine($"Parsing took {sw.ElapsedMilliseconds} ms");
/*
foreach (var valve in valves)
{
    Console.WriteLine(valve.ToString());
}
*/

// We need the adjacency mapping:
sw.Restart();
var shortestPaths = ShortestPath.FloydWarshall(valves,
    valve => valve.ConnectedValves.Select(name => valveDict[name]));
Console.WriteLine($"Creating shortest path mapping took {sw.ElapsedMilliseconds} ms");

// Print shortest paths:
/*
foreach (var i in Enumerable.Range(0, shortestPaths.Size))
{
    Console.Write("[");
    foreach (var j in Enumerable.Range(0, shortestPaths.Size))
    {
        Console.Write($"{shortestPaths.Distances[i, j]}, ");
    }
    Console.WriteLine("]");
}
foreach (var item in shortestPaths.Index.Mapping)
{
    Console.WriteLine($"{item.Key.Name}: {item.Value} -> {shortestPaths.Index.FromIndex(item.Value).Name}");
}
*/
// We'll attempt solving it with DFS
sw.Restart();
var path = TunnelHelpers.OptimalRelease(
    source: valveDict["AA"],
    shortestPathMapping: shortestPaths,
    timeout: 30
);
var part1 = TunnelHelpers.EvaluatePath(path, 30);
Console.WriteLine($"Part 1: {part1} [{sw.ElapsedMilliseconds}]");
foreach (var (valve, time) in path)
{
    Console.Write($"{valve.Name}[{time}], ");
}
Console.WriteLine("\n-------");
// Part 2:
sw.Restart();
var part2 = TunnelHelpers.OptimalElephantRelease(
    source: valveDict["AA"],
    shortestPathMapping: shortestPaths,
    timeout: 26
);
Console.WriteLine($"Part 2: {part2} [{sw.ElapsedMilliseconds}]");