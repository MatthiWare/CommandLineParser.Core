using MatthiWare.CommandLine.Abstractions.Command;
using System;
using static SampleApp.Program;

namespace SampleApp.DependencyInjection
{
    public class CommandWithInjectedServices : Command<Options, DICommandOptions>
    {
        private readonly ICustomInjectedService customService;

        public CommandWithInjectedServices(ICustomInjectedService customService)
        {
            this.customService = customService ?? throw new ArgumentNullException(nameof(customService));
        }

        public override void OnConfigure(ICommandConfigurationBuilder builder)
        {
            builder
                .Name("di")
                .Required(false)
                .Description("Example using Dependency Injection");
        }

        public override void OnExecute(Options options, DICommandOptions commandOptions)
        {
            if (!commandOptions.PrintRegisteredServices)
            {
                customService.Execute($"Unable to execute because {nameof(commandOptions.PrintRegisteredServices)} was set to false");
                return;
            }

            customService.PrintServices();
        }
    }
}
