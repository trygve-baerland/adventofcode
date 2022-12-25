using Utils;

namespace Day25;

public class Snafu
{
    public IEnumerable<char> Digits { get; set; } = new List<char>();

    public Snafu(string text)
    {
        Digits = text.ToList();
    }

    public long ToInt() => Digits
        .Reverse()
        .Aggregate<char, (long total, long denom), long>(
            seed: (total: 0, denom: 1),
            func: (acc, item) => (checked(acc.total + CharValue(item) * acc.denom), checked(acc.denom * 5)),
            resultSelector: item => item.total
        );

    public static long CharValue(char c) => c switch
    {
        '=' => -2,
        '-' => -1,
        '0' => 0,
        '1' => 1,
        '2' => 2,
        _ => throw new Exception($"'{c}' is not a SNAFU digit.")
    };

    public static Snafu FromInt(long value)
    {
        // Initialize:
        List<char> digits = new();
        // Get number of digits:
        while (value != 0)
        {
            long mod = value % 5; // 0,1,2,3,4
            // Get snafu digit:
            long digit = mod switch
            {
                <= 2 => mod,
                _ => mod - 5
            };
            value -= digit; // Number should now be congruent to 0 mod 5
            value /= 5;
            // Add digit:
            digits.Add(ToChar(digit));
        }
        digits.Reverse();
        return new Snafu(string.Join("", digits));
    }

    public override string ToString()
    {
        return string.Join("", Digits);
    }

    public static char ToChar(long value) => value switch
    {
        -2 => '=',
        -1 => '-',
        0 => '0',
        1 => '1',
        2 => '2',
        _ => throw new Exception($"{value} cannot be converted to a snafu digit.")
    };
}