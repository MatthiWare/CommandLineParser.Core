using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class DefaultResolver<T> : ArgumentResolver<T>
    {
        private readonly Type m_type;
        private static readonly string TryParseName = "TryParse";

        public override bool CanResolve(ArgumentModel model)
        {
            return TryResolve(model, out _);
        }

        public override T Resolve(ArgumentModel model)
        {
            TryResolve(model, out T instance);

            return instance;
        }

        private bool TryResolve(ArgumentModel model, out T result)
        {
            if (!model.HasValue)
            {
                result = default;
                return false;
            }

            var ctor = typeof(T).GetConstructors()
                .Where(FindStringCtor)
                .FirstOrDefault();

            if (ctor != null)
            {
                result = (T)Activator.CreateInstance(typeof(T), model.Value);
                return true;
            }

            // search for TryParse method and take one with most arguments
            var tryParseMethod = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(FindTryParse).Reverse().FirstOrDefault();

            if (tryParseMethod != null)
            {
                int amountOfParams = tryParseMethod.GetParameters().Length;

                if (amountOfParams == 3)
                {
                    var tryParse = (CustomTryParseWithFormat)tryParseMethod.CreateDelegate(typeof(CustomTryParseWithFormat));

                    return tryParse(model.Value, CultureInfo.InvariantCulture, out result);
                }
                else if (amountOfParams == 2)
                {
                    var tryParse = (CustomTryParse)tryParseMethod.CreateDelegate(typeof(CustomTryParse));

                    return tryParse(model.Value, out result);
                }
            }

            result = default;
            return false;
        }

        private delegate bool CustomTryParseWithFormat(string input, IFormatProvider format, out T result);
        private delegate bool CustomTryParse(string input, out T result);

        /// <summary>
        /// Finds the Type.TryParse(String, IFormatProvider, out T result) method
        /// </summary>
        /// <param name="method"></param>
        /// <returns>True or false</returns>
        private bool FindTryParse(MethodInfo method)
        {
            if (method.Name != TryParseName)
                return false;

            if (method.ReturnType != typeof(bool))
                return false;

            var args = method.GetParameters();

            if (args.Length < 2 || args.Length > 3)
                return false;

            // Starts with string
            if (args[0].ParameterType != typeof(string))
                return false;

            // Last one should be out param
            if (!args[args.Length - 1].IsOut)
                return false;

            // If provided the 2nd param should be an IFormatProvider
            if (args.Length == 3 && args[1].ParameterType != typeof(IFormatProvider))
                return false;

            return true;
        }

        private bool FindStringCtor(ConstructorInfo ctor)
        {
            var args = ctor.GetParameters();

            // more than 1 ctor argument
            if (args.Length != 1)
                return false;

            // Check if it is a string
            if (args[0].ParameterType != typeof(string))
                return false;

            return true;
        }
    }
}
