using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MatthiWare.CommandLine.Core.Utils
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Checks if an open generic type is assignable to the generic type
        /// 
        /// For more info see:
        /// https://stackoverflow.com/a/5461399/6058174
        /// </summary>
        public static bool IsAssignableToGenericType(this Type self, Type genericType)
        {
            var interfaceTypes = self.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (self.IsGenericType && self.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = self.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public static IEnumerable<string> SplitOnPostfix(this IEnumerable<string> self, bool hasShortPostfix, bool hasLongPostfix, string shortPostfix, string longPostfix)
        {
            foreach (var item in self)
            {
                string[] tokens = null;
                int idx = -1;

                if (hasLongPostfix && (idx = item.IndexOf(longPostfix)) != -1)
                {
                    tokens = new []{ item.Substring(0, idx ), item.Substring(idx + 1, item.Length - idx - 1) };
                } 
                else if (hasShortPostfix && (idx = item.IndexOf(shortPostfix)) != -1)
                {
                    tokens = new[] { item.Substring(0, idx ), item.Substring(idx + 1, item.Length - idx - 1) };
                }

                if (idx != -1)
                {
                    yield return tokens[0];
                    yield return tokens[1];
                }
                else
                {
                    yield return item;
                }
            }
        }

        public static LambdaExpression GetLambdaExpression(this PropertyInfo propInfo, out string key)
        {
            var entityType = propInfo.DeclaringType;
            var propType = propInfo.PropertyType;
            var parameter = Expression.Parameter(entityType, entityType.FullName);
            var property = Expression.Property(parameter, propInfo);
            var funcType = typeof(Func<,>).MakeGenericType(entityType, propType);

            key = $"{entityType.ToString()}.{propInfo.Name}";

            return Expression.Lambda(funcType, property, parameter);
        }

        public static StringBuilder AppendIf(this StringBuilder self, bool contition, string text)
        {
            if (contition)
                self.Append(text);

            return self;
        }
    }
}
