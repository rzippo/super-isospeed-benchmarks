param (
    [Parameter()]
    [bool]
    $sanityCheck = $true,
    [Parameter()]
    [UInt64]
    $numberOfPairs = 100,
    [Parameter()]
    [UInt64]
    $rngSeed = 4321,
    [Parameter()]
    [UInt64]
    $rngMaxInteger = 100,
    [Parameter()]
    [UInt64]
    $largestExtensionsThreshold = 500
)

Write-Host "sanityCheck set to $sanityCheck"
Write-Host "numberOfPairs set to $numberOfPairs"
Write-Host "rngSeed set to $rngSeed"
Write-Host "rngMaxInteger set to $rngMaxInteger"
Write-Host "largestExtensionsThreshold set to $largestExtensionsThreshold"

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
            $csprojContent | Out-File $csprojPath;
        }
        else
        {
            if(-not ($csprojContent -match '(?<=\W)SANITY_CHECK(?=\W)'))
            {
                $csprojContent = $csprojContent -replace '<DefineConstants>(.+)</DefineConstants>','<DefineConstants>SANITY_CHECK,$1</DefineConstants>';
                $csprojContent | Out-File $csprojPath;
            }
        }
    }
    else
    {
        $csprojContent = (Get-Content -Raw $csprojPath);
        $csprojContent = $csprojContent -replace '(?<=\W)SANITY_CHECK(?=\W)', '';
        $csprojContent = $csprojContent -replace ',,',',';
        $csprojContent = $csprojContent -replace '<DefineConstants>,','<DefineConstants>';
        
        $csprojContent | Out-File $csprojPath;
    }    
}

function setNumberOfPairs($programPath)
{
    $programContent = (Get-Content -Raw $programPath);
    $programContent = $programContent -replace 'public const int TEST_COUNT = \d+;',"public const int TEST_COUNT = $numberOfPairs;"
    $programContent | Out-File $programPath;
}

function setRngSeed($programPath)
{
    $programContent = (Get-Content -Raw $programPath);
    $programContent = $programContent -replace 'public const int RNG_SEED = \d+;',"public const int RNG_SEED = $rngSeed;"
    $programContent | Out-File $programPath;
}

function setRngMaxInteger($programPath)
{
    $programContent = (Get-Content -Raw $programPath);
    $programContent = $programContent -replace 'public const int RNG_MAX = \d+;',"public const int RNG_MAX = $rngMaxInteger;"
    $programContent | Out-File $programPath;
}

function setLargestExtensionsThreshold($programPath)
{
    $programContent = (Get-Content -Raw $programPath);
    $programContent = $programContent -replace 'public const int LARGE_EXTENSION_LCM_THRESHOLD = \d+;',"public const int LARGE_EXTENSION_LCM_THRESHOLD = $largestExtensionsThreshold;"
    $programContent | Out-File $programPath;
}

setSanityCheck $minPlusCsprojPath;
setNumberOfPairs $minPlusProgramPath;
setRngSeed $minPlusProgramPath;
setRngMaxInteger $minPlusProgramPath;
setLargestExtensionsThreshold $minPlusProgramPath;

# Skip (max,+) until Program.cs is updated

# setSanityCheck $maxPlusCsprojPath;
