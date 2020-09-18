using System;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Abstractions.Validations
{
    /// <summary>
    /// Contains all the validators registered
    /// </summary>
    public interface IValidatorsContainer
    {
        /// <summary>
        /// Adds a validator
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="validator"></param>
        void AddValidator<TKey>(IValidator<TKey> validator);

        /// <summary>
        /// Adds a validator
        /// </summary>
        /// <param name="key"></param>
        /// <param name="validator"></param>
        void AddValidator(Type key, IValidator validator);

        /// <summary>
        /// Adds a validator
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="V"></typeparam>
        void AddValidator<TKey, V>() where V : IValidator<TKey>;

        /// <summary>
        /// Adds a validator
        /// </summary>
        /// <param name="key"></param>
        /// <param name="validator"></param>
        void AddValidator(Type key, Type validator);

        /// <summary>
        /// Checks if a validator exists for a given type
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        bool HasValidatorFor<TKey>();

        /// <summary>
        /// Checks if a validator exists for a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool HasValidatorFor(Type type);

        /// <summary>
        /// Returns a read-only list of validators for a given type
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        IReadOnlyCollection<IValidator> GetValidators<TKey>();

        /// <summary>
        /// Returns a read-only list of validators for a given type
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IReadOnlyCollection<IValidator> GetValidators(Type key);
    }
}
