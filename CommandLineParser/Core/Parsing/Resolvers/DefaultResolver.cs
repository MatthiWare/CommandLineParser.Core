using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Collections;
using MatthiWare.CommandLine.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class DefaultResolver<T> : BaseArgumentResolver<T>
    {
        private const string TryParseName = "TryParse";
        private const string ParseName = "Parse";

        private readonly Type genericType;
        private readonly ILogger<CommandLineParser> logger;
        private readonly IServiceProvider serviceProvider;

        private readonly Dictionary<Type, ICommandLineArgumentResolver> CachedCollectionResolvers = new Dictionary<Type, ICommandLineArgumentResolver>();

        public DefaultResolver(ILogger<CommandLineParser> logger, IServiceProvider serviceProvider)
        {
            genericType = typeof(T);
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override bool CanResolve(ArgumentModel model) => TryResolve(model, out _);

        public override bool CanResolve(string value)
        {
            throw new NotImplementedException();
        }

        public override T Resolve(string value)
        {
            throw new NotImplementedException();
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
                logger.LogDebug("DefaultResolver model does not have a value");
                result = default;
                return false;
            }

            var value = model.Values.FirstOrDefault();

            if (genericType.IsEnum && TryParseEnum(model, out result))
            {
                logger.LogDebug("DefaultResolver {input} resolved to {result}", value, result);
                return true;
            }

            if (AttemptTryParse(value, out result))
            {
                return true;
            }
            
            if (AttemptParse(value, out result))
            {
                return true;
            }
            
            if (AttemptCtor(value, out result))
            {
                return true;
            }

            if (TryResolveCollections(model, out result))
            {
                return true;
            }

            logger.LogDebug("DefaultResolver unable to resolve {input}", value);
            result = default;
            return false;
        }

        private bool TryResolveCollections(ArgumentModel model, out T result)
        {
            result = default;

            if (!genericType.CanHaveMultipleValues())
            {
                return false;
            }

            var resolver = GetOrCreateCollectionTypeResolver();

            if (resolver == null)
            {
                return false;
            }

            if (!resolver.CanResolve(model))
            {
                return false;
            }

            result = (T)resolver.Resolve(model);
            return true;
        }

        private ICommandLineArgumentResolver GetOrCreateCollectionTypeResolver()
        {
            if (CachedCollectionResolvers.TryGetValue(genericType, out var resolver))
            {
                return resolver;
            }

            if (genericType.IsArray)
            {
                var elementType = genericType.GetElementType();

                var arrayType = typeof(IArrayResolver<>).MakeGenericType(elementType);
                resolver = (ICommandLineArgumentResolver)serviceProvider.GetRequiredService(arrayType);

                CachedCollectionResolvers.Add(genericType, resolver);

                return resolver;
            }

            var collectionType = genericType.GetGenericTypeDefinition();
            
            if (typeof(IList<>) == collectionType
                    || typeof(IEnumerable<>) == collectionType
                    || typeof(ICollection<>) == collectionType
                    || typeof(IReadOnlyCollection<>) == collectionType
                    || typeof(IReadOnlyList<>) == collectionType
                    || typeof(List<>) == collectionType)
            {
                var elementType = genericType.GetGenericArguments().First();
                var listType = typeof(IListResolver<>).MakeGenericType(elementType);
                resolver = (ICommandLineArgumentResolver)serviceProvider.GetRequiredService(listType);

                CachedCollectionResolvers.Add(genericType, resolver);

                return resolver;
            }

            if (typeof(ISet<>) == collectionType
                    || typeof(HashSet<>) == collectionType)
            {
                var elementType = genericType.GetGenericArguments().First();
                var listType = typeof(ISetResolver<>).MakeGenericType(elementType);
                resolver = (ICommandLineArgumentResolver)serviceProvider.GetRequiredService(listType);

                CachedCollectionResolvers.Add(genericType, resolver);

                return resolver;
            }

            return null;
        }

        private delegate bool CustomTryParseWithFormat(string input, IFormatProvider format, out T result);
        private delegate bool CustomTryParse(string input, out T result);
        private delegate T CustomParseWithFormat(string input, IFormatProvider format);
        private delegate T CustomParse(string input);

        private bool AttemptCtor(string value, out T result)
        {
            result = default;

            var ctor = genericType.GetConstructors()
                .Where(FindStringCtor)
                .FirstOrDefault();

            if (ctor == null)
            {
                return false;
            }

            result = (T)Activator.CreateInstance(genericType, value);
            return true;
        }

        private bool AttemptParse(string value, out T result)
        {
            result = default;

            // search for Parse method and take one with most arguments
            var parseMethod = genericType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(FindParse).OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();

            if (parseMethod == null)
            {
                return false;
            }

            int amountOfParams = parseMethod.GetParameters().Length;

            if (amountOfParams == 2)
            {
                var parse = (CustomParseWithFormat)parseMethod.CreateDelegate(typeof(CustomParseWithFormat));

                result = parse(value, CultureInfo.InvariantCulture);

                logger.LogDebug("DefaultResolver {input} resolved to {result}", value, result);

                return true;
            }
            else if (amountOfParams == 1)
            {
                var parse = (CustomParse)parseMethod.CreateDelegate(typeof(CustomParse));

                result = parse(value);

                logger.LogDebug("DefaultResolver {input} resolved to {result}", value, result);

                return true;
            }

            return false;
        }

        private bool AttemptTryParse(string value, out T result)
        {
            result = default;

            // search for TryParse method and take one with most arguments
            var tryParseMethod = genericType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(FindTryParse).OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();

            if (tryParseMethod == null)
            {
                return false;
            }

            int amountOfParams = tryParseMethod.GetParameters().Length;

            if (amountOfParams == 3)
            {
                var tryParse = (CustomTryParseWithFormat)tryParseMethod.CreateDelegate(typeof(CustomTryParseWithFormat));

                var returnResult = tryParse(value, CultureInfo.InvariantCulture, out result);

                logger.LogDebug("DefaultResolver TryParse {input} resolved to {result}", value, result);

                return returnResult;
            }
            else if (amountOfParams == 2)
            {
                var tryParse = (CustomTryParse)tryParseMethod.CreateDelegate(typeof(CustomTryParse));

                var returnResult = tryParse(value, out result);

                logger.LogDebug("DefaultResolver TryParse {input} resolved to {result}", value, result);

                return returnResult;
            }

            return false;
        }

        private bool TryParseEnum(ArgumentModel model, out T result)
        {
            try
            {
                result = (T)Enum.Parse(genericType, model.Values.FirstOrDefault(), true);
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
