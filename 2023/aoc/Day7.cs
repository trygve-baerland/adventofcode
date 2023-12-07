using Sprache;
using Utils;

namespace AoC;

public sealed class Day7 : IPuzzle
{
    public List<Hand> TestHands = "inputdata/day7_test.txt".GetLines()
        .Select( line => Helpers.Hand.Parse( line ) )
        .ToList();
    public List<Hand> ActualHands = "inputdata/day7.txt".GetLines()
        .Select( line => Helpers.Hand.Parse( line ) )
        .ToList();
    public void Part1()
    {
        var result = ActualHands.OrderBy(
            keySelector: h => h,
            comparer: new JokerComparer( false )
        ).Select( ( h, i ) => (i + 1) * h.Bid )
        .Sum();
        Console.WriteLine( $"Result: {result}" );
    }

    public void Part2()
    {
        var result = ActualHands.OrderBy(
            keySelector: h => h,
            comparer: new JokerComparer( true )
        )
        .Select( ( h, i ) => (i + 1) * h.Bid )
        .Sum();
        Console.WriteLine( $"Result: {result}" );
    }
}

public class Card( char label )
{
    public int Value( bool asJoker = false ) => label switch {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => asJoker ? 0 : 11,
        'T' => 10,
        char c when char.IsDigit( c ) => label - '0',
        _ => throw new ArgumentException( $"Invalid card label {label}" )
    };

    public override string ToString()
    {
        return $"{Value( false ) switch {
            14 => 'A',
            13 => 'K',
            12 => 'Q',
            11 => 'J',
            10 => 'T',
            int d => d.ToString()
        }}";
    }

    public int CompareWith( Card? other, bool wj = false )
    {
        if ( other is null ) return 1;

        return Value( wj ).CompareTo( other.Value( wj ) );
    }
}

public class Hand( IEnumerable<Card> cards, int bid )
{
    public Card[] Cards { get; } = cards.ToArray();

    public int Bid { get; } = bid;

    public int NumJokers() => Cards.Count( c => c.Value( true ) == 0 );

    public override string ToString()
    {
        return $"{string.Join( ", ", Cards.Select( c => c.ToString() ) )} {Bid}";
    }

    public bool IsNOfAKind( int N, int numJokers = 0, bool wj = false )
    {
        // N=5 betyr at vi skal se om det er fem like
        // numJokers er antall J i hånda
        // wj: true hvis vi skal se på jokere som wildcards
        if ( numJokers == 0 )
        {
            return Cards
            .GroupBy( c => c.Value( wj ) )
            .Where( g => g.Key > 0 )
            .Any( g => g.Count() == N );
        }
        if ( numJokers == 5 && N == 5 )
        {
            return true;
        }
        return IsNOfAKind( N - 1, numJokers - 1, wj );
    }
    public bool IsHouse( int numJokers = 0, bool wj = false )
    {
        if ( numJokers == 0 )
        {
            return Cards.GroupBy( c => c.Value( wj ) ).Where( g => g.Key > 0 ).Any( g => g.Count() == 3 ) &&
                Cards.GroupBy( c => c.Value( wj ) ).Where( g => g.Key > 0 ).Any( g => g.Count() == 2 );
        }
        if ( numJokers > 0 )
        {
            return HasNPairs( 2, 0, wj );
        }
        throw new ArgumentException( $"Invalid full house card {this}" );
    }
    public bool HasNPairs( int N, int numJokers = 0, bool wj = false )
    {
        if ( numJokers == 0 )
        {
            return Cards.GroupBy( c => c.Value( wj ) ).Where( g => g.Key > 0 ).Count( g => g.Count() == 2 ) >= N;
        }
        if ( numJokers == 1 && N == 1 )
        {
            return AllDistinct( wj );
        }
        else if ( numJokers == 1 && N > 1 )
        {
            return false;
        }
        throw new ArgumentException( $"Invalid  two pairs card {this}" );
    }

    public bool AllDistinct( bool wj = false ) =>
        Cards.GroupBy( c => c.Value( wj ) ).All( g => g.Count() == 1 );

    public int HandTypeLevel( bool wj = false )
    {
        var numJokers = wj ? NumJokers() : 0;
        if ( IsNOfAKind( 5, numJokers, wj ) ) return 7;
        if ( IsNOfAKind( 4, numJokers, wj ) ) return 6;
        if ( IsHouse( numJokers, wj ) ) return 5;
        if ( IsNOfAKind( 3, numJokers, wj ) ) return 4;
        if ( HasNPairs( 2, numJokers, wj ) ) return 3;
        if ( HasNPairs( 1, numJokers, wj ) ) return 2;
        if ( AllDistinct( wj ) ) return 1;
        throw new ArgumentException( $"Invalid hand {this}" );
    }

}

public class JokerComparer( bool withJoker ) : IComparer<Hand>
{
    public bool WithJoker { get; } = withJoker;
    public int Compare( Hand? x, Hand? y )
    {
        if ( x is null )
        {
            if ( y is null ) return 0;
            return -1;
        };
        if ( y is null ) return 1;
        var xLevel = x.HandTypeLevel( WithJoker );
        var yLevel = y.HandTypeLevel( WithJoker );
        if ( xLevel != yLevel ) return xLevel.CompareTo( yLevel );
        return x.Cards.Zip( y.Cards )
            .Select( item => item.First.CompareWith( item.Second, WithJoker ) )
            .FirstOrDefault( c => c != 0, defaultValue: 0 );
    }
}


public static partial class Helpers
{
    #region parsers
    public static readonly Parser<Card> Card =
        Parse.Chars( "AKQJT98765432" ).Select( c => new Card( c ) );

    public static readonly Parser<Hand> Hand =
        Card.Repeat( 5 )
            .Then( cards => Parse.WhiteSpace.AtLeastOnce().Select( _ => cards ) )
            .Then( cards => Parse.Number.Select( bid => new Hand( cards, int.Parse( bid ) ) ) );
    #endregion
}
