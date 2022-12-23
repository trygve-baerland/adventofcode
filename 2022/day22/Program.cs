using Utils;
using Day22;

// Parse input:
var lines = File.OpenText("input.txt")
    .GetLines()
    .ToList();

// Get path from last line:
var path = TracingParser.FromText(lines.Last());

// Get board:
var board = new Board
{
    Map = lines.Take(lines.Count - 2)
        .Select(lines => lines.ToCharArray())
        .ToArray()
};

// Set up starting position
var turtle = new Turtle
{
    Position = new Node<int>
    {
        X = board.GetValidCols(0).Where(item => item.Item1 == '.').First().Item2,
        Y = 0
    },
    MovingDirection = Direction.R
};

//Console.WriteLine(string.Join(", ", Enumerable.Range(0, 50).Reverse()));
// Make moves:
foreach (var (numSteps, rot) in path.Item1)
{
    turtle.MoveOnBoard(board, numSteps, onCube: true);
    turtle.Rotate(rot);
}
turtle.MoveOnBoard(board, path.Item2);
Console.WriteLine($"Final position of turtle: {turtle}");
Console.WriteLine($"Part 1: {turtle.EvaluatePosition()}");