using System;
using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Extensions.FluentValidations;
using SampleApp.Validations;

namespace SampleApp
{
    public class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine($"args: {string.Join(", ", args)}");

            var parser = new CommandLineParser<Options>();

            // Example to add FluentValidations to the project
            parser.UseFluentValidations((c) => c.AddValidator<Options, OptionsValidator>()
                .AddValidator<CommandOptions, CommandOptionsValidator>());

            // setup
            parser.Configure(opt => opt.MyInt)
                .Name("i", "int")
                .Description("Description for -i option, needs an integer.")
                .Required();

            parser.Configure(opt => opt.MyString)
                .Name("s", "string")
                .Description("Description for -s option, needs a string.")
                .Required();

            parser.Configure(opt => opt.MyBool)
                .Name("b", "bool")
                .Description("Description for -b option, needs a boolean.")
                .Required();

            parser.Configure(opt => opt.MyDouble)
                .Name("d", "double")
                .Description("Description for -d option, needs a double.")
                .Required();

            var startCmd = parser.AddCommand<CommandOptions>()
                .Name("start")
                .Description("Start the server command.")
                .Required()
                .OnExecuting((opt, parsedCmdOption) => Console.WriteLine($"Starting server using verbose option: {parsedCmdOption.Verbose}"));

            startCmd.Configure(cmd => cmd.Verbose) // configures the command options can also be done using attributes
                .Description("Verbose output [true/false]")
                .Default(false)
                .Name("v", "verbose");

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                // note that errors will be automatically printed if `CommandLineParserOptions.AutoPrintUsageAndErrors` is set to true.
                foreach (var exception in result.Errors)
                    HandleException(exception);

                Console.ReadKey();

                return -1;
            }

            var options = result.Result;

            Console.WriteLine($"MyInt: {options.MyInt}");
            Console.WriteLine($"MyString: {options.MyString}");
            Console.WriteLine($"MyBool: {options.MyBool}");
            Console.WriteLine($"MyDouble: {options.MyDouble}");

            Console.ReadKey();

            return 0;
        }

        private static void HandleException(Exception exception)
        {
            Console.WriteLine(exception.Message);
        }

        public class Options
        {
            public int MyInt { get; set; }
            public string MyString { get; set; }
            public bool MyBool { get; set; }
            public double MyDouble { get; set; }
        }

        public class CommandOptions
        {
            public bool Verbose { get; set; }
        }
    }
}
