using Sprache;
using Utils;

namespace AoC;

public sealed class Day6 : IPuzzle
{
    public List<Race> TestRaces = Helpers.TimesAndDistances.Parse(
        string.Concat( "inputdata/day6_test.txt".Stream().GetChars() )
    ).GetRaces();

    public List<Race> ActualRaces = Helpers.TimesAndDistances.Parse(
        string.Concat( "inputdata/day6.txt".Stream().GetChars() )
    ).GetRaces();
    public void Part1()
    {
        foreach ( var race in TestRaces )
        {
            Console.WriteLine( $"Race {race} ({race.WaysToWin()})" );
        }
        var result = ActualRaces.Select( race => race.WaysToWin() )
            .Aggregate( ( x, item ) => x * item );

        Console.WriteLine( $"Result: {result}" );
    }

    public void Part2()
    {
        var result = ActualRaces.SumRaces().WaysToWin();
        Console.WriteLine( $"Result: {result}" );
    }
}

public class Race( long time, long distance )
{
    public long Time { get; } = time;
    public long Distance { get; } = distance;

    public override string ToString()
    {
        return $"({Time}, {Distance})";
    }

    /// The race distance is (Time - Load)*Load
    /// And we want this to be greater than 
    /// distance
    /// (t-x)x > d
    /// x^2 - tx + d > 0
    /// D := t^2 - 4d > 0 (always true)
    /// x* = (t +- sqrt(D))/2
    /// [x*] = sqrt(d)
    public long WaysToWin()
    {
        // return Enumerable.Range( 0, ( int ) Time )
        //    .Select( t => (Time - t) * t )
        //    .Where( d => d > Distance )
        //    .Count();
        double D2 = Time * Time - 4 * Distance;
        if ( D2 < 0 ) throw new ArgumentException( $"Negative discriminant {D2}" );
        var D = Math.Sqrt( D2 );
        return ( long ) (0.5 * (Math.Floor( D ) + Math.Ceiling( D ))) + 1;
    }
}
public static partial class Helpers
{
    public static readonly Parser<IEnumerable<long>> LongNumbers =
        Long.DelimitedBy( Parse.WhiteSpace.AtLeastOnce() );

    public static readonly Parser<(IEnumerable<long> times, IEnumerable<long> distances)> TimesAndDistances =
        Parse.String( "Time: " ).Token()
            .Then( _ => LongNumbers )
            .Then( times => Parse.LineEnd.Select( _ => times ) )
            .Then( times => Parse.String( "Distance: " ).Token().Select( _ => times ) )
            .Then( times => LongNumbers.Select( distances => (times, distances) ) );

    public static List<Race> GetRaces( this (IEnumerable<long> times, IEnumerable<long> distances) timesAndDistances )
    {
        var times = timesAndDistances.times;
        var distances = timesAndDistances.distances;
        return times.Zip( distances ).Select( item => new Race( item.First, item.Second ) ).ToList();
    }

    public static Race SumRaces( this List<Race> races )
    {
        var time = long.Parse( String.Concat( races.Select( race => race.Time.ToString() ) ) );
        var distance = long.Parse( string.Concat( races.Select( race => race.Distance.ToString() ) ) );
        return new Race( time, distance );
    }
}
