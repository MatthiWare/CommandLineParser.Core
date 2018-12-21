using System;

using MatthiWare.CommandLine;

namespace SampleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine($"args: {string.Join(',', args)}");

            var parser = new CommandLineParser<Options>();

            // setup
            parser.Configure(opt => opt.MyInt)
                .Name("-i", "--int")
                .Required();

            parser.Configure(opt => opt.MyString)
                .Name("-s", "--string")
                .Required();

            parser.Configure(opt => opt.MyBool)
                .Name("-b", "--bool")
                .Required();

            parser.Configure(opt => opt.MyDouble)
                .Name("-d", "--double")
                .Required();

            var startCmd = parser.AddCommand<CommandOptions>()
                .Name("-s", "--start")
                .Required()
                .OnExecuting(parsedCmdOption => Console.WriteLine($"Starting server using verbose option: {parsedCmdOption.Verbose}"));

            startCmd.Configure(cmd => cmd.Verbose) // configures the command options can also be done using attributes
                .Required()
                .Name("-v", "--verbose");

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                Console.Error.WriteLine(result.Error);
                Console.ReadKey();

                return -1;
            }

            foreach (var cmdResult in result.CommandResults)
            {
                cmdResult.ExecuteCommand(); // executes the command handler that is configured above. 
            }

            var options = result.Result;

            Console.WriteLine($"MyInt: {options.MyInt}");
            Console.WriteLine($"MyString: {options.MyString}");
            Console.WriteLine($"MyBool: {options.MyBool}");
            Console.WriteLine($"MyDouble: {options.MyDouble}");

            Console.ReadKey();

            return 0;
        }

        private class Options
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
