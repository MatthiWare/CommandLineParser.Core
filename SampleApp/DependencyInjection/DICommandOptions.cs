using MatthiWare.CommandLine.Core.Attributes;

namespace SampleApp.DependencyInjection
{
    public class DICommandOptions
    {
        [Name("p", "print"), Description("Prints all services"), Required(false), DefaultValue(false)]
        public bool PrintRegisteredServices { get; set; }
    }
}
