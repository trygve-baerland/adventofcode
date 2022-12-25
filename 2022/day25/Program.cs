using Utils;
using Day25;

// Part 1:
var part1 = File.OpenText("input.txt")
    .GetLines()
    .Select(line => new Snafu(line).ToInt())
    .Sum();

Console.WriteLine($"Part 1: {part1} or {Snafu.FromInt(part1)}");
