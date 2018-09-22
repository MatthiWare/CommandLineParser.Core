using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface IOptionBuilder<TProperty>
    {
        IOptionBuilder<TProperty> Required(bool required = true);

        IOptionBuilder<TProperty> HelpText(string help);

        IOptionBuilder<TProperty> Default(TProperty defaultValue = default(TProperty));

        IOptionBuilder<TProperty> ShortName(string shortName);

        IOptionBuilder<TProperty> LongName(string longName);
    }
}
