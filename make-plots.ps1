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

$tikzToPdfRoot = "$PSScriptRoot/tikz-to-pdf/tikz-to-pdf";
$tikzToPdfCsproj = "$tikzToPdfRoot/tikz-to-pdf.csproj";

$dotnetOutput = dotnet publish -c Release $tikzToPdfCsproj;

if($IsWindows) {
    $tikzToPdfExe = "$tikzToPdfRoot/bin/Release/net8.0/publish/tikz-to-pdf.exe"
}
else {
    $tikzToPdfExe = "$tikzToPdfRoot/bin/Release/net8.0/publish/tikz-to-pdf"
}

$tikzFiles = Get-ChildItem -Recurse -Path $resultsPath -Filter "*.tikz";
$tikzFilesCounter = 0;
$tikzFilesTotal = $tikzFiles.Length;
foreach ($tikzFile in $tikzFiles) 
{
    Write-Progress -Activity "Converting .tikz to .pdf and .png" -Status "$tikzFilesCounter of $tikzFilesTotal" -PercentComplete ($tikzFilesCounter / $tikzFilesTotal);
    $tikzFilesCounter++;

    $pdfFile = "$tikzFile.pdf"
    $pngFile = "$tikzFile.png"

    if($force) {
        $skip = $false
    } 
    else {
        $skip = (Test-Path $pdfFile) -and (Test-Path $pngFile);
    }

    if(-not $skip)
    {
        & $tikzToPdfExe $tikzFile --png
    }
}