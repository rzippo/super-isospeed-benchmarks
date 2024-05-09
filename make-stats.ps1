<#
    .SYNOPSIS
    Makes stats tables.

    .DESCRIPTION
    Makes tables showing the speedups obtained in the benchmarks, as .csv files.
#>

[CmdletBinding()]
param (
)

$ErrorActionPreference = "Stop";

Write-Host " -- Making tables from results";
Write-Host " --- Making .csv tables";

$benchmarkResultsStatsRoot = "$PSScriptRoot/benchmark-results-stats";
$benchmarkResultsStatsCsproj = "$benchmarkResultsStatsRoot/benchmark-results-stats.csproj";

$dotnetOutput = dotnet publish -c Release $benchmarkResultsStatsCsproj;

if($IsWindows) {
    $benchmarkResultsStatsExe = "$benchmarkResultsStatsRoot/bin/Release/net8.0/publish/benchmark-results-stats.exe"
}
else {
    $benchmarkResultsStatsExe = "$benchmarkResultsStatsRoot/bin/Release/net8.0/publish/benchmark-results-stats"
}

$resultsPath = "$PSScriptRoot/results";

$benchmarkResultsFolders = Get-ChildItem -Directory $resultsPath;

foreach ($benchmarkResultsFolder in $benchmarkResultsFolders) 
{
    if($force) {
        & $benchmarkResultsStatsExe --force $benchmarkResultsFolder
    }
    else {
        & $benchmarkResultsStatsExe $benchmarkResultsFolder
    }
}
