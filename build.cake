#tool "nuget:?package=xunit.runner.console&version=2.2.0"
#tool nuget:?package=Codecov
#addin nuget:?package=Cake.Codecov
#addin nuget:?package=Cake.Coverlet
///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PARAMETERS
//////////////////////////////////////////////////////////////////////

var project = "CommandLineParser";
var solution = $"./{project}.sln";
var cmdParserProject = $"./{project}/{project}.csproj";
var nuspecFile = $"./{project}/{project}.nuspec";
var tests = $"./{project}.Tests/{project}.Tests.csproj";
var publishPath = MakeAbsolute(Directory("./output"));
var nugetPackageDir = MakeAbsolute(Directory("./nuget"));

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does( () => {
        CleanDirectories($"./{project}/obj/**/*.*");
        CleanDirectories($"./{project}/bin/{configuration}/**/*.*");
});

Task("Clean-Publish")
    .IsDependentOn("Clean")
    .Does( () => {
        CleanDirectory(publishPath);
});

Task("Build")
    .Does(() => 
	{
		DotNetCoreBuild(solution,
			new DotNetCoreBuildSettings 
			{
				NoRestore = false,
				Configuration = configuration
			});
	});

Task("Test")
    .IsDependentOn("Build")
    .Does( () => {
		
        var coverletSettings = new CoverletSettings {
                CollectCoverage = true,
                CoverletOutputFormat = CoverletOutputFormat.opencover,
                CoverletOutputName = $"coverage.xml"
            };

		DotNetCoreTest(tests,
            new DotNetCoreTestSettings {
                NoBuild = true,
                NoRestore = true,
                Configuration = configuration
            }, coverletSettings);

        foreach(var file in GetFiles("C:\\projects\\commandlineparser-core\\*.*"))
            {
                Information(file.Path.FullPath);
            }


        // Upload a coverage report.
        Codecov("coverage.xml");
});

Task("Publish")
    .IsDependentOn("Test")
    .IsDependentOn("Clean-Publish")
    .Does( () => {
    DotNetCorePublish(cmdParserProject,
        new DotNetCorePublishSettings {

            NoRestore = true,
            Configuration = configuration,
            OutputDirectory = publishPath
        });
});

Task("Publish-NuGet")
	.IsDependentOn("Publish")
	.Does(() => 
	{
        var nuGetPackSettings = new NuGetPackSettings
        {
            BasePath = publishPath,
            OutputDirectory = nugetPackageDir,
            IncludeReferencedProjects = false,

            Properties = new Dictionary<string, string>
            {
                { "Configuration", configuration }
            }
        };

        NuGetPack(nuspecFile, nuGetPackSettings);

	});

Task("Default")
    .IsDependentOn("Test");

Task("AppVeyor")
    .IsDependentOn("Publish-NuGet");

RunTarget(target);