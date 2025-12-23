using CommandLine;
namespace AoC;

[Flags]
public enum Part : ushort
{
    Part1 = 1,
    Part2 = 2
}
public class CommandLineOptions
{
    [Option( 'd', "day", Required = true, HelpText = "Hvilken dag du ønsker å løse" )]
    public int Day { get; set; }

    [Option( 'y', "year", Required = false, HelpText = "Hvilket år du ønsker å løse" )]
    public int Year { get; set; } = 2025;

    [Option( 'p', "part", Required = false, HelpText = "Hvilken del du ønsker å løse" )]
    public Part Del { get; set; } = Part.Part1 | Part.Part2;
}
