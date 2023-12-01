using CommandLine;
namespace AoC;

public class CommandLineOptions
{
    [Option( 'd', "day", Required = true, HelpText = "Hvilken dag du ønsker å løse" )]
    public int Day { get; set; }
}
