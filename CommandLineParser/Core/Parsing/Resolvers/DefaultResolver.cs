using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class DefaultResolver<T> : BaseArgumentResolver<T>
    {
        private static readonly string TryParseName = "TryParse";
        private static readonly string ParseName = "Parse";

        private readonly Type genericType;
        private readonly ILogger<CommandLineParser> logger;

        public DefaultResolver(ILogger<CommandLineParser> logger)
        {
            genericType = typeof(T);
            this.logger = logger;
        }

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
            if (model is null)
            {
                logger.LogDebug("DefaultResolver input is null");
                result = default;
                return false;
            }

            if (!model.HasValue)
            {
                logger.LogDebug("DefaultResolver model does not have a value", model.Value);
                result = default;
                return false;
            }

            if (genericType.IsEnum && TryParseEnum(model, out result))
            {
                logger.LogDebug("DefaultResolver {input} resolved to {result}", model.Value, result);
                return true;
            }

            var ctor = genericType.GetConstructors()
                .Where(FindStringCtor)
                .FirstOrDefault();

            if (ctor != null)
            {
                result = (T)Activator.CreateInstance(genericType, model.Value);
                return true;
            }

            // search for TryParse method and take one with most arguments
            var tryParseMethod = genericType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(FindTryParse).OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();

            if (tryParseMethod != null)
            {
                int amountOfParams = tryParseMethod.GetParameters().Length;

                if (amountOfParams == 3)
                {
                    var tryParse = (CustomTryParseWithFormat)tryParseMethod.CreateDelegate(typeof(CustomTryParseWithFormat));

                    var returnResult = tryParse(model.Value, CultureInfo.InvariantCulture, out result);
                    logger.LogDebug("DefaultResolver {input} resolved to {result}", model.Value, result);
                    return returnResult;
                }
                else if (amountOfParams == 2)
                {
                    var tryParse = (CustomTryParse)tryParseMethod.CreateDelegate(typeof(CustomTryParse));

                    var returnResult = tryParse(model.Value, out result);
                    logger.LogDebug("DefaultResolver {input} resolved to {result}", model.Value, result);
                    return returnResult;
                }
            }

            // search for Parse method and take one with most arguments
            var parseMethod = genericType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(FindParse).OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();

            if (parseMethod != null)
            {
                int amountOfParams = parseMethod.GetParameters().Length;

                if (amountOfParams == 2)
                {
                    var parse = (CustomParseWithFormat)parseMethod.CreateDelegate(typeof(CustomParseWithFormat));

                    result = parse(model.Value, CultureInfo.InvariantCulture);
                    logger.LogDebug("DefaultResolver {input} resolved to {result}", model.Value, result);
                    return true;
                }
                else if (amountOfParams == 1)
                {
                    var parse = (CustomParse)parseMethod.CreateDelegate(typeof(CustomParse));

                    result = parse(model.Value);
                    logger.LogDebug("DefaultResolver {input} resolved to {result}", model.Value, result);
                    return true;
                }
            }

            logger.LogDebug("DefaultResolver unable to resolve {input}", model.Value);
            result = default;
            return false;
        }

        private delegate bool CustomTryParseWithFormat(string input, IFormatProvider format, out T result);
        private delegate bool CustomTryParse(string input, out T result);
        private delegate T CustomParseWithFormat(string input, IFormatProvider format);
        private delegate T CustomParse(string input);

        private bool TryParseEnum(ArgumentModel model, out T result)
        {
            try
            {
                result = (T)Enum.Parse(genericType, model.Value, true);
                return true;
            }
            catch (Exception)
            {
                result = default;

                return false;
            }
        }

        /// <summary>
        /// Finds the Type.TryParse(String, IFormatProvider, out T result) method
        /// </summary>
        /// <param name="method"></param>
        /// <returns>True or false</returns>
        private bool FindTryParse(MethodInfo method)
        {
            if (method.Name != TryParseName)
            {
                return false;
            }

            if (method.ReturnType != typeof(bool))
            {
                return false;
            }

            var args = method.GetParameters();

            if (args.Length < 2 || args.Length > 3)
            {
                return false;
            }

            // Starts with string
            if (args[0].ParameterType != typeof(string))
            {
                return false;
            }

            // Last one should be out param
            if (!args[args.Length - 1].IsOut || args[args.Length - 1].ParameterType != genericType.MakeByRefType())
            {
                return false;
            }

            // If provided the 2nd param should be an IFormatProvider
            if (args.Length == 3 && args[1].ParameterType != typeof(IFormatProvider))
            {
                return false;
            }

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
            {
                return false;
            }

            if (method.ReturnType != genericType)
            {
                return false;
            }

            var args = method.GetParameters();

            if (args.Length < 1 || args.Length > 2)
            {
                return false;
            }

            // Starts with string
            if (args[0].ParameterType != typeof(string))
            {
                return false;
            }

            // If provided the 2nd param should be an IFormatProvider
            if (args.Length == 2 && args[1].ParameterType != typeof(IFormatProvider))
            {
                return false;
            }

            return true;
        }

        private bool FindStringCtor(ConstructorInfo ctor)
        {
            var args = ctor.GetParameters();

            // more than 1 ctor argument
            if (args.Length != 1)
            {
                return false;
            }

            // Check if it is a string
            if (args[0].ParameterType != typeof(string))
            {
                return false;
            }

            return true;
        }
    }
}
