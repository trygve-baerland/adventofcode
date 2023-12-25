using AoC.Utils;
using Sprache;
using System.Text;
namespace AoC.Y2023;


public sealed class Day15 : IPuzzle
{
    public List<List<char>> TestInstructions { get; } = "2023/inputdata/day15_test.txt".Stream().GetChars().Split( ',' ).Select( chars => chars.ToList() ).ToList();
    public IEnumerable<IEnumerable<char>> ActualInstructions { get; } = "2023/inputdata/day15.txt".Stream().GetChars().Split( ',' ).Select( chars => chars.ToList() ).ToList();
    public void Part1()
    {
        var result = ActualInstructions.Select( Day15Helpers.Hash ).Sum();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var instructions = ActualInstructions
            .Select( s => Helpers.InstructionParser.Parse( string.Concat( s ) ) );
        var boxes = new AllBoxes();
        foreach ( var instruction in instructions )
        {
            boxes.Apply( instruction );
        }
        var result = boxes.Score();
        Console.WriteLine( result );
    }
}

public class Lens( string identifier, int strength )
{
    public string Identifier { get; } = identifier;
    public int Strength { get; } = strength;

    public override string ToString()
    {
        return $"[{Identifier} {Strength}]";
    }

    public long Score() => Strength;
}

public class LensBox
{
    public List<Lens> Lenses { get; } = new();
    public override string ToString()
    {
        return string.Join( " ", Lenses );
    }

    public void Apply( IINstruction instruction )
    {
        instruction.Visit( this );
    }

    public int Count => Lenses.Count;

    public long Score() => Lenses.Select( ( lens, index ) => lens.Score() * (index + 1) ).Sum();
}

public class AllBoxes
{
    public LensBox[] Boxes { get; } = Enumerable.Range( 0, 256 ).Select( _ => new LensBox() ).ToArray();

    public override string ToString()
    {
        var build = new StringBuilder();
        foreach ( var (box, index) in Boxes.Select( ( box, index ) => (box, index) ) )
        {
            if ( box.Count > 0 )
            {
                build.AppendLine( $"Box {index}: {box}" );
            }
        }
        return build.ToString();
    }

    public void Apply( IINstruction instruction )
    {
        var index = instruction.Hash();
        Boxes[index].Apply( instruction );
    }

    public bool Contains( string identifier )
    {
        return Boxes.Any( box => box.Lenses.Any( lens => lens.Identifier == identifier ) );
    }

    public long Score() => Boxes.Select( ( box, index ) => box.Score() * (index + 1) ).Sum();
}
public static class Day15Helpers
{
    public static long Hash( char c ) => 17 * c % 256;
    public static long Hash( long hash, char c ) => 17 * (hash + c) % 256;

    public static long Hash( IEnumerable<char> chars ) =>
        chars.Aggregate( 0L, Hash );
}

public interface IINstruction
{
    public string Identifier { get; }
    public string ToString();

    public int Hash() => ( int ) Day15Helpers.Hash( Identifier );

    public void Visit( LensBox box );
}

public class RemovalInstruction( string identifier ) : IINstruction
{
    public string Identifier { get; } = identifier;
    public override string ToString()
    {
        return $"{Identifier}-";
    }

    public void Visit( LensBox box )
    {
        box.Lenses.RemoveAll( lens => lens.Identifier == Identifier );
    }
}

public class InsertionInstruction( string identifier, int strength ) : IINstruction
{
    public string Identifier { get; } = identifier;
    public int Strength { get; set; } = strength;

    public override string ToString()
    {
        return $"{Identifier}={Strength}";
    }

    public Lens ToLens() => new Lens( Identifier, Strength );

    public void Visit( LensBox box )
    {
        // If the box already contains a lens with the same identifier, replace it:
        var existing = box.Lenses.FirstOrDefault( lens => lens.Identifier == Identifier );
        var index = box.Lenses.FindIndex( l => l.Identifier == Identifier );
        if ( index >= 0 )
        {
            box.Lenses[index] = ToLens();
        }
        else
        {
            box.Lenses.Add( ToLens() );
        }
    }
}
public static partial class Helpers
{
    #region parser
    public static Parser<RemovalInstruction> RemovalInstructionParser =
        Parse.Letter.AtLeastOnce().Text()
            .Then( identifier => Parse.Char( '-' )
                .Select( _ => new RemovalInstruction( identifier ) )
            );

    public static Parser<InsertionInstruction> InsertionInstructionParser =
        Parse.Letter.AtLeastOnce().Text()
            .Then( identifier => Parse.Char( '=' ).Select( _ => identifier ) )
            .Then( identifier => Parse.Number.Select( int.Parse )
                .Select( number => new InsertionInstruction( identifier, number ) )
            );

    public static Parser<IINstruction> InstructionParser =
        RemovalInstructionParser.Select( r => r as IINstruction )
        .Or( InsertionInstructionParser.Select( i => i as IINstruction ) );
    #endregion parser
}
