using MatthiWare.CommandLine.Abstractions.Command;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SampleApp.Program;

namespace SampleApp.Commands
{
    public class StartServerCommand : Command<Options, CommandOptions>
    {
        public override void OnConfigure(ICommandConfigurationBuilder<CommandOptions> builder)
        {
            base.OnConfigure(builder);

            builder
                .Name("start")
                .Description("Start the server command.")
                .Required();

            builder.Configure(opt => opt.Verbose)
                .Description("Verbose output [true/false]")
                .Default(false)
                .Name("v", "verbose");
        }

        public override async Task OnExecuteAsync(Options options, CommandOptions commandOptions, CancellationToken cancellationToken)
        {
            Console.Write("Starting server");
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(750);
                Console.Write(".");
            }
            Console.WriteLine();

            await Task.Delay(1250);

            Console.Write("Beep boop initializing socket connections");
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(750);
                Console.Write(".");
            }
            Console.WriteLine();

            await Task.Delay(1250);

            Console.WriteLine($"Server started using verbose option: {commandOptions.Verbose}");
        }
    }
}
