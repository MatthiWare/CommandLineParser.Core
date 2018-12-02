using System;

namespace MatthiWare.CommandLine.Core.Attributes
{
    [Obsolete("Help texts are currently not working.", false)]
    public class HelpTextAttribute : BaseAttribute
    {
        public string HelpText { get; private set; }

        public HelpTextAttribute(string helpText) => HelpText = helpText;
    }
}
