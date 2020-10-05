using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MatthiWare.CommandLine.Core.Utils
{
    /// <summary>
    /// Utility extension methods used the command line parser or other internal classes
    /// </summary>
    public static class ExtensionMethods
    {
        internal static bool CanHaveMultipleValues(this Type type)
        {
            if (type.IsArray)
            {
                return true;
            }

            if (type.IsEnum)
            {
                var flagsAttribute = type.GetCustomAttribute<FlagsAttribute>();

                return flagsAttribute != null;
            }

            if (!type.IsGenericType)
            {
                return false;
            }

            var genericType = type.GetGenericTypeDefinition();

            if (typeof(IList<>) == genericType
                    || typeof(IEnumerable<>) == genericType
                    || typeof(ICollection<>) == genericType
                    || typeof(IReadOnlyCollection<>) == genericType
                    || typeof(IReadOnlyList<>) == genericType
                    || typeof(List<>) == genericType 
                    || typeof(ISet<>) == genericType
                    || typeof(HashSet<>) == genericType)
            {
                return true;
            }

            return false;
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
                {
                    return true;
                }
            }

            if (self.IsGenericType && self.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = self.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType);
        }

        /// <summary>
        /// Invokes a generic method
        /// </summary>
        /// <param name="method">Method to invoke</param>
        /// <param name="propertyInfo">Generic argument to use</param>
        /// <param name="source">Instance object</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Method invocation result</returns>
        public static object InvokeGenericMethod(this MethodInfo method, PropertyInfo propertyInfo, object source, params object[] args)
        {
            var generic = method.MakeGenericMethod(propertyInfo.PropertyType);

            return generic.Invoke(source, args);
        }

        /// <summary>
        /// Execute generic register command
        /// </summary>
        /// <param name="obj">Instance object</param>
        /// <param name="methodName">Register method name</param>
        /// <param name="cmdType">Type of the command</param>
        /// <param name="optionTypes">Option types</param>
        /// <returns></returns>
        public static object ExecuteGenericRegisterCommand(this object obj, string methodName, Type cmdType, params Type[] optionTypes)
        {
            var baseType = obj.GetType();

            var genericTypes = cmdType.BaseType.GenericTypeArguments;
            var amountGenericTypes = genericTypes.Length;
            var nonNullOptionTypes = optionTypes.Where(t => t != null).ToArray();

            var method = baseType.GetMethods().FirstOrDefault(m =>
            {
                return FindBestFittingGenericMethod(m, methodName, amountGenericTypes);
            });

            if (method == null)
            {
                throw new ArgumentException($"No method by the name '{methodName}' found that takes {amountGenericTypes} generic type arguments.");
            }

            var types = new List<Type>
            {
                cmdType
            };

            if (genericTypes.Length > 1)
            {
                if (nonNullOptionTypes.Length > 0 && cmdType != nonNullOptionTypes[0])
                {
                    types.Add(nonNullOptionTypes[0]);
                }
                else
                {
                    types.Add(genericTypes[1]);
                }
            }

            var methodInstance = method.MakeGenericMethod(types.ToArray());

            return methodInstance.Invoke(obj, null);
        }

        private static bool FindBestFittingGenericMethod(MethodInfo methodInfo, string methodName, int amountOfGenericArgs)
        {
            if (!methodInfo.IsGenericMethod)
            {
                return false;
            }

            if (methodInfo.Name != methodName)
            {
                return false;
            }

            if (amountOfGenericArgs == 0)
            {
                return methodInfo.GetGenericArguments().Length == 1;
            }
            else
            {
                return methodInfo.GetGenericArguments().Length == amountOfGenericArgs;
            }
        }

        /// <summary>
        /// Splits on postfix
        /// </summary>
        /// <param name="self"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string WithoutPreAndPostfixes(this string self, CommandLineParserOptions settings)
        {
            bool hasLongPrefix = !string.IsNullOrEmpty(settings.PrefixLongOption);
            bool hasShortPrefix = !string.IsNullOrEmpty(settings.PrefixShortOption);
            bool hasPostfix = !string.IsNullOrEmpty(settings.PostfixOption);

            string result = self;

            if (hasLongPrefix && self.StartsWith(settings.PrefixLongOption))
            {
                var index = self.IndexOf(settings.PrefixLongOption) + 1;

                if (index != 0)
                { 
                    result = self.Substring(index);
                }
            }
            else if (hasShortPrefix && self.StartsWith(settings.PrefixShortOption))
            {
                var index = self.IndexOf(settings.PrefixShortOption) + 1;

                if (index != 0)
                { 
                    result = self.Substring(index);
                }
            }

            if (hasPostfix && self.EndsWith(settings.PostfixOption))
            {
                var index = self.LastIndexOf(settings.PostfixOption) + 1;

                if (index != 0)
                { 
                    result = self.Substring(0, result.Length - index);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates lambda expression from property info
        /// </summary>
        /// <param name="propInfo"></param>
        /// <param name="key"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Append text if condition evaluates to true
        /// </summary>
        /// <param name="self"></param>
        /// <param name="contition"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static StringBuilder AppendIf(this StringBuilder self, bool contition, string text)
        {
            if (contition)
            {
                self.Append(text);
            }

            return self;
        }

        /// <summary>
        /// Compares strings using <see cref="string.Equals(string, string, StringComparison)"/> with <see cref="StringComparison.InvariantCultureIgnoreCase"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="equals"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string input, string equals)
            => input.Equals(equals, StringComparison.InvariantCultureIgnoreCase);
    }
}
