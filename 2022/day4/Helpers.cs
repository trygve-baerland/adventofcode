
namespace Day4;

public class SectionInterval
{
    public int Start { get; set; }
    public int End { get; set; }

    public bool Contains(SectionInterval other)
    {
        return Start <= other.Start && End >= other.End;
    }

    public static bool operator <(SectionInterval lhs, SectionInterval rhs)
    {
        return rhs.Contains(lhs);
    }

    public static bool operator >(SectionInterval lhs, SectionInterval rhs)
    {
        return lhs.Contains(rhs);
    }

    public static bool operator |(SectionInterval lhs, SectionInterval rhs)
    {
        return lhs < rhs || rhs < lhs;
    }

    public static SectionInterval Parse(string line)
    {
        var numbers = line.Split("-");
        int start, end;
        if (!int.TryParse(numbers[0], out start))
        {
            throw new ArgumentException($"Unable to parse start number for {line}, {numbers[0]}");
        }
        if (!int.TryParse(numbers[1], out end))
        {
            throw new ArgumentException($"Unable to parse end number for {line}, {numbers[1]}");
        }
        return new SectionInterval
        {
            Start = start,
            End = end
        };
    }

    public static (SectionInterval lhs, SectionInterval rhs) ParsePair(string line)
    {
        var pair = line.Split(",");
        return (Parse(pair[0]), Parse(pair[1]));
    }

    public static bool Overlap(SectionInterval lhs, SectionInterval rhs)
    {
        return (lhs.Start <= rhs.End && rhs.Start <= lhs.End);
    }
}
