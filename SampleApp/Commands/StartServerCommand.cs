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
            await WriteTextWithDots("Starting server");

            await Task.Delay(1250);

            await WriteTextWithDots("Beep boop initializing socket connections");

            await Task.Delay(1250);

            Console.WriteLine($"Server started using verbose option: {commandOptions.Verbose}");
        }

        private async Task WriteTextWithDots(string text, int delayPerDot = 750, int amountOfDots = 3)
        {
            Console.Write(text);

            for (int i = 0; i < amountOfDots; i++)
            {
                await Task.Delay(delayPerDot);
                Console.Write(".");
            }

            Console.WriteLine();
        }
    }
}
