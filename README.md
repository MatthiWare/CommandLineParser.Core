[![AppVeyor](https://ci.appveyor.com/api/projects/status/4w6ik2k8lx95afp8?svg=true)](https://ci.appveyor.com/project/Matthiee/commandlineparser-core)
[![Issues](https://img.shields.io/github/issues/MatthiWare/CommandLineParser.Core.svg)](https://github.com/MatthiWare/CommandLineParser.Core/issues)
[![CodeCov](https://codecov.io/gh/MatthiWare/CommandLineParser.Core/branch/master/graph/badge.svg)](https://codecov.io/gh/MatthiWare/CommandLineParser.Core)
[![CodeFactor](https://www.codefactor.io/repository/github/matthiware/commandlineparser.core/badge)](https://www.codefactor.io/repository/github/matthiware/commandlineparser.core)
[![License](https://img.shields.io/badge/License-AGPL%20v3-blue.svg)](https://tldrlegal.com/license/apache-license-2.0-(apache-2.0))
[![Nuget](https://buildstats.info/nuget/MatthiWare.CommandLineParser)](https://www.nuget.org/packages/MatthiWare.CommandLineParser)

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
   var parserResult = parser.Parse(args);

   // check for parsing errors
   if (parserResult.HasErrors)
   {
      Console.ReadKey();

      return -1;
   }

   var serverOptions = parserResult.Result;

   Console.WriteLine($"Parsed port is {serverOptions.Port}");
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

#### For more advanced configuration options see [the wiki](https://github.com/MatthiWare/CommandLineParser.Core/wiki). 


# Contributors

[![](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/images/0)](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/links/0)[![](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/images/1)](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/links/1)[![](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/images/2)](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/links/2)[![](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/images/3)](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/links/3)[![](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/images/4)](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/links/4)[![](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/images/5)](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/links/5)[![](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/images/6)](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/links/6)[![](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/images/7)](https://sourcerer.io/fame/Matthiee/MatthiWare/CommandLineParser.Core/links/7)
