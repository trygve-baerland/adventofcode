using Utils;
using Day11;
// Create monkeys
Dictionary<int, Monkey> monkeys = new()
{
    {
        0,
        new Monkey
        {
            Id = 0,
            Worries = new Queue<ulong>(new ulong[] { 63, 57 }),
            Operation = i => checked(i * 11),
            Condition = i => i % 7 == 0
        }
    },

    {
        1,
        new Monkey
        {
            Id = 1,
            Worries = new Queue<ulong>(new ulong[] { 82, 66, 87, 78, 77, 92, 83 }),
            Operation = i => checked(i + 1),
            Condition = i => i % 11 == 0
        }
    },

    {
        2,
        new Monkey
        {
            Id = 2,
            Worries = new Queue<ulong>(new ulong[] { 97, 53, 53, 85, 58, 54 }),
            Operation = i => checked(i * 7),
            Condition = i => i % 13 == 0
        }
    },

    {
        3,
        new Monkey
        {
            Id = 3,
            Worries = new Queue<ulong>(new ulong[] { 50 }),
            Operation = i => checked(i + 3),
            Condition = i => i % 3 == 0
        }
    },

    {
        4,
        new Monkey
        {
            Id = 4,
            Worries = new Queue<ulong>(new ulong[] { 64, 69, 52, 65, 73 }),
            Operation = i => checked(i + 6),
            Condition = i => i % 17 == 0
        }
    },

    {
        5,
        new Monkey
        {
            Id = 5,
            Worries = new Queue<ulong>(new ulong[] { 57, 91, 65 }),
            Operation = i => checked(i + 5),
            Condition = i => i % 2 == 0
        }
    },

    {
        6,
        new Monkey
        {
            Id = 6,
            Worries = new Queue<ulong>(new ulong[] { 67, 91, 84, 78, 60, 69, 99, 83 }),
            Operation = i => checked(i * i),
            Condition = i => i % 5 == 0
        }
    },

    {
        7,
        new Monkey
        {
            Id = 7,
            Worries = new Queue<ulong>(new ulong[] { 58, 78, 69, 65 }),
            Operation = i => checked(i + 7),
            Condition = i => i % 19 == 0
        }
    }
};

// Add connections:
monkeys[0].IfTrue = monkeys[6];
monkeys[0].IfFalse = monkeys[2];

monkeys[1].IfTrue = monkeys[5];
monkeys[1].IfFalse = monkeys[0];

monkeys[2].IfTrue = monkeys[4];
monkeys[2].IfFalse = monkeys[3];

monkeys[3].IfTrue = monkeys[1];
monkeys[3].IfFalse = monkeys[7];

monkeys[4].IfTrue = monkeys[3];
monkeys[4].IfFalse = monkeys[7];

monkeys[5].IfTrue = monkeys[0];
monkeys[5].IfFalse = monkeys[6];

monkeys[6].IfTrue = monkeys[2];
monkeys[6].IfFalse = monkeys[4];

monkeys[7].IfTrue = monkeys[5];
monkeys[7].IfFalse = monkeys[1];

// Finally, do some rounds:
int numRounds = 10000;
Enumerable.Range(0, numRounds).ForEach(arg =>
{
    Console.Write($"Round: {arg}\r");
    monkeys.ForEach(item => item.Value.DoRound());
});
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