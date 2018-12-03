<p align="center">
    <a href="https://ci.appveyor.com/project/Matthiee/CommandLineParser.Core"><img src="https://ci.appveyor.com/api/projects/status/4w6ik2k8lx95afp8?svg=true" alt="Buitld Status (AppVeyor)"></a>
    <a href="https://github.com/MatthiWare/CommandLineParser.Core/issues"><img src="https://img.shields.io/github/issues/MatthiWare/CommandLineParser.Core.svg" alt="Open Issues"></a>
    <a href="https://codecov.io/gh/MatthiWare/CommandLineParser.Core"><img src="https://codecov.io/gh/MatthiWare/CommandLineParser.Core/branch/master/graph/badge.svg" alt="Codecov" /></a>
    <a href="https://www.codefactor.io/repository/github/matthiware/commandlineparser.core"><img src="https://www.codefactor.io/repository/github/matthiware/commandlineparser.core/badge" alt="Codefactor badge"></a>
    <a href="https://tldrlegal.com/license/apache-license-2.0-(apache-2.0)"><img src="https://img.shields.io/badge/License-AGPL%20v3-blue.svg" alt="AGPL v3"></a>
    <a href="https://www.nuget.org/packages/MatthiWare.CommandLineParser">
        <img src="https://img.shields.io/nuget/v/MatthiWare.CommandLineParser.svg" alt="NuGet badge">
        <img src="https://img.shields.io/nuget/dt/MatthiWare.CommandLineParser.svg" alt="NuGet Downloads badge">
    </a>
</p>
<p align="center">
    <img src="https://buildstats.info/appveyor/chart/Matthiee/commandlineparser-core?branch=master" />
</p>

# CommandLineParser
A simple, light-weight and strongly typed commandline parser made in .Net standard.

## Configuration

#### Using model class with attributes

    using MatthiWare.CommandLine;
    using MatthiWare.CommandLine.Core.Attributes;

    static int Main(string[] args)
    {
        var parser = new CommandLineParser<OptionsModel>();

        var parseResult = parser.Parse(args);

        if (result.HasErrors)
        {
            Console.Error.WriteLine(result.Error);
            Console.ReadKey();

            return -1;
        }

        var options = result.Result;

        Console.WriteLine($"Message: {options.Message}");
        Console.WriteLine($"Port: {options.Port}");

        Console.ReadKey();

        return 0;
    }

    public class OptionsModel
    {
        [Required, Name("-m", "--message")]
        public string Message { get; set; } // Mandatory

        [Name("-p", "--port"), DefaultValue(8080)]
        public int Port { get; set; } // Optional
    }

_**Warning:** Attributes will be overwritten by fluent api if both are configured_

#### Using model class and Fluent API

    using MatthiWare.CommandLine;

    static int Main(string[] args)
    {
        var parser = new CommandLineParser<OptionsModel>();

        parser.Configure(opt => opt.Message)
                .Name("-m", "--message")
                .Required();

            parser.Configure(opt => opt.Port)
                .Name("-p", "--port")
                .Default(8080);

        var parseResult = parser.Parse(args);

        if (result.HasErrors)
        {
            Console.Error.WriteLine(result.Error);
            Console.ReadKey();

            return -1;
        }

        var options = result.Result;

        Console.WriteLine($"Message: {options.Message}");
        Console.WriteLine($"Port: {options.Port}");

        Console.ReadKey();

        return 0;
    }

    public class OptionsModel
    {
        public string Message { get; set; } // Mandatory

        public int Port { get; set; } // Optional
    }

_**Warning:** Attributes will be overwritten by fluent api if both are configured_

### Commands

    using MatthiWare.CommandLine;

    static int Main(string[] args)
    {
        var parser = new CommandLineParser<OptionsModel>();

        parser.Configure(opt => opt.Message)
                .Name("-m", "--message")
                .Required();

            parser.Configure(opt => opt.Port)
                .Name("-p", "--port")
                .Default(8080);

        var parseResult = parser.Parse(args);

        var startCmd = parser.AddCommand<CommandOptions>()
            .Name("-s", "--start")
            .Required()
            .OnExecuting(parsedCmdOption => Console.WriteLine($"Starting server using verbose option: {parsedCmdOption.Verbose}"));

        startCmd.Configure(cmd => cmd.Verbose) // configures the command options can also be done using attributes
            .Required()
            .Name("-v", "--verbose");

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

        Console.WriteLine($"Message: {options.Message}");
        Console.WriteLine($"Port: {options.Port}");

        Console.ReadKey();

        return 0;
    }

    public class OptionsModel
    {
        public string Message { get; set; } // Mandatory

        public int Port { get; set; } // Optional
    }

    public class CommandOptions
    {
        public bool Verbose { get; set; }
    }
