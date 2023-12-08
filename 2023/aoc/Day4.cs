using Utils;
using Sprache;

namespace AoC;

public sealed class Day4 : IPuzzle
{
    public IEnumerable<LotteryCard> TestData = "inputdata/day4_test.txt".GetLines().Select( LotteryCard.FromString ).ToList();
    public IEnumerable<LotteryCard> ActualData = "inputdata/day4.txt".GetLines().Select( LotteryCard.FromString ).ToList();

    public void Part1()
    {
        //var result = TestData.Select( card => card.Prize() ).Sum();
        var result = ActualData.Select( card => card.Prize() ).Sum();
        Console.WriteLine( $"Result: {result}" );
    }


    public void Part2()
    {
        var cards = ActualData.ToList();
        var updates = Enumerable.Repeat( 1, cards.Count() ).ToArray();
        foreach ( var card in cards )
        {
            var numMatches = card.Matches();
            foreach ( var id in Enumerable.Range( card.Id, numMatches ) )
            {
                updates[id] += updates[card.Id - 1];
            }
        }
        var result = updates.Sum();
        Console.WriteLine( $"Result: {result}" );
    }
}

public class LotteryCard( int id, IEnumerable<int> winningNumbers, IEnumerable<int> yourNumbers )
{
    public int Id { get; } = id;
    public List<int> WinningNumbers { get; } = winningNumbers.ToList();
    public List<int> YourNumbers { get; } = yourNumbers.ToList();

    public static LotteryCard FromString( string input )
    {
        return Helpers.LotteryCard.Parse( input );
    }

    public override string ToString()
    {
        return $"Card {Id}: {string.Join( ' ', WinningNumbers )} | {string.Join( ' ', YourNumbers )}";
    }

    public int Matches() => YourNumbers.Where( WinningNumbers.Contains ).Count();

    public int Prize()
    {
        var count = YourNumbers.Where( WinningNumbers.Contains ).Count();
        return count > 0 ? ( int ) System.Math.Pow( 2, count - 1 ) : 0;
    }


}
public static partial class Helpers
{
    #region parser stuff
    public static readonly Parser<IEnumerable<int>> NumberSet =
        Number.DelimitedBy( Parse.Char( ' ' ).AtLeastOnce() );

    public static readonly Parser<LotteryCard> LotteryCard =
        Parse.String( "Card" ).Then( _ => Parse.Char( ' ' ).AtLeastOnce() )
            .Then( _ => Number )
            .Then( id => Parse.Char( ':' ).Token().Select( _ => id ) )
            .Then( id => NumberSet.Select( winners => (id, winners) ) )
            .Then( item => Parse.Char( '|' ).Token().Select( _ => item ) )
            .Then( item => NumberSet.Select( yourNumbers => (item.id, item.winners, yourNumbers) ) )
            .Select( item => new AoC.LotteryCard( item.id, item.winners, item.yourNumbers ) );

    #endregion parser stuff

}
