using Day20;
using Utils;
// Parse input:
long decryptionKey = 811589153;
int repetitions = 10;
var elements = File.OpenText("input.txt")
    .GetLines()
    .Select((line, id) => new Element
    {
        Value = long.Parse(line) * decryptionKey,
        Position = id,
        InitialPosition = id
    })
    .ToList();

// Perform mixing:
var mixed = Decryption.MixDecrypt(elements, repetitions: repetitions);

if (mixed.Select(item => item.Position).Distinct().Count() < mixed.Count)
{
    throw new Exception($"Something went wrong when updating positions...");
}
// Part 1:
// Get position of 0:
var pos0 = mixed.Where(element => element.Value == 0).First().Position;
var coordPositions = new List<int> { 1000, 2000, 3000 };

var part1 = coordPositions
    .Select(number => Decryption.GetValueInPosition(mixed, number + pos0))
    .Sum();

Console.WriteLine($"Part 1: {part1}");