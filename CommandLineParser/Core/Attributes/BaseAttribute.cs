using System;

namespace MatthiWare.CommandLine.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class BaseAttribute : Attribute
    {
    }
}
