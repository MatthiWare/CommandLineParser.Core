using System;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// Container class to resolve <see cref="ICommandLineArgumentResolver"/>'s. 
    /// </summary>
    public interface IResolverFactory
    {
        /// <summary>
        /// Checks if the container contains an resolver for the given type.
        /// </summary>
        /// <typeparam name="T">Generic type to check</typeparam>
        /// <returns>True if it contains an resolver, false if not.</returns>
        bool Contains<T>();

        /// <summary>
        /// Checks if the container contains an resolver for the given type.
        /// </summary>
        /// <param name="argument">Type to check</param>
        /// <returns>True if it contains an resolver, false if not.</returns>
        bool Contains(Type argument);

        /// <summary>
        /// Registers an instance of <see cref="ArgumentResolver{TArgument}"/>
        /// </summary>
        /// <typeparam name="TArgument">Argument type to resolve</typeparam>
        /// <param name="resolverInstance">Instance of the resolver</param>
        /// <param name="overwrite">Overwrite if the resolver already exists</param>
        void Register<TArgument>(ArgumentResolver<TArgument> resolverInstance, bool overwrite = false);

        /// <summary>
        /// Registers a <see cref="ICommandLineArgumentResolver{TArgument}"/>
        /// </summary>
        /// <typeparam name="TArgument">Argument type to resolve</typeparam>
        /// <typeparam name="TResolver">Type of the resolver</typeparam>
        /// <param name="overwrite">Overwrite if the resolver already exists</param>
        void Register<TArgument, TResolver>(bool overwrite = false) where TResolver : ArgumentResolver<TArgument>;

        /// <summary>
        /// Registers a <see cref="ICommandLineArgumentResolver"/>
        /// </summary>
        /// <param name="argument">Argument type to resolve</param>
        /// <param name="resolver">Type of the resolver</param>
        /// <param name="overwrite">Overwrite if the resolver already exists</param>
        void Register(Type argument, Type resolver, bool overwrite = false);

        /// <summary>
        /// Creates an resolver for a specific type
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns><see cref="ICommandLineArgumentResolver{T}"/></returns>
        ICommandLineArgumentResolver<T> CreateResolver<T>();

        /// <summary>
        /// Creates an resolver for a specific type
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns><see cref="ICommandLineArgumentResolver{T}"/></returns>
        ICommandLineArgumentResolver CreateResolver(Type type);
    }
}
