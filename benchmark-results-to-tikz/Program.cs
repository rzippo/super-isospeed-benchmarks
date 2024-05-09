// See https://aka.ms/new-console-template for more information

using System.Globalization;
using benchmark_results_model;
using iso_bench_to_tikz;

// constants
var DirectPlotsLabel = "direct";
var InversePlotsLabel = "inverse";
var IsospeedPlotsLabel = "isospeed";
var SuperIsospeedPlotsLabel = "super-isospeed";
var BestPlotsLabel = "best";

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
        var directVsIsospeedTikzPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-direct-isospeed.tikz");
        if (!File.Exists(directVsIsospeedTikzPath) || forceOption)
        {
            var directVsIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.IsospeedBenchmarksLabel);
            var directVsIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
                directVsIsospeedValues, DirectPlotsLabel, IsospeedPlotsLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(directVsIsospeedTikzPath, directVsIsospeedTikzContent);
        }

        var directVsSuperIsospeedPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-direct-superIsospeed.tikz");
        if (!File.Exists(directVsSuperIsospeedPath) || forceOption)
        {
            var directVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.SuperIsospeedBenchmarksLabel);
            var directVsSuperIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
                directVsSuperIsospeedValues, DirectPlotsLabel, SuperIsospeedPlotsLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(directVsSuperIsospeedPath, directVsSuperIsospeedTikzContent);
        }
    }

    if (hasInverse)
    {
        var inverseVsIsospeedPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-inverse-isospeed.tikz");
        if (!File.Exists(inverseVsIsospeedPath) || forceOption)
        {
            var inverseVsIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.InverseBenchmarksLabel, BenchmarkResults.IsospeedBenchmarksLabel);
            var inverseVsIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
                inverseVsIsospeedValues, InversePlotsLabel, IsospeedPlotsLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(inverseVsIsospeedPath, inverseVsIsospeedTikzContent);
        }

        var inverseVsSuperIsospeedPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-inverse-superIsospeed.tikz");
        if (!File.Exists(inverseVsSuperIsospeedPath) || forceOption)
        {
            var inverseVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.InverseBenchmarksLabel, BenchmarkResults.SuperIsospeedBenchmarksLabel);
            var inverseVsSuperIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
                inverseVsSuperIsospeedValues, InversePlotsLabel, SuperIsospeedPlotsLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(inverseVsSuperIsospeedPath, inverseVsSuperIsospeedTikzContent);
        }
    }

    if (hasDirect && hasInverse)
    {
        var inverseVsDirectPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-inverse-direct.tikz");
        if (!File.Exists(inverseVsDirectPath) || forceOption)
        {
            var inverseVsDirectValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.InverseBenchmarksLabel, BenchmarkResults.DirectBenchmarksLabel);
            var inverseVsDirectTikz = TikzPlotter.PlotTikzComparison(
                inverseVsDirectValues, InversePlotsLabel, DirectPlotsLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(inverseVsDirectPath, inverseVsDirectTikz);
        }
    }

    if (hasDirect && hasInverse)
    {
        var isospeedVsBestPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-best-isospeed.tikz");
        if(!File.Exists(isospeedVsBestPath) || forceOption)
        {
            var isospeedVsBestValues = BenchmarkResults.GetComparisonValues_Best(samples, BenchmarkResults.IsospeedBenchmarksLabel,
                new List<string> { BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.InverseBenchmarksLabel });
            var isospeedVsBestTikz = TikzPlotter.PlotTikzComparison(
                isospeedVsBestValues, BestPlotsLabel, IsospeedPlotsLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(isospeedVsBestPath, isospeedVsBestTikz);
        }

        var superIsospeedVsBestPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-best-superIsospeed.tikz");
        if(!File.Exists(superIsospeedVsBestPath) || forceOption)
        {
            var superIsospeedVsBestValues = BenchmarkResults.GetComparisonValues_Best(samples, BenchmarkResults.SuperIsospeedBenchmarksLabel,
                new List<string> { BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.InverseBenchmarksLabel });
            var superIsospeedVsBestTikz = TikzPlotter.PlotTikzComparison(
                superIsospeedVsBestValues, BestPlotsLabel, SuperIsospeedPlotsLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(superIsospeedVsBestPath, superIsospeedVsBestTikz);
        }
    }

    var superIsospeedVsIsospeedPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-superIsospeed-isospeed.tikz");
    if(!File.Exists(superIsospeedVsIsospeedPath) || forceOption)
    {
        var superIsospeedVsIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.IsospeedBenchmarksLabel, BenchmarkResults.SuperIsospeedBenchmarksLabel);
        var superIsospeedVsIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
            superIsospeedVsIsospeedValues,IsospeedPlotsLabel, SuperIsospeedPlotsLabel,
            new TikzPlotterSettings { FontSize = FontSize.tiny });
        File.WriteAllText(superIsospeedVsIsospeedPath, superIsospeedVsIsospeedTikzContent);
    }
}
