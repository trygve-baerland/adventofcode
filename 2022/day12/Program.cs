using Day12;
using Utils;
// Read height map:
var graph = Graph.FromFile("input.txt");

// Find source and target:
var S = graph.IndexOf('S');
var E = graph.IndexOf('E');

Console.WriteLine($"'S' is at {S}");
Console.WriteLine($"'E' is at {E}");

// Part 1:
var result = graph.BFS(S, E,
    adjacentNodes: v => BFS.AdjacentNodes(graph, v, false));
// Print distances:
Console.WriteLine($"Part 1: {result}");

// Part 2:

// Now we need to calculate all distance to 'E' where starting point is 'a':
var result2 = graph.GetNodes()
    .Where(v => graph.GetHeight(v) == 'a')
    .Select(v => graph.BFS(v, E,
        vv => BFS.AdjacentNodes(graph, vv, false))
    )
    .Min();

Console.WriteLine($"Part 2: {result2}");

// Now with Dijkstra:

// Part 1:
var result3 = graph.Dijkstra(S, E,
    v => BFS.AdjacentNodes(graph, v, true),
    (v, w) => BFS.EdgeWeight(graph, v, w));
Console.WriteLine($"Dijkstra 1: {result3}");

// Part 2:
var result4 = graph.GetNodes()
    .Where(v => graph.GetHeight(v) == 'a')
    .Select(v => graph.Dijkstra(v, E,
        vv => BFS.AdjacentNodes(graph, vv, true),
        (vv, w) => BFS.EdgeWeight(graph, vv, w))
    )
    .Min();

Console.WriteLine($"Dijkstra 2: {result4}");