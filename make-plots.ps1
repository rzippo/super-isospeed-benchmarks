<#
    .SYNOPSIS
    Makes plots.

    .DESCRIPTION
    Makes plots for the benchmark results, as .tikz, .pdf and .png files.
    By default, it skips plot files that are already present.
#>

[CmdletBinding()]
param (
    # If set, all plots will be done again, overwriting the existing files.
    [Parameter()]
    [switch]
    $force = $false
)

$benchmarkResultsToTikzRoot = "$PSScriptRoot/benchmark-results-to-tikz";
$benchmarkResultsToTikzCsproj = "$benchmarkResultsToTikzRoot/benchmark-results-to-tikz.csproj";

$dotnetOutput = dotnet publish -c Release $benchmarkResultsToTikzCsproj;

if($IsWindows) {
    $benchmarkResultsToTikzExe = "$benchmarkResultsToTikzRoot/bin/Release/net8.0/publish/benchmark-results-to-tikz.exe"
}
else {
    $benchmarkResultsToTikzExe = "$benchmarkResultsToTikzRoot/bin/Release/net8.0/publish/benchmark-results-to-tikz"
}

$resultsPath = "$PSScriptRoot/results";

$benchmarkResultsFolders = Get-ChildItem -Directory $resultsPath;

foreach ($benchmarkResultsFolder in $benchmarkResultsFolders) 
{
    if($force) {
        & $benchmarkResultsToTikzExe --force $benchmarkResultsFolder
    }
    else {
        & $benchmarkResultsToTikzExe $benchmarkResultsFolder
    }
}

# then convert...