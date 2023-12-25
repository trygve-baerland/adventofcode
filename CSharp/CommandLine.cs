using CommandLine;
namespace AoC;

public class CommandLineOptions
{
    [Option( 'd', "day", Required = true, HelpText = "Hvilken dag du ønsker å løse" )]
    public int Day { get; set; }

    [Option( 'y', "year", Required = false, HelpText = "Hvilket år du ønsker å løse" )]
    public int Year { get; set; } = 2023;
}
