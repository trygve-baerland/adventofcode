using System.Linq.Expressions;
using Sprache;

namespace Day11;

public static class MonkeyParserStuff
{
    #region Parse lines

    public static readonly Parser<uint> Header =
         from header in Parse.String("Monkey ").Text()
         from id in Parse.Number
         from colon in Parse.Char(':')
         from eol in Parse.LineEnd
         select uint.Parse(id);

    public static readonly Parser<ulong> Number =
        from number in Parse.Number
        select ulong.Parse(number);
    public static readonly Parser<Queue<ulong>> StartingItems =
        from leading in Parse.WhiteSpace.AtLeastOnce()
        from header in Parse.String("Starting items: ").Token()
        from items in Number.DelimitedBy(Parse.Char(',').Token())
        from eol in Parse.LineEnd
        select new Queue<ulong>(items.ToArray());

    public static readonly Parser<uint> Divisor =
        from leading in Parse.WhiteSpace.AtLeastOnce()
        from header in Parse.String("Test: divisible by ")
        from number in Parse.Number
        from eol in Parse.LineEnd
        select uint.Parse(number);

    public static readonly Parser<uint> IfTrue =
        from leading in Parse.WhiteSpace.AtLeastOnce()
        from header in Parse.String("If true: throw to monkey ")
        from number in Parse.Number
        from eol in Parse.LineEnd
        select uint.Parse(number);

    public static readonly Parser<uint> IfFalse =
        from leading in Parse.WhiteSpace.AtLeastOnce()
        from header in Parse.String("If false: throw to monkey ")
        from number in Parse.Number
        from eol in Parse.LineEnd
        select uint.Parse(number);

    public static readonly Parser<Expression<Func<ulong, ulong>>> Operation =
        from leading in Parse.WhiteSpace.AtLeastOnce()
        from header in Parse.String("Operation: new = ")
        from expr in Lambda
        select expr;

    #endregion Parse lines

    #region Parse expression
    public static Parser<ExpressionType> Operator(string op, ExpressionType opType)
        => Parse.String(op).Token().Return(opType);

    public static readonly Parser<ExpressionType> Add = Operator("+", ExpressionType.AddChecked);
    public static readonly Parser<ExpressionType> Multiply = Operator("*", ExpressionType.MultiplyChecked);

    public static readonly Parser<Expression> Constant =
        Parse.Number
        .Select(x => Expression.Constant(ulong.Parse(x), typeof(ulong)));

    public static readonly Parser<Expression> Parameter =
        Parse.String("old").Text()
        .Select(x => Old!);

    public static ParameterExpression Old { get; } = Expression.Parameter(typeof(ulong), "old");
    public static readonly Parser<Expression> Factor =
        (from lparen in Parse.Char('(')
         from expr in Parse.Ref(() => Expr)
         from rparen in Parse.Char(')')
         select expr).Named("expression")
        .XOr(Constant)
        .XOr(Parameter);

    public static readonly Parser<Expression> Term = Parse.ChainOperator(Multiply, Factor, Expression.MakeBinary);
    public static readonly Parser<Expression> Expr = Parse.ChainOperator(Add, Term, Expression.MakeBinary);

    public static readonly Parser<Expression<Func<ulong, ulong>>> Lambda =
        Expr.Select(body => Expression.Lambda<Func<ulong, ulong>>(
            body,
            new ParameterExpression[] { Old }
        ));

    public static Expression<Func<ulong, ulong>> ParseExpression(string text)
    {
        return Lambda.Parse(text);
    }
    #endregion Parse expression

    public static readonly Parser<Monkey> MonkeyParse =
        from id in Header
        from items in StartingItems
        from expr in Operation
        from divisor in Divisor
        from ifTrue in IfTrue
        from ifFalse in IfFalse
        select new Monkey()
        {
            Id = id,
            Worries = items,
            Divisor = divisor,
            Operation = expr.Compile(),
            IfTrue = ifTrue,
            IfFalse = ifFalse
        };
    #region Main Parser
    public static readonly Parser<Dictionary<uint, Monkey>> AllMonkeys =
        MonkeyParse.DelimitedBy(Parse.LineEnd)
            .Select(x => x.ToDictionary(
                item => item.Id,
                item => item
            ));
    #endregion Main Parser


}