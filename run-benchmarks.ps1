<#
    .SYNOPSIS 
    Runs the benchmarks.

    .DESCRIPTION
    Runs the benchmarks. 
    The command line output and results from the benchmark are saved in the results folder, 
    in a subfolder named "min-plus-" or "max-plus-" followed by a timestamp of when the benchmark was launched.

#>

[CmdletBinding()]
param (

)

# (min,+) benchmark

Write-Host "Building benchmark"

$minPlusBenchmarkRoot = "$PSScriptRoot/min-plus-convolution-benchmark"
$minPlusBenchmarkProj = "$minPlusBenchmarkRoot/min-plus-convolution-benchmark.csproj"

dotnet publish -c Release $minPlusBenchmarkProj

if($IsWindows) {
    $minPlusBenchmarkExe = "$minPlusBenchmarkRoot/bin/Release/net8.0/publish/min-plus-convolution-benchmark.exe"
}
else {
    $minPlusBenchmarkExe = "$minPlusBenchmarkRoot/bin/Release/net8.0/publish/min-plus-convolution-benchmark"
}

$time = Get-Date -Format "yyyy-MM-dd-HH-mm";
$resultsFolder = "$PSScriptRoot/results/min-plus-$time";
$null = New-Item -ItemType Directory $resultsFolder;

if(-not(Test-Path $resultsFolder)) {
    Write-Host "Critical error: could not make results folder: $resultsFolder"
    exit;
}

Push-Location $resultsFolder
& $minPlusBenchmarkExe --filter "*" 3>&1 2>&1 > "run.log"

Pop-Location

# (max,+) benchmark

# todo