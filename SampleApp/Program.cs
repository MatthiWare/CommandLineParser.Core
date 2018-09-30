using System;
using MatthiWare.CommandLine;

namespace SampleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            var parser = new CommandLineParser<Options>();

            // setup
            parser.Configure(opt => opt.MyInt)
                .ShortName("-i")
                .LongName("--int")
                .Required();

            parser.Configure(opt => opt.MyString)
                .ShortName("-s")
                .LongName("--string")
                .Required();

            parser.Configure(opt => opt.MyBool)
                .ShortName("-b")
                .LongName("--bool")
                .Required();

            parser.Configure(opt => opt.MyDouble)
                .ShortName("-d")
                .LongName("--double")
                .Required();

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                Console.Error.WriteLine(result.Error);
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

        private class Options
        {
            public int MyInt { get; set; }
            public string MyString { get; set; }
            public bool MyBool { get; set; }
            public double MyDouble { get; set; }
        }
    }
}
