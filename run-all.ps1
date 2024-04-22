<#
    .SYNOPSIS 
    Single script to configure benchmarks, run them, then make the plots.

    .DESCRIPTION
    Performs the following steps:
    1. Configures the benchmarks, altering directly their source code.
       If called without parameters it restores the default configuration.
       See config-benchmarks.ps1 for more info on this step

    2. Runs the benchmarks, saving their output in the results folder. 
       See run-benchmarks.ps1 for more info on this step

    3. Makes .tikz plots
       WRITE MORE

    4. Makes .pdf and .png versions of the above
       WRITE MORE

    .EXAMPLE
    PS> ./run-all.ps1
    
    Uses the default configuration.

    .EXAMPLE
    PS> ./run-all.ps1 -sanityCheck $false -numberOfPairs 100 -rngSeed 4321 -rngMaxInteger 100 -largestExtensionsLowerThreshold 50 -largestExtensionsUpperThreshold 500

    Uses the configuration specified.
#>


[CmdletBinding()]
param (
    # If true, a sanity check is run before running the benchmarks.
    # This check uses all the pairs of curves to test that all algorithms (direct, inverse, super-isospeed) compute the same convolution result. 
    [Parameter()]
    [bool]
    $sanityCheck = $true,
    
    # The number of pairs of curves to generate for the benchmark.
    [Parameter()]
    [UInt64]
    $numberOfPairs = 100,
    
    # The seed to use for the random number generation.
    [Parameter()]
    [UInt64]
    $rngSeed = 4321,
    
    # Each time an integer is generated (e.g. numerators and denominators), this is the maximum value used.
    [Parameter()]
    [UInt64]
    $rngMaxInteger = 100,

    # If set greater than 0, when pairs of curves are generated, they are filtered so that the largest between k_{d_f}, k_{d_g}, k_{c_f} and k_{c_g} is above this threshold.
    # This is used to ensure the pair has some significance as runtime to be optimized.
    # However, with a larger threshold, the benchmark runtime may grow too much, becoming impractical.
    [Parameter()]
    [Int64]
    $largestExtensionsLowerThreshold = 50,

    # If set greater than 0, when pairs of curves are generated, they are filtered so that the largest between k_{d_f}, k_{d_g}, k_{c_f} and k_{c_g} is below threshold.
    # This is used to keep the benchmark runtime from growing too much.
    # However, with a larger threshold, more significant examples of performance improvement can be found.
    [Parameter()]
    [Int64]
    $largestExtensionsUpperThreshold = 500,

    # If set greater or equal than 0, all benchmarks are set to run with the given number of warmup runs.
    # If not, BenchmarkDotNet handles it automatically, see https://benchmarkdotnet.org/articles/configs/jobs.html 
    [Parameter()]
    [Int64]
    $warmupCount = -1,
    
    # If set greater than 0, all benchmarks are set to run with the given number of iteration runs.
    # If not, BenchmarkDotNet handles it automatically, see https://benchmarkdotnet.org/articles/configs/jobs.html 
    [Parameter()]
    [Int64]
    $iterationCount = 0
)

.\config-benchmarks.ps1 `
    -sanityCheck $sanityCheck `
    -numberOfPairs $numberOfPairs `
    -rngSeed $rngSeed `
    -rngMaxInteger $rngMaxInteger `
    -largestExtensionsLowerThreshold $largestExtensionsLowerThreshold `
    -largestExtensionsUpperThreshold $largestExtensionsUpperThreshold `
    -warmupCount $warmupCount `
    -iterationCount $iterationCount

.\run-benchmarks.ps1

# do the other steps