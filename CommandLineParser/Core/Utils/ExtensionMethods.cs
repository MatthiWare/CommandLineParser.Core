using MatthiWare.CommandLine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MatthiWare.CommandLine.Core.Utils
{
    internal static class ExtensionMethods
    {
        private static ICommandLineOption FindMatchingOption(ICollection<ICommandLineOption> options, CommandLineParserOptions settings, string item)
        {
            if (string.IsNullOrEmpty(settings.PostfixOption))
                return null;

            return options.Where(option =>
            {
                string shortStr = $"{option.ShortName}{settings.PostfixOption}";
                string longStr = $"{option.LongName}{settings.PostfixOption}";

                return (option.HasShortName && item.StartsWith(shortStr)) || (option.HasLongName && item.StartsWith(longStr));
            }).FirstOrDefault();
        }

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

        public static object InvokeGenericMethod(this MethodInfo method, PropertyInfo propertyInfo, object source, params object[] args)
        {
            var generic = method.MakeGenericMethod(propertyInfo.PropertyType);

            return generic.Invoke(source, args);
        }

        public static object ExecuteGenericRegisterCommand(this object obj, string methodName, Type cmdType, params Type[] optionTypes)
        {
            var baseType = obj.GetType();

            var genericTypes = cmdType.BaseType.GenericTypeArguments;
            var amountGenericTypes = genericTypes.Length;
            var nonNullOptionTypes = optionTypes.Where(t => t != null).ToArray();

            var method = baseType.GetMethods().FirstOrDefault(m =>
            {
                return (m.Name == methodName && m.IsGenericMethod && m.GetGenericArguments().Length == amountGenericTypes);
            });

            if (method == null)
                throw new ArgumentException($"No method by the name '{methodName}' found that takes {amountGenericTypes} generic type arguments.");

            List<Type> types = new List<Type>();
            types.Add(cmdType);

            if (genericTypes.Length > 1)
            {
                if (nonNullOptionTypes.Length > 0)
                    types.Add(nonNullOptionTypes[0]);
                else
                    types.Add(genericTypes[1]);
            }


            var methodInstance = method.MakeGenericMethod(types.ToArray());

            return methodInstance.Invoke(obj, null);

        }

        public static IEnumerable<string> SplitOnPostfix(this IEnumerable<string> self, CommandLineParserOptions settings, ICollection<ICommandLineOption> options)
        {
            bool hasPostfix = !string.IsNullOrEmpty(settings.PostfixOption);

            foreach (var item in self)
            {
                int idx = -1;

                if (hasPostfix && (idx = item.IndexOf(settings.PostfixOption)) != -1 && FindMatchingOption(options, settings, item) != null)
                {
                    var tokens = new[] { item.Substring(0, idx), item.Substring(idx + 1, item.Length - idx - 1) };

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
