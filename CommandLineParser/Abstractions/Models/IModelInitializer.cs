using System;

namespace MatthiWare.CommandLine.Abstractions.Models
{
    public interface IModelInitializer
    {
        void InitializeModel(Type optionType, object caller, string configureMethodName, string registerMethodName);
    }
}
