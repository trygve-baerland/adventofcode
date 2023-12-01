using System.Text.RegularExpressions;
using Utils;

namespace AoC;

public sealed class Day1 : IPuzzle
{
  public IEnumerable<string> TestData => "inputdata/day1_test.txt".GetLines();
  public IEnumerable<string> ActualData => "inputdata/day1.txt".GetLines();


  public Day1()
  {
  }
  public void Part1()
  {
    var result = ActualData
      .Select(l => GetNumber(l, new Regex(@"\d"), new Regex(@"\d")))
      .Sum();
    Console.WriteLine($"Result: {result}");
  }

  public void Part2()
  {
    var result = ActualData
      .Select(l => GetNumber(l, Matcher, LastMatcher))
      .Sum();
    Console.WriteLine($"Result: {result}");
  }

  private Regex Matcher
  { get; } = new(@"\d|one|two|three|four|five|six|seven|eight|nine");
  private Regex LastMatcher { get; } = new(@"\d|one|two|three|four|five|six|seven|eight|nine", RegexOptions.RightToLeft);

  private int GetNumber(string line, Regex firstRegex, Regex lastRegex)
  {
    var first = ConvertDigit(firstRegex.Match(line).Value);
    var last = ConvertDigit(lastRegex.Match(line).Value);
    var number = 10 * first + last;
    return number;
  }

  private int ConvertDigit(string number)
  {
    try { return int.Parse(number); }
    catch (Exception)
    {
      return number switch
      {
        "one" => 1,
        "two" => 2,
        "three" => 3,
        "four" => 4,
        "five" => 5,
        "six" => 6,
        "seven" => 7,
        "eight" => 8,
        "nine" => 9,
        _ => throw new ArgumentException($"Invalid number {number}")
      };
    }
  }

}