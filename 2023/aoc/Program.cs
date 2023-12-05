using AoC;
using CommandLine;

Console.WriteLine( "Welcome to my AoC 2023 solutions!" );

// Get all available solutions:
var solutions = System.Reflection.Assembly.GetExecutingAssembly()
  .GetTypes()
  .Where( mytype => mytype.GetInterfaces().Contains( typeof( IPuzzle ) ) )
  .ToDictionary( mytype => mytype.Name, mytype => mytype );

Parser.Default.ParseArguments<CommandLineOptions>( args )
  .WithParsed( o => {
      var solution = ( IPuzzle ) Activator.CreateInstance( solutions[$"Day{o.Day}"] )!;
      Console.WriteLine( "Part 1:" );
      solution.Part1();
      Console.WriteLine( "===========================" );
      Console.WriteLine( "Part 2:" );
      solution.Part2();
      Console.WriteLine( "===========================" );
  } )
  .WithNotParsed( errs => {
      Console.WriteLine( "You stupid piece of shit." );
      Console.WriteLine( "Tilgjengelige løsninger er:" );
      foreach ( var name in solutions.Keys )
      {
          Console.WriteLine( $"\t{name}" );
      }
  } );
