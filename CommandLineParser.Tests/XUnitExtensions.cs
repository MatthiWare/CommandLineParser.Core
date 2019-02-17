using System;
using System.Linq.Expressions;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Tests
{
    public static class XUnitExtensions
    {
        public static LambdaExpression CreateLambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            return expression;
        }

        public static void AssertNoErrors<T>(this IParserResult<T> result)
        {
            if (result == null)
                throw new NullReferenceException("Parsing result was null");

            foreach (var err in result.Errors)
                throw err;
        }

    }
}
