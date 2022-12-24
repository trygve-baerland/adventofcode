using Utils;
using Day23;

// Read input:
var elves = File.OpenText("input.txt")
    .GetLines()
    .Select((line, y) =>
        line.Select((c, x) => (c, x))
            .Where(item => item.c == '#')
            .Select(item => new Node<int>
            {
                X = item.x,
                Y = y
            })
        )
    .Aggregate((acc, item) => acc.Chain(item))
    .ToList();

Console.WriteLine($"We have parsed {elves.Count} elves.");

// Do some rounds:
int counter = 0;
bool movesMade = true;
while (movesMade)
{
    Console.WriteLine($"Round {counter}");
    (elves, movesMade) = SpreadingOut.DoRound(elves);
    counter++;
}
var part1 = SpreadingOut.EvaluatePosition(elves);
Console.WriteLine($"Part 1: {part1}");
Console.WriteLine($"Part 2: {counter}");