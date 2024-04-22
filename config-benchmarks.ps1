<#
    .SYNOPSIS 
    Configures the benchmarks.

    .DESCRIPTION
    Configures the benchmarks, altering directly their source code.
    If called without parameters it restores the default configuration.

    .EXAMPLE
    PS> ./config-benchmarks.ps1
    
    Restores the default configuration.

    .EXAMPLE
    PS> ./config-benchmarks.ps1 -sanityCheck $false -numberOfPairs 100 -rngSeed 4321 -rngMaxInteger 100 -largestExtensionsLowerThreshold 50 -largestExtensionsUpperThreshold 500

    Sets the specified parameters.
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

Write-Host "sanityCheck set to $sanityCheck"
Write-Host "numberOfPairs set to $numberOfPairs"
Write-Host "rngSeed set to $rngSeed"
Write-Host "rngMaxInteger set to $rngMaxInteger"
Write-Host "largestExtensionsLowerThreshold set to $largestExtensionsLowerThreshold"
Write-Host "largestExtensionsUpperThreshold set to $largestExtensionsUpperThreshold"
Write-Host "warmupCount set to $warmupCount"
Write-Host "iterationCount set to $iterationCount"

$minPlusCsprojPath = "$PSScriptRoot/min-plus-convolution-benchmark/min-plus-convolution-benchmark.csproj"
$minPlusProgramPath = "$PSScriptRoot/min-plus-convolution-benchmark/Program.cs"

$maxPlusCsprojPath = "$PSScriptRoot/max-plus-convolution-benchmark/max-plus-convolution-benchmark.csproj"
$maxPlusProgramPath = "$PSScriptRoot/max-plus-convolution-benchmark/Program.cs"

function setSanityCheck($csprojPath)
{
    if($sanityCheck)
    {
        $csprojContent = (Get-Content -Raw $csprojPath);
        if($csprojContent -match '<DefineConstants></DefineConstants>')
        {
            $csprojContent = $csprojContent -replace '<DefineConstants></DefineConstants>','<DefineConstants>SANITY_CHECK</DefineConstants>';
            $csprojContent | Out-File -NoNewline $csprojPath;
        }
        else
        {
            if(-not ($csprojContent -match '(?<=\W)SANITY_CHECK(?=\W)'))
            {
                $csprojContent = $csprojContent -replace '<DefineConstants>(.+)</DefineConstants>','<DefineConstants>SANITY_CHECK,$1</DefineConstants>';
                $csprojContent | Out-File -NoNewline $csprojPath;
            }
        }
    }
    else
    {
        $csprojContent = (Get-Content -Raw $csprojPath);
        $csprojContent = $csprojContent -replace '(?<=\W)SANITY_CHECK(?=\W)', '';
        $csprojContent = $csprojContent -replace ',,',',';
        $csprojContent = $csprojContent -replace '<DefineConstants>,','<DefineConstants>';
        
        $csprojContent | Out-File -NoNewline $csprojPath;
    }    
}

function setNumberOfPairs($programPath)
{
    $programContent = (Get-Content -Raw $programPath);
    $programContent = $programContent -replace 'public const int TEST_COUNT = \d+;',"public const int TEST_COUNT = $numberOfPairs;"
    $programContent | Out-File -NoNewline $programPath;
}

function setRngSeed($programPath)
{
    $programContent = (Get-Content -Raw $programPath);
    $programContent = $programContent -replace 'public const int RNG_SEED = \d+;',"public const int RNG_SEED = $rngSeed;"
    $programContent | Out-File -NoNewline $programPath;
}

function setRngMaxInteger($programPath)
{
    $programContent = (Get-Content -Raw $programPath);
    $programContent = $programContent -replace 'public const int RNG_MAX = \d+;',"public const int RNG_MAX = $rngMaxInteger;"
    $programContent | Out-File -NoNewline $programPath;
}

function setLargestExtensionsThresholds($programPath)
{
    $programContent = (Get-Content -Raw $programPath);
    if(($largestExtensionsLowerThreshold -gt 0) -and ($largestExtensionsUpperThreshold -gt 0))
    {
        # both filters enabled
        $programContent = $programContent -replace 'public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = [\w\.]+;',"public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = FILTER_EXTENSION_ENUM.FILTER_BOTH;"
        $programContent = $programContent -replace 'public const int LARGE_EXTENSION_LCM_LOWER_THRESHOLD = \d+;',"public const int LARGE_EXTENSION_LCM_LOWER_THRESHOLD = $largestExtensionsLowerThreshold;"
        $programContent = $programContent -replace 'public const int LARGE_EXTENSION_LCM_UPPER_THRESHOLD = \d+;',"public const int LARGE_EXTENSION_LCM_UPPER_THRESHOLD = $largestExtensionsUpperThreshold;"
    }
    elseif ($largestExtensionsLowerThreshold -gt 0) 
    {
        # only lower threshold enabled
        $programContent = $programContent -replace 'public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = [\w\.]+;',"public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = FILTER_EXTENSION_ENUM.FILTER_SMALLER;"
        $programContent = $programContent -replace 'public const int LARGE_EXTENSION_LCM_LOWER_THRESHOLD = \d+;',"public const int LARGE_EXTENSION_LCM_LOWER_THRESHOLD = $largestExtensionsLowerThreshold;"
    }
    elseif ($largestExtensionsUpperThreshold -gt 0)
    {
        # only upper threshold enabled
        $programContent = $programContent -replace 'public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = [\w\.]+;',"public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = FILTER_EXTENSION_ENUM.FILTER_LARGER;"
        $programContent = $programContent -replace 'public const int LARGE_EXTENSION_LCM_LOWER_THRESHOLD = \d+;',"public const int LARGE_EXTENSION_LCM_LOWER_THRESHOLD = $largestExtensionsLowerThreshold;"
    }
    else
    {
        # does not filter
        $programContent = $programContent -replace 'public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = [\w\.]+;',"public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = FILTER_EXTENSION_ENUM.NO_FILTER;"
    }
    $programContent | Out-File -NoNewline $programPath;
}

function setJobs($programPath)
{
    $programContent = (Get-Content -Raw $programPath);
    $jobLine;
    if(($warmupCount -lt 0) -and ($iterationCount -le 0))
    {
        # BenchmarkDotNet handles both automatically
        $jobLine = "[SimpleJob()]"
    }
    elseif ($warmupCount -lt 0) {
        # BenchmarkDotNet handles warmup automatically, but not iterations
        $jobLine = "[SimpleJob(iterationCount: $iterationCount)]"
    }
    elseif ($iterationCount -le 0) {
        # BenchmarkDotNet handles iterations automatically, but not warmup
        $jobLine = "[SimpleJob(warmupCount: $warmupCount)]"
    }
    else {
        # Both warmup and iterations are specified by the user
        $jobLine = "[SimpleJob(warmupCount: $warmupCount, iterationCount: $iterationCount)]"
    }
    $programContent = $programContent -replace '\[SimpleJob(?:\(.*?\))?\]',$jobLine ;
    $programContent | Out-File -NoNewline $programPath;
}

setSanityCheck $minPlusCsprojPath;
setNumberOfPairs $minPlusProgramPath;
setRngSeed $minPlusProgramPath;
setRngMaxInteger $minPlusProgramPath;
setLargestExtensionsThresholds $minPlusProgramPath;
setJobs $minPlusProgramPath;

# Skip (max,+) until Program.cs is updated

setSanityCheck $maxPlusCsprojPath;
setNumberOfPairs $maxPlusProgramPath;
setRngSeed $maxPlusProgramPath;
setRngMaxInteger $maxPlusProgramPath;
setLargestExtensionsThresholds $maxPlusProgramPath;
setJobs $maxPlusProgramPath;