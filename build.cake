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
var tests = $"./{project}.Tests/{project}.Tests.csproj";
var publishPath = MakeAbsolute(Directory("./output"));

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
		
		XUnit2("./**/bin/" + configuration + "/**/*.Tests.dll", new XUnit2Settings {
			XmlReport = true
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

Task("Default")
    .IsDependentOn("Test");

Task("AppVeyor")
    .IsDependentOn("Publish");

RunTarget(target);