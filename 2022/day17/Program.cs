using Utils;
using Day17;

// Parse input:
var movesList = TetrisParser.MovesFromText(File.OpenText("input.txt")
    .GetLines()
    .First());

Console.WriteLine($"Number of moves are {movesList.Count()}");

var moves = movesList
    .Select((move, index) => (move, index))
    .Repeat()
    .GetEnumerator();

var shapes = Shape.Shapes
    .Select((shape, index) => (shape, index))
    .Repeat()
    .GetEnumerator();
// Initialize board:
var board = new Board()
{
    Width = 7
};

// Make some moves:
long counter = 0;
long maxCounter = 1000000000000;
var dHeights = new List<long>();
var jetIndexSet = new HashSet<int>();
int prevJetIndex = 0;
int? repeatJetIndex = null;

int bottomRockCount = 0;
int repeatRockCount = 0;
while (counter < maxCounter && shapes.MoveNext())
{
    Console.Write($"{counter}\r");
    var (shape, shapeIndex) = shapes.Current;
    var (dHeight, jetIndex) = board.InsertShape(shape, moves);
    dHeights.Add(dHeight);
    counter++;
    // Find where we have looped around next shape is a horizontal line:
    if (jetIndex < prevJetIndex && ((shapeIndex + 1) % 5 == 0))
    {
        if (repeatJetIndex is null)
        {
            if (jetIndexSet.Contains(jetIndex))
            {
                bottomRockCount = dHeights.Count;
                repeatJetIndex = jetIndex;
            }
            else
            {
                jetIndexSet.Add(jetIndex);
            }
        }
        else if (jetIndex == repeatJetIndex)
        {
            repeatRockCount = dHeights.Count - bottomRockCount;
            break;
        }
    }
    prevJetIndex = jetIndex;
}
Console.WriteLine($"Bottom Rock Count: {bottomRockCount}");
Console.WriteLine($"Bottom Height: {dHeights.Take(bottomRockCount).Sum()}");

var period = dHeights.Skip(bottomRockCount).ToList();
Console.WriteLine($"Repeat Rock Count: {repeatRockCount}");
Console.WriteLine($"Altnernative repeatHeight: {period.Sum()}");

var numFullRepeats = (maxCounter - bottomRockCount) / repeatRockCount;
int numLastPart = (int)((maxCounter - bottomRockCount) % repeatRockCount);
var bottomHeight = dHeights.Take(bottomRockCount).Sum();
long height = bottomHeight + numFullRepeats * period.Sum() + period.Take(numLastPart).Sum();
Console.WriteLine($"Part 2: {height}");