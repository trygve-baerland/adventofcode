using Utils;
using Day17;

// Parse input:
var moves = TetrisParser.MovesFromText(File.OpenText("input.txt")
    .GetLines()
    .First())
    .Repeat()
    .GetEnumerator();

var shapes = Shape.Shapes.Repeat().GetEnumerator();
// Initialize board:
var board = new Board()
{
    Width = 7
};

// Make some moves:
long counter = 0;
while (counter < 1000000000000 && shapes.MoveNext())
{
    Console.Write($"{counter}\r");
    var shape = shapes.Current;
    board.InsertShape(shape, moves);
    counter++;
}
//board.Print();
Console.WriteLine($"Part 1: {board.Height}");