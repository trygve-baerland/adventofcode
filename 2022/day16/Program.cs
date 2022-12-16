using Day16;
using Utils;

// Parse input:
var valves = File.OpenText("input.txt")
    .GetLines()
    .Select(TunnelParsing.ParseValve);

foreach (var valve in valves)
{
    Console.WriteLine(valve.ToString());
}