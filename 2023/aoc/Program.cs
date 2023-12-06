using System.Diagnostics;
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
      var sw = new Stopwatch();
      Console.WriteLine( "Part 1:" );
      sw.Start();
      solution.Part1();
      sw.Stop();
      Console.WriteLine( $"Finished in {sw.ElapsedMilliseconds} ms" );
      Console.WriteLine( "===========================" );
      Console.WriteLine( "Part 2:" );
      sw.Restart();
      solution.Part2();
      sw.Stop();
      Console.WriteLine( $"Finised in {sw.ElapsedMilliseconds} ms" );
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
