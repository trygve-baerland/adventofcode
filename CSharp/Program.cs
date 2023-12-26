using System.Diagnostics;
using AoC;
using CommandLine;

Console.WriteLine( "Velkommen til Trygves AoC løsninger i C#!" );

// Tilgjengelige løsninger
var solutions = System.Reflection.Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where( mytype => mytype.GetInterfaces().Contains( typeof( IPuzzle ) ) )
    .GroupBy( mytype => mytype.Namespace! )
    .ToDictionary(
        g => g.Key,
        g => g.ToDictionary( item => item.Name, item => item )
    );

Parser.Default.ParseArguments<CommandLineOptions>( args )
    .WithParsed( o => {
        var solutionsOfYear = solutions[$"AoC.Y{o.Year}"] ?? throw new ArgumentException( $"År {o.Year} er ikke gyldig" );
        var dayType = solutionsOfYear[$"Day{o.Day}"] ?? throw new ArgumentException( $"År {o.Year}, dag {o.Day} er ikke gyldig" );
        var solution = ( IPuzzle ) Activator.CreateInstance( dayType )!;
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
        foreach ( var year in solutions.Keys )
        {
            Console.WriteLine( $"{year}:" );
            foreach ( var day in solutions[year].Keys )
            {
                Console.WriteLine( $"\t{day}" );
            }
        }
    } );
