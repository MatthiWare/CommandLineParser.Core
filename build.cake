#tool "nuget:?package=xunit.runner.console&version=2.2.0"
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
		
		DotNetCoreTest(tests,
        new DotNetCoreTestSettings {
            NoBuild = true,
            NoRestore = true,
            Configuration = configuration
        });
});

Task("Publish")
    .IsDependentOn("Test")
    .IsDependentOn("Clean-Publish")
    .Does( () => {
    DotNetCorePublish(solution,
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
            IncludeReferencedProjects = true,

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