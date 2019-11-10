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
        private static readonly string TryParseName = "TryParse";
        private static readonly string ParseName = "Parse";

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
            var tryParseMethod = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(FindTryParse).OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();

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

            // search for Parse method and take one with most arguments
            var parseMethod = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(FindParse).OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();

            if (parseMethod != null)
            {
                int amountOfParams = parseMethod.GetParameters().Length;

                if (amountOfParams == 2)
                {
                    var parse = (CustomParseWithFormat)parseMethod.CreateDelegate(typeof(CustomParseWithFormat));

                    result = parse(model.Value, CultureInfo.InvariantCulture);
                    return true;
                }
                else if (amountOfParams == 1)
                {
                    var parse = (CustomParse)parseMethod.CreateDelegate(typeof(CustomParse));

                    result = parse(model.Value);
                    return true;
                }
            }

            result = default;
            return false;
        }

        private delegate bool CustomTryParseWithFormat(string input, IFormatProvider format, out T result);
        private delegate bool CustomTryParse(string input, out T result);
        private delegate T CustomParseWithFormat(string input, IFormatProvider format);
        private delegate T CustomParse(string input);

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
            if (!args[args.Length - 1].IsOut || args[args.Length - 1].ParameterType != typeof(T).MakeByRefType())
                return false;

            // If provided the 2nd param should be an IFormatProvider
            if (args.Length == 3 && args[1].ParameterType != typeof(IFormatProvider))
                return false;

            return true;
        }

        /// <summary>
        /// Finds the Type.TryParse(String, IFormatProvider, out T result) method
        /// </summary>
        /// <param name="method"></param>
        /// <returns>True or false</returns>
        private bool FindParse(MethodInfo method)
        {
            if (method.Name != ParseName)
                return false;

            if (method.ReturnType != typeof(T))
                return false;

            var args = method.GetParameters();

            if (args.Length < 1 || args.Length > 2)
                return false;

            // Starts with string
            if (args[0].ParameterType != typeof(string))
                return false;

            // If provided the 2nd param should be an IFormatProvider
            if (args.Length == 2 && args[1].ParameterType != typeof(IFormatProvider))
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
