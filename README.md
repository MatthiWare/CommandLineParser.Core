<p align="center">
    <a href="https://ci.appveyor.com/project/Matthiee/commandlineparser-core"><img src="https://ci.appveyor.com/api/projects/status/4w6ik2k8lx95afp8?svg=true" alt="Buitld Status (AppVeyor)"></a>
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
A simple, light-weight and strongly typed commandline parser made in .NET Standard!

## Installation
```powershell
PM> Install-Package MatthiWare.CommandLineParser
```

# Quick Start

Example of configuring the port option using the Fluent API. 

``` csharp
static void Main(string[] args)
{
   // create the parser
   var parser = new CommandLineParser<ServerOptions>();
   
   // configure the options using the Fluent API
   parser.Configure(options => options.Port)
      .Name("p", "port")
      .Description("The port of the server")
      .Required();

   // parse
   var parsed = parser.Parse(args);

   // check for parsing errors
   if (parsed.HasErrors)
   {
      Console.ReadKey();

      return -1;
   }

   Console.WriteLine($"Parsed port is {parsed.Result.Port}");
}

private class ServerOptions
{
   // options
   public int Port { get; set; }
}

```

Run command line

```shell
dotnet myapp --port 2551
```

### For more advanced configuration options see [the wiki](https://github.com/MatthiWare/CommandLineParser.Core/wiki). 
