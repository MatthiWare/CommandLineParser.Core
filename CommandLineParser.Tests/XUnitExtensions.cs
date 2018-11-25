using System;
using System.Linq.Expressions;
using Xunit.Sdk;

namespace MatthiWare.CommandLineParser.Tests
{
    public static class XUnitExtensions
    {
        public static void Fail(string reason)
            => throw new XunitException(reason);

        public static LambdaExpression CreateLambda<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            return expression;
        }
    }
}
