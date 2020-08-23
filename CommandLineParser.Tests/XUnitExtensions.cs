using System;
using System.Linq.Expressions;
using MatthiWare.CommandLine.Abstractions.Parsing;
using Xunit;

namespace MatthiWare.CommandLine.Tests
{
    public static class XUnitExtensions
    {
        public static LambdaExpression CreateLambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            return expression;
        }

        public static bool AssertNoErrors<T>(this IParserResult<T> result, bool shouldThrow = true)
        {
            if (result == null)
                throw new NullReferenceException("Parsing result was null");

            foreach (var err in result.Errors)
            {
                if (shouldThrow)
                {
                    throw err;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }

    [CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
    public class NonParallelCollection
    { 
    }
}
