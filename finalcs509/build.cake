#tool nuget:?package=coverlet.console&version=3.2.0
#tool nuget:?package=reportgenerator&version=5.1.26
#tool nuget:?package=xunit.runner.console&version=2.4.2
#tool nuget:?package=docfx.console&version=2.59.4

//////////////////////////////////////////////////////
// Arguments
//////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = "Release";

//////////////////////////////////////////////////////
// Tasks
//////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./**/bin/" + configuration);
    CleanDirectories("./**/obj/" + configuration);
});

Task("Restore")
    .Does(() =>
{
    DotNetRestore("./ATMApp.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetBuild("./ATMApp.sln", new DotNetBuildSettings
    {
        Configuration = configuration
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetTest("./ATMApp.Tests/ATMApp.Tests.csproj", new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
        ArgumentCustomization = args => args
            .Append("/p:CollectCoverage=true")
            .Append("/p:CoverletOutputFormat=cobertura")
            .Append(@"/p:Exclude=\[ATMApp].Migrations.*")
            .Append("/p:ExcludeByName=^Program\\.Main$")
    });
});

Task("Report")
    .IsDependentOn("Test")
    .Does(() =>
{
    StartProcess("reportgenerator", new ProcessSettings {
    Arguments = new ProcessArgumentBuilder()
        .Append("-reports:./**/coverage.cobertura.xml")
        .Append("-targetdir:./coverage-report")
        .Append("-reporttypes:Html")
});
});

Task("DocFX")
    .IsDependentOn("Build")
    .Does(() =>
{
    StartProcess("docfx", "docfx.json");
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Report")
    .IsDependentOn("DocFX");

//////////////////////////////////////////////////////
// Run Target
//////////////////////////////////////////////////////
RunTarget(target);
