using Utils;
using Day17;

// Parse input:
var movesList = TetrisParser.MovesFromText(File.OpenText("input.txt")
    .GetLines()
    .First());

Console.WriteLine($"Number of moves are {movesList.Count()}");

var moves = movesList
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
long maxCounter = 2022; //1000000000000;
while (counter < maxCounter && shapes.MoveNext())
{
    Console.Write($"{counter}\r");
    var shape = shapes.Current;
    board.InsertShape(shape, moves);
    counter++;
}
//board.Print();
Console.WriteLine($"Part 1: {board.Height}");