using AoC.Utils;

namespace AoC.Y2024;

public sealed class Day9 : IPuzzle
{

    private readonly List<int> TestDisk = [.. "2024/inputdata/day9_test.txt".GetText().Select( c => c - '0' )];

    private readonly List<int> ProdDisk = [.. "2024/inputdata/day9.txt".GetText().Select( c => c - '0' )];
    public void Part1()
    {
        var disk = TestDisk;
        var files = disk.Select( ( s, i ) => (s, i) )
            .Where( tup => tup.i % 2 == 0 )
            .Select( tup => new File { Id = tup.i / 2, Size = tup.s } )
            .ToList();

        var spaces = disk.Select( ( s, i ) => (s, i) )
            .Where( tup => tup.i % 2 == 1 )
            .Select( tup => tup.s )
            .ToList();
    }

    public void Part2() { }
}

internal record struct File
{
    public int Id { get; init; }
    public int Size { get; set; }
}

internal record struct ChecksumCalcState
{
    public long Value { get; set; }
    public bool OnFile { get; set; }
}
