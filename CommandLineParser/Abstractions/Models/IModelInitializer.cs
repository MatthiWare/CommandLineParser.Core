using System;

namespace MatthiWare.CommandLine.Abstractions.Models
{
    /// <summary>
    /// Tool used to initialize based on a model
    /// </summary>
    public interface IModelInitializer
    {
        /// <summary>
        /// Configure options and register commands from the option model
        /// </summary>
        /// <param name="optionType">Option model type</param>
        /// <param name="caller">Caller instance</param>
        /// <param name="configureMethodName">configure method name</param>
        /// <param name="registerMethodName">register method name</param>
        void InitializeModel(Type optionType, object caller, string configureMethodName, string registerMethodName);
    }
}
