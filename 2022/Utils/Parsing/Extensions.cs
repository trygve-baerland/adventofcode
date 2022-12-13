using Sprache;
namespace Utils.Parsers;

public static class ParserExtensions
{
    public static Parser<TResult> FollowedBy<TFirst, TSecond, TResult>(
        this Parser<TFirst> first,
        Parser<TSecond> second,
        Func<TFirst, TSecond, TResult> semanticAction
        )
    {
        return
            from item1 in first
            from item2 in second
            select semanticAction(item1, item2);
    }
}