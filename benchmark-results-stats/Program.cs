// See https://aka.ms/new-console-template for more information

using System.Globalization;
using benchmark_results_model;

// constants
var DirectTableLabel = "direct";
var InverseTableLabel = "inverse";
var IsospeedTableLabel = "isospeed";
var SuperIsospeedTableLabel = "super-isospeed";
var BestTableLabel = "best";

var percentiles = new List<int>{ 10, 25, 50, 75, 90 };

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

var resultsDirectoryPath = ".";
var forceOption = false;

foreach (var arg in args)
{
    if (arg.StartsWith("--"))
    {
        switch (arg)
        {
            case "--force":
                forceOption = true;
                break;

            default:
                Console.WriteLine($"Option {arg} not recognized.");
                System.Environment.Exit(1);
                break;
        }
    }
    else
    {
        if(Directory.Exists(arg))
            resultsDirectoryPath = arg;
        else
        {
            Console.WriteLine($"Not a path to a folder: {arg}");
            System.Environment.Exit(1);
            break;
        }
    }
}

var csvPaths = Directory.GetFiles(resultsDirectoryPath, "*-report.csv");

foreach (var csvPath in csvPaths)
{
    if (!File.Exists(csvPath))
        throw new InvalidOperationException($"File \"{csvPath}\" does not exist!");

    var benchmarkName = Path.GetFileName(csvPath).Replace("-report.csv", "");

    var samples = BenchmarkResults.ParseCsv(csvPath);

    var hasDirect = samples.Any(s => s.Method == BenchmarkResults.DirectBenchmarksLabel);
    var hasInverse = samples.Any(s => s.Method == BenchmarkResults.InverseBenchmarksLabel);

    if (hasDirect)
    {
        var directVsIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.IsospeedBenchmarksLabel);
        var directVsIsospeedPercentileValues = SpeedupsTable.GetSpeedupsPercentiles(directVsIsospeedValues, percentiles);
        var directVsIsospeedPercentilesString = string.Join("; ", directVsIsospeedPercentileValues);
        Console.WriteLine($"{benchmarkName}; {DirectTableLabel}; {IsospeedTableLabel}; {directVsIsospeedPercentilesString}");

        var directVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.SuperIsospeedBenchmarksLabel);
        var directVsSuperIsospeedPercentileValues = SpeedupsTable.GetSpeedupsPercentiles(directVsSuperIsospeedValues, percentiles);
        var directVsSuperIsospeedPercentilesString = string.Join("; ", directVsSuperIsospeedPercentileValues);
        Console.WriteLine($"{benchmarkName}; {DirectTableLabel}; {SuperIsospeedTableLabel}; {directVsSuperIsospeedPercentilesString}");
    }

    if (hasInverse)
    {
        var inverseVsIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.InverseBenchmarksLabel, BenchmarkResults.IsospeedBenchmarksLabel);
        var inverseVsIsospeedPercentileValues = SpeedupsTable.GetSpeedupsPercentiles(inverseVsIsospeedValues, percentiles);
        var inverseVsIsospeedPercentilesString = string.Join("; ", inverseVsIsospeedPercentileValues);
        Console.WriteLine($"{benchmarkName}; {InverseTableLabel}; {IsospeedTableLabel}; {inverseVsIsospeedPercentilesString}");

        var inverseVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.InverseBenchmarksLabel, BenchmarkResults.SuperIsospeedBenchmarksLabel);
        var inverseVsSuperIsospeedPercentileValues = SpeedupsTable.GetSpeedupsPercentiles(inverseVsSuperIsospeedValues, percentiles);
        var inverseVsSuperIsospeedPercentilesString = string.Join("; ", inverseVsSuperIsospeedPercentileValues);
        Console.WriteLine($"{benchmarkName}; {InverseTableLabel}; {SuperIsospeedTableLabel}; {inverseVsSuperIsospeedPercentilesString}");
    }

    if (hasDirect && hasInverse)
    {
        var inverseVsDirectValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.InverseBenchmarksLabel, BenchmarkResults.DirectBenchmarksLabel);
        var inverseVsDirectPercentileValues = SpeedupsTable.GetSpeedupsPercentiles(inverseVsDirectValues, percentiles);
        var inverseVsDirectPercentilesString = string.Join("; ", inverseVsDirectPercentileValues);
        Console.WriteLine($"{benchmarkName}; {InverseTableLabel}; {DirectTableLabel}; {inverseVsDirectPercentilesString}");
    }

    if (hasDirect && hasInverse)
    {
        var bestVsIsospeedValues = BenchmarkResults.GetComparisonValues_Best(samples, BenchmarkResults.IsospeedBenchmarksLabel,
            new List<string> { BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.InverseBenchmarksLabel });
        var bestVsIsospeedPercentileValues = SpeedupsTable.GetSpeedupsPercentiles(bestVsIsospeedValues, percentiles);
        var bestVsIsospeedPercentilesString = string.Join("; ", bestVsIsospeedPercentileValues);
        Console.WriteLine($"{benchmarkName}; {BestTableLabel}; {IsospeedTableLabel}; {bestVsIsospeedPercentilesString}");

        var bestVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues_Best(samples, BenchmarkResults.SuperIsospeedBenchmarksLabel,
            new List<string> { BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.InverseBenchmarksLabel });
        var bestVsSuperIsospeedPercentileValues = SpeedupsTable.GetSpeedupsPercentiles(bestVsSuperIsospeedValues, percentiles);
        var bestVsSuperIsospeedPercentilesString = string.Join("; ", bestVsSuperIsospeedPercentileValues);
        Console.WriteLine($"{benchmarkName}; {BestTableLabel}; {SuperIsospeedTableLabel}; {bestVsSuperIsospeedPercentilesString}");
    }

    var isospeedVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.IsospeedBenchmarksLabel, BenchmarkResults.SuperIsospeedBenchmarksLabel);
    var isospeedVsSuperIsospeedPercentileValues = SpeedupsTable.GetSpeedupsPercentiles(isospeedVsSuperIsospeedValues, percentiles);
    var isospeedVsSuperIsospeedPercentilesString = string.Join("; ", isospeedVsSuperIsospeedPercentileValues);
    Console.WriteLine($"{benchmarkName}; {IsospeedTableLabel}; {SuperIsospeedTableLabel}; {isospeedVsSuperIsospeedPercentilesString}");
}
