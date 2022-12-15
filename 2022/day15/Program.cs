using Utils;
using Day15;

// Parse input
var pairs = File.OpenText("input.txt")
    .GetLines()
    .Select(Parsers.FromText)
    .Select(item => (item.sensor, item.beacon, dist: Day15Helpers.L1Dist(item.sensor, item.beacon)))
    .ToList();

var sensors = pairs.Select(item => (item.sensor, item.dist)).ToList();

//var beacons = pairs.Select(item => item.beacon).Distinct().ToList();
var beacons = new HashSet<Node>();
foreach (var (sensor, beacon, dist) in pairs)
{
    beacons.Add(beacon);
    beacons.Add(sensor);
}

foreach (var (sensor, beacon, dist) in pairs)
{
    Console.WriteLine($"S:{sensor}, B: {beacon}, dist: {dist}");
}

// Part 1:
int row = 2000000;
var result = Day15Helpers.FeasibleNodes(sensors, row, beacons)
    .Distinct()
    .Where(node => Day15Helpers.CanNotHaveBeacon(node, sensors))
    .Count();

Console.WriteLine($"Part 1: {result}");

// Part 2:
int maxCoord = 4000000;

Day15Helpers.SearchSignal((0, maxCoord), sensors, beacons)
    .ForEach(node => { Console.WriteLine($"Possible candidate: {node}: {Day15Helpers.TuningFrequency(node)}"); });

