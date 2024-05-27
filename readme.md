# Super-Isospeed Benchmarks

This repository contains code to replicate the benchmark results in the paper
```
Put your paper here
```

# How to use

Make sure your system meets the requirements, then run the `run-all.ps1` script.
It will run all the computations and, when it's done, you can find the results in 📁`results`.

The default setup of the repository is the exact same one used to produce the plots in the paper.

It may take _quite a while_: on our machine, it took 25 days to run the benchmarks shown in the paper.
Obviously, your results should be a little different depending on how much your machine differs from ours, but the relative performance of the algorithms should be the same.

The "raw" results from our benchmarks are already in 📁`results`.
You can check the output from [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) and, to produce the tables and plots shown in the table, run
```pwsh
  make-stats.ps1
  make-plots.ps1
```

## Reducing the computation time

If you would like to run similar but cheaper benchmarks, there are different things you can do to reduce the computation time, at the cost of having less precise or less significant results.

- **Number of pairs**. 
  In the default configuration, for each scenario we generate 100 pairs of curves for the benchmark. 
  You can tune this number with the `-numberOfPairs` option.

- **Number of repetitions**.
  In the default configuration, the [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) framework will automatically determine how many times to run each convolution in order to accurately measure its runtime, removing noise (e.g. garbage collection, just in time compilation) and statistical errors.
  You can disable this behavior with the `-warmupCount` and `-iterationCount` options, which allow you to set the number of repetitions.
  
- **Heuristic for computation size**.
  We use an heuristic to avoid computations that are too small or too large.
  For each generated pair, we compute its largest extension multiplier, and if it is below `-largestExtensionsLowerThreshold` (by default, set to 50) or above `-largestExtensionsUpperThreshold` (by default, set to 500) the pair is discarded.

- **Sanity check**.
  In the event the implementation had any bugs left, we run a sanity check before the benchmark phase.
  This check computes each convolution with the different methods, and checks that they all return the same result.
  You can control whether this check is performed or not with the `-sanityCheck` option.

You can change the above using 💻 `config-benchmarks.ps1` before 💻 `run-benchmarks.ps1`, or directly through ✨ `run-all.ps1`.
For example:
```pwsh
./run-all.ps1 -numberOfPairs 10 -warmupCount 0 -iterationCount 1 -largestExtensionsLowerThreshold 5 -largestExtensionsUpperThreshold 30 -sanityCheck $false
```
This command should run for 30 minutes at most, and from its results you should be able to see improvements up to 10x.

# Requirements
- .NET 8.0 SDK
- PowerShell
- LaTeX

All of the above are cross-platform. 

This repository also provides a [devcontainer](https://code.visualstudio.com/docs/devcontainers/containers) that has all of the above installed and configured.

# Repository structure

## Folders
- 📁 `benchmark-results-to-tikz`
    - Utility that parses the benchmark results to build `.tikz` files for the graphs shown in the paper.
- 📁 `nancy `
    - The Nancy library used in the benchmarks. 
      The code is based on public version 1.1.8 (1f83e099) + the changes implementing *super-isospeed*.
    - Its public repository is https://github.com/rzippo/nancy.
- 📁 `max-plus-convolution-benchmark `
    - Benchmark for (max,+) convolution.
- 📁 `min-plus-convolution-benchmark `
    - Benchmark for (min,+) convolution.
- 📁 `results`
    - Results of the benchmarks and their plots will be put here.
- 📁 `tikz-to-pdf`
    - Utility that renders `.tikz` file to `.pdf` and `.png`. Requires `pdflatex` to be available on the system.
    - Its public repository is https://github.com/rzippo/tikz-to-pdf.

## Scripts

The scripts below are all PowerShell scripts, and need to be launched from a PowerShell shell.

> Use `Get-Help name-of-the-script.ps1` to see the documentation.

- 💻 `config-benchmarks.ps1`
  - Configures the benchmarks. Running it with no args restores the default config, which is the same used in the paper.
- 💻 `run-benchmarks.ps1`
  - Runs all benchmarks, saving the results in 📁 `results`.
- 💻 `make-stats.ps1`
  - Makes `speedups.csv` tables for all results in 📁 `results`.
- 💻 `make-plots.ps1`
  - Makes `.tikz` plots for all results in 📁 `results`. 
  - Will leave `.tikz`, `.pdf` and `.png` files in the same folder as the benchmark they refer to.
- ✨ `run-all.ps1`
  - Run benchmarks and make plots, all in a single command.

## Other

- 📄 `license.md`
  - All the code in this project is under MIT license. Do what you want with it.
- 📄 `readme.md`
  - The text that you are reading.
- 📄 `super-isospeed-benchmarks.sln`
  - .NET solution file that references the other projects in the repository.
