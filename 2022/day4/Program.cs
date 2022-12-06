using Day4;
using Utils;

// Parse input:
StreamReader reader = File.OpenText("input.txt");

// Part 1:
var result = reader.GetLines()
    .Select(SectionInterval.ParsePair)
    .Where(pair => pair.lhs | pair.rhs)
    .Count();

Console.WriteLine($"Part 1: {result}");

// Part 2:
reader.DiscardBufferedData();
reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
var result2 = reader.GetLines()
    .Select(SectionInterval.ParsePair)
    .Where(pair => SectionInterval.Overlap(pair.lhs, pair.rhs))
    .Count();

Console.WriteLine($"Part 2: {result2}");