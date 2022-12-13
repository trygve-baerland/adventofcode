using Sprache;
using Utils;
using Day11;
// Create monkeys

var monkeys = MonkeyParserStuff.AllMonkeys.Parse(
    File.ReadAllText("input.txt")
);
// Finally, do some rounds:
int numRounds = 20; // 10000 for part 2
bool getCalmed = true; // false for part 2
Helpers.Repeat(arg =>
    {
        Console.Write($"Round: {arg}\r");
        monkeys.DoRound(getCalmed);
    },
    times: numRounds);
Console.Write("\n");
// Part 1:
var result = monkeys.Select(item => item.Value.Inspected)
    .OrderByDescending(val => val)
    .Take(2)
    .Aggregate((acc, item) => acc * item);

monkeys.ForEach(item =>
{
    Console.WriteLine($"Monkey {item.Key} has made {item.Value.Inspected} inspections");
});

Console.WriteLine($"Part 1: {result}");