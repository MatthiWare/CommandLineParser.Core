using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Utils;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MatthiWare.CommandLine.Core.Models
{
    public class ModelInitializer : IModelInitializer
    {
        public void InitializeModel(Type optionType, object caller, string configureMethodName, string registerMethodName)
        {
            var properties = optionType.GetProperties();

            foreach (var propInfo in properties)
            {
                var attributes = propInfo.GetCustomAttributes(true);

                var lambda = propInfo.GetLambdaExpression(out string key);

                var cfg = caller.GetType().GetMethod(configureMethodName, BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var attribute in attributes)
                {
                    switch (attribute)
                    {
                        case RequiredAttribute required:
                            GetOption(cfg, propInfo, lambda, key).Required(required.Required);
                            break;
                        case DefaultValueAttribute defaultValue:
                            GetOption(cfg, propInfo, lambda, key).Default(defaultValue.DefaultValue);
                            break;
                        case DescriptionAttribute helpText:
                            GetOption(cfg, propInfo, lambda, key).Description(helpText.Description);
                            break;
                        case NameAttribute name:
                            GetOption(cfg, propInfo, lambda, key).Name(name.ShortName, name.LongName);
                            break;
                    }
                }

                var commandType = propInfo.PropertyType;

                bool isAssignableToCommand = typeof(Abstractions.Command.Command).IsAssignableFrom(commandType);

                if (isAssignableToCommand)
                {
                    caller.ExecuteGenericRegisterCommand(registerMethodName, commandType);
                }
            }

            IOptionBuilder GetOption(MethodInfo method, PropertyInfo prop, LambdaExpression lambda, string key)
            {
                return method.InvokeGenericMethod(prop, caller, lambda, key) as IOptionBuilder;
            }
        }
    }
}
