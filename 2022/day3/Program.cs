using Day3;

// Parse input:
StreamReader reader = File.OpenText("input.txt");

// Part1:
var result = reader.GetLines() // Read all lines as enumerable
    .Select(Helpers.SplitLine) // Split line in half.
    .Select(tup => tup.lhs.Intersect(tup.rhs).First()) // Get First common element
    .Select(Helpers.Score)
    .Sum();

Console.WriteLine($"Part 1: {result}");

// Part 2:
StreamReader newReader = File.OpenText("input.txt");
result = newReader.GetLines() // Read all lines as enumerable
    .Clump(3)
    .Select(Helpers.GetCommonLetter)
    .Select(Helpers.Score)
    .Sum();

Console.WriteLine($"Part 2: {result}");

