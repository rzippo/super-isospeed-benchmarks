# Super-Isospeed Benchmarks

This repository contains code to replicate the benchmark results in the paper
```
Put your paper here
```

# How to use

Make sure your system meets the requirements, then run the `run-all.ps1` script.
When it's done, you can find the results in 📁`results`.

The default setup of the repository is the exact same one used to produce the plots in the paper.
It may take _quite a while_.
Obviously, your results should be a little different depending on how much your machine differs from ours, but the relative performance of the algorithms should be the same.

To reduce the computation time (but get different results), you can
- **Explain sanity check and how to skip it.**
- **Explain how to reduce the number of convolutions.**
- **Explain how to heuristically reduce the time taken by each convolution.**

You can change the above using 💻 `config-benchmarks.ps1` to the above easily.

# Requirements
- .NET 8.0 SDK
- PowerShell
- LaTeX

All of the above are cross-platform.

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
