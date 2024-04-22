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

function runBenchmark($name, $shortname)
{
    Write-Host " --- Building $name"

    $benchmarkRoot = "$PSScriptRoot/$name"
    $benchmarkProj = "$benchmarkRoot/$name.csproj"

    $dotnetOutput = dotnet publish -c Release $benchmarkProj;

    if($IsWindows) {
        $benchmarkExe = "$benchmarkRoot/bin/Release/net8.0/publish/$name.exe"
    }
    else {
        $benchmarkExe = "$benchmarkRoot/bin/Release/net8.0/publish/$name"
    }

    $time = Get-Date -Format "yyyy-MM-dd-HH-mm";
    $resultsFolder = "$PSScriptRoot/results/$shortname-$time";
    $null = New-Item -ItemType Directory $resultsFolder;

    if(-not(Test-Path $resultsFolder)) {
        Write-Host "Critical error: could not make results folder: $resultsFolder"
        exit;
    }

    Push-Location $resultsFolder
    Write-Host " --- Launching $name"
    & $benchmarkExe --filter "*" 3>&1 2>&1 > "run.log"

    $benchmarkDotNetLog = Get-ChildItem -Path "$resultsFolder/BenchmarkDotNet.Artifacts/" -Filter "BenchmarkRun*.log";
    $benchmarkDotNetLog | Move-Item -Destination $resultsFolder;

    $benchmarkDotNetOutput = Get-ChildItem -Path "$resultsFolder/BenchmarkDotNet.Artifacts/results/";
    $benchmarkDotNetOutput | Move-Item -Destination $resultsFolder;

    Remove-Item -Recurse "$resultsFolder/BenchmarkDotNet.Artifacts/"

    Pop-Location
}

Write-Host " -- Running (min,+) convolution benchmark";
runBenchmark "min-plus-convolution-benchmark" "min-plus"

Write-Host " -- Running (max,+) convolution benchmark";
runBenchmark "max-plus-convolution-benchmark" "max-plus"
