using System.Text.RegularExpressions;
using Utils;

// Parse input:
StreamReader reader = File.OpenText("input.txt");
var lines = reader.GetLines();

// Parse initial stack:
string line = "Initial";
Stack<string> initStack = new();
var enumerator = lines.GetEnumerator();
while (line != "" && enumerator.MoveNext())
{
    line = enumerator.Current;
    initStack.Push(line);
}
// Pop last, empty line:
initStack.Pop();

// Parse to line to find indices of stacks (Why is this part of the fucking challenge?!)
var binsLine = initStack.Pop();
var binIndices = binsLine.ToCharArray()
    .Select((c, i) => new { C = c, Index = i })
    .Where(item => char.IsDigit(item.C))
    .Select(item => new { N = int.Parse(item.C.ToString()), item.Index })
    .ToDictionary(
        item => item.N,
        item => item.Index
    );

// Initialize stacks
var stacks = binIndices.ToDictionary(
    item => item.Key,
    item => new Stack<char>()
);

// Make initial stacks:
while (initStack.Count > 0)
{
    var toParse = initStack.Pop();
    // Go through each bin:
    foreach (var item in binIndices)
    {
        // Get character at current line and index:
        char c = toParse[item.Value];
        // If it's a letter, we put it on the corresponding stack
        if (char.IsAsciiLetter(c))
        {
            stacks[item.Key].Push(c);
        }
    }
}

// Move through each line in the remainder of the input file:
string pattern = @"move (\d+) from (\d+) to (\d+)";
var r = new Regex(pattern, RegexOptions.None);
foreach (var toParse in lines)
{
    var match = r.Match(toParse);
    int num = int.Parse(match.Groups[1].Value);
    int from = int.Parse(match.Groups[2].Value);
    int to = int.Parse(match.Groups[3].Value);
    // So do the move num times:
    var tempStack = new Stack<char>();
    for (int i = 0; i < num; i++)
    {
        tempStack.Push(stacks[from].Pop());
    }
    while (tempStack.Count > 0)
    {
        stacks[to].Push(tempStack.Pop());
    }
}

// Part 1:
foreach (var (key, stack) in stacks)
{
    Console.WriteLine($"{key}: {stack.Peek()}");
}



// Iterator now continues after initial block.
class Helpers
{
    public static void PrintStack(Stack<char> stack)
    {
        while (stack.Count > 0)
        {
            Console.WriteLine($"{stack.Pop()}, ");
        }
    }
}