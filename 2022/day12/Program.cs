using System.Diagnostics;
using Day12;
using Utils;
using Utils.Graphs;
// Read height map:
var graph = Graph.FromFile("input.txt");

// Find source and target:
var S = graph.IndexOf('S');
var E = graph.IndexOf('E');

Console.WriteLine($"'S' is at {S}");
Console.WriteLine($"'E' is at {E}");

var sw = new Stopwatch();
// Part 1:
sw.Start();
var result = ShortestPath.BFS(S, E,
    adjacentNodes: v => Node.Directions
        .Select(dir => v + dir)
        .Where(w => graph.InGrid(w) && graph.CanTraverse(v, w))
);
// Print distances:
Console.WriteLine($"Part 1: {result} [{sw.ElapsedMilliseconds}]");

// Part 2:
sw.Restart();
// Now we need to calculate all distance to 'E' where starting point is 'a':
var result2 = graph.GetNodes()
    .Where(v => graph.GetHeight(v) == 'a')
    .Select(v => ShortestPath.BFS(v, E,
        vv => Node.Directions
            .Select(dir => vv + dir)
            .Where(w => graph.InGrid(w) && graph.CanTraverse(vv, w))
        )
    )
    .Min();

Console.WriteLine($"Part 2: {result2} [{sw.ElapsedMilliseconds}]");

// Part 2 with multiple sources:
sw.Restart();
var result22 = ShortestPath.BFS(
    sources: graph.GetNodes().Where(v => graph.GetHeight(v) == 'a'),
    target: E,
    adjacentNodes: v => Node.Directions
        .Select(dir => v + dir)
        .Where(w => graph.InGrid(w) && graph.CanTraverse(v, w))
);
Console.WriteLine($"Part 2 (altnernative approach): {result22} [{sw.ElapsedMilliseconds}]");

// Part 3 with multiple targets:
sw.Restart();
var result23 = ShortestPath.BFS(
    source: E,
    target: v => graph.GetHeight(v) == 'a',
    adjacentNodes: v => Node.Directions
        .Select(dir => v + dir)
        .Where(w => graph.InGrid(w) && graph.CanDescend(v, w))
);

Console.WriteLine($"Part 2 (starting from 'E'): {result23} [{sw.ElapsedMilliseconds}]");

// Now with Dijkstra:

// Part 1:
sw.Restart();
var result3 = ShortestPath.Dijkstra(S, E,
    v => Node.Directions
        .Select(dir => v + dir)
        .Where(w => graph.InGrid(w)),
    graph.EdgeWeight);

Console.WriteLine($"Dijkstra 1: {result3} [{sw.ElapsedMilliseconds}]");

// Part 2:
sw.Restart();
var result4 = graph.GetNodes()
    .Where(v => graph.GetHeight(v) == 'a')
    .Select(v => ShortestPath.Dijkstra(v, E,
        vv => Node.Directions
            .Select(dir => vv + dir)
            .Where(w => graph.InGrid(w)),
        (vv, w) => graph.EdgeWeight(vv, w))
    )
    .Min();

Console.WriteLine($"Dijkstra 2: {result4} [{sw.ElapsedMilliseconds}]");