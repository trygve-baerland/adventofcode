using Day10;
using Utils;
// Read input
var lines = File.OpenText("input.txt")
    .GetLines();

VideoOperator videoOperator = new(1, 0);
List<int> interestingCycles = new() { 20, 60, 100, 140, 180, 220 };
int sum = 0;

lines.ForEach(
    line => videoOperator.DoCommand(line, arg =>
    {
        if (interestingCycles.Contains(arg.Cycle))
        {
            sum += arg.SignalStrength;
        }
    })
);

// Part 1:
Console.WriteLine($"Part 1: {sum}");

// Part 2:
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Part 2:");
string printOut = "";
VideoOperator videoOperator2 = new(1, 0);
File.OpenText("input.txt")
    .GetLines()
    .ForEach(
        line => videoOperator2.DoCommand(line, arg =>
        {
            var pos = (arg.Cycle - 1) % 40;
            // Get position on line:
            if (Math.Abs(arg.X - pos) <= 1)
            {
                printOut += "\u258A";
            }
            else
            {
                printOut += ".";
            }
            // Print out line if we're at end of line:
            if (arg.Cycle % 40 == 0)
            {
                Console.WriteLine(printOut);
                printOut = "";
            }
        })
    );