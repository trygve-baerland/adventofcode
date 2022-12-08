using Utils;
// Read input:
var array = File.OpenText("input.txt")
    .GetLines()
    .Select(line => line.ToCharArray())
    .ToArray();


int visibleTrees = 0;
int maxScenic = 0;
for (var row = 0; row < array.Count(); row++)
{
    var Nrow = array[row].Count();
    for (var col = 0; col < Nrow; col++)
    {
        if (Helpers.Visible(array, row, col)) visibleTrees++;

        var scenic = Helpers.ScenicScore(array, row, col);
        if (scenic > maxScenic) maxScenic = scenic;
    }
}

Console.WriteLine($"Part 1: {visibleTrees}");
Console.WriteLine($"Part 2: {maxScenic}");
class Helpers
{
    public static bool VisibleFromRight(char[][] arr, int row, int col)
    {
        var value = arr[row][col];
        return !arr[row].Select((v, i) => (v, i))
            .Where(item => item.i > col && item.v >= value)
            .Any();
    }

    public static bool VisibleFromLeft(char[][] arr, int row, int col)
    {
        var value = arr[row][col];
        return !arr[row].Select((v, i) => (v, i))
            .Where(item => item.i < col && item.v >= value)
            .Any();
    }

    public static bool VisibleFromAbove(char[][] arr, int row, int col)
    {
        // Get column array:
        var colArr = arr.Select(row => row[col]).ToArray();

        var value = arr[row][col];
        return !colArr.Select((v, i) => (v, i))
            .Where(item => item.i < row && item.v >= value)
            .Any();
    }

    public static bool VisibleFromBelow(char[][] arr, int row, int col)
    {
        // Get column array:
        var colArr = arr.Select(row => row[col]).ToArray();

        var value = arr[row][col];
        return !colArr.Select((v, i) => (v, i))
            .Where(item => item.i > row && item.v >= value)
            .Any();

    }

    public static bool Visible(char[][] arr, int row, int col)
    {
        return VisibleFromRight(arr, row, col) ||
            VisibleFromLeft(arr, row, col) ||
            VisibleFromAbove(arr, row, col) ||
            VisibleFromBelow(arr, row, col);
    }

    public static int Scenic(IEnumerable<char> sightLine, char height)
    {
        int visibleTrees = 0;

        foreach (var (tree, i) in sightLine.Select((v, i) => (v, i)))
        {
            visibleTrees++;
            if (tree >= height)
            {
                return visibleTrees;
            }
        }
        return visibleTrees;
    }

    public static IEnumerable<char> GetRightSightLine(char[][] arr, int row, int col)
    {
        return arr[row].Select((v, i) => (v, i))
            .Where(item => item.i > col)
            .Select(item => item.v);
    }

    public static IEnumerable<char> GetLeftSightLine(char[][] arr, int row, int col)
    {
        return arr[row].Select((v, i) => (v, i))
            .Where(item => item.i < col)
            .Select(item => item.v)
            .Reverse();
    }

    public static IEnumerable<char> GetUpSightLine(char[][] arr, int row, int col)
    {
        return arr.Select((v, i) => (v[col], i))
            .Where(item => item.i < row)
            .Select(item => item.Item1)
            .Reverse();
    }

    public static IEnumerable<char> GetDownSightLine(char[][] arr, int row, int col)
    {
        return arr.Select((v, i) => (v[col], i))
            .Where(item => item.i > row)
            .Select(item => item.Item1);
    }

    public static int ScenicScore(char[][] arr, int row, int col)
    {
        return Scenic(GetRightSightLine(arr, row, col), arr[row][col])
            * Scenic(GetLeftSightLine(arr, row, col), arr[row][col])
            * Scenic(GetUpSightLine(arr, row, col), arr[row][col])
            * Scenic(GetDownSightLine(arr, row, col), arr[row][col]);
    }


}