// See https://aka.ms/new-console-template for more information

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using iso_bench_to_tikz;

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

    // constants
    var DirectMethod = "Direct";
    var InverseMethod = "Inverse";
    var IsospeedMethod = "Isospeed";
    var SuperIsospeedMethod = "SuperIsospeed";

    var DirectMethodLabel = "direct";
    var InverseMethodLabel = "inverse";
    var IsospeedMethodLabel = "isospeed";
    var SuperIsospeedMethodLabel = "super-isospeed";
    var BestMethodLabel = "best";

    using var reader = new StreamReader(csvPath);

    var delimiter = File.ReadLines(csvPath).First().StartsWith("Method,") ? "," : ";";
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = delimiter
    };
    using var csv = new CsvReader(reader, config);

    var samples = csv.GetRecords<BenchmarkResult>().ToList();
    var hasDirect = samples.Any(s => s.Method == DirectMethod);
    var hasInverse = samples.Any(s => s.Method == InverseMethod);

    if (hasDirect)
    {
        var directVsIsospeedTikzPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-direct-isospeed.tikz");
        if (!File.Exists(directVsIsospeedTikzPath) || forceOption)
        {
            var directVsIsospeedValues = GetComparisonValues(samples, DirectMethod, IsospeedMethod);
            var directVsIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
                directVsIsospeedValues, DirectMethodLabel, IsospeedMethodLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(directVsIsospeedTikzPath, directVsIsospeedTikzContent);
        }

        var directVsSuperIsospeedPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-direct-superIsospeed.tikz");
        if (!File.Exists(directVsSuperIsospeedPath) || forceOption)
        {
            var directVsSuperIsospeedValues = GetComparisonValues(samples, DirectMethod, SuperIsospeedMethod);
            var directVsSuperIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
                directVsSuperIsospeedValues, DirectMethodLabel, SuperIsospeedMethodLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(directVsSuperIsospeedPath, directVsSuperIsospeedTikzContent);
        }
    }

    if (hasInverse)
    {
        var inverseVsIsospeedPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-inverse-isospeed.tikz");
        if (!File.Exists(inverseVsIsospeedPath) || forceOption)
        {
            var inverseVsIsospeedValues = GetComparisonValues(samples, InverseMethod, IsospeedMethod);
            var inverseVsIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
                inverseVsIsospeedValues, InverseMethodLabel, IsospeedMethodLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(inverseVsIsospeedPath, inverseVsIsospeedTikzContent);
        }

        var inverseVsSuperIsospeedPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-inverse-superIsospeed.tikz");
        if (!File.Exists(inverseVsSuperIsospeedPath) || forceOption)
        {
            var inverseVsSuperIsospeedValues = GetComparisonValues(samples, InverseMethod, SuperIsospeedMethod);
            var inverseVsSuperIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
                inverseVsSuperIsospeedValues, InverseMethodLabel, SuperIsospeedMethodLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(inverseVsSuperIsospeedPath, inverseVsSuperIsospeedTikzContent);
        }
    }

    if (hasDirect && hasInverse)
    {
        var inverseVsDirectPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-inverse-direct.tikz");
        if (!File.Exists(inverseVsDirectPath) || forceOption)
        {
            var inverseVsDirectValues = GetComparisonValues(samples, InverseMethod, DirectMethod);
            var inverseVsDirectTikz = TikzPlotter.PlotTikzComparison(
                inverseVsDirectValues, InverseMethodLabel, DirectMethodLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(inverseVsDirectPath, inverseVsDirectTikz);
        }
    }

    if (hasDirect && hasInverse)
    {
        var isospeedVsBestPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-best-isospeed.tikz");
        if(!File.Exists(isospeedVsBestPath) || forceOption)
        {
            var isospeedVsBestValues = GetComparisonValues_Best(samples, IsospeedMethod,
                new List<string> { DirectMethod, InverseMethod });
            var isospeedVsBestTikz = TikzPlotter.PlotTikzComparison(
                isospeedVsBestValues, BestMethodLabel, IsospeedMethodLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(isospeedVsBestPath, isospeedVsBestTikz);
        }

        var superIsospeedVsBestPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-best-superIsospeed.tikz");
        if(!File.Exists(superIsospeedVsBestPath) || forceOption)
        {
            var superIsospeedVsBestValues = GetComparisonValues_Best(samples, SuperIsospeedMethod,
                new List<string> { DirectMethod, InverseMethod });
            var superIsospeedVsBestTikz = TikzPlotter.PlotTikzComparison(
                superIsospeedVsBestValues, BestMethodLabel, SuperIsospeedMethodLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            File.WriteAllText(superIsospeedVsBestPath, superIsospeedVsBestTikz);
        }
    }

    var superIsospeedVsIsospeedPath = Path.Combine(resultsDirectoryPath, $"{benchmarkName}-superIsospeed-isospeed.tikz");
    if(!File.Exists(superIsospeedVsIsospeedPath) || forceOption)
    {
        var superIsospeedVsIsospeedValues = GetComparisonValues(samples, IsospeedMethod, SuperIsospeedMethod);
        var superIsospeedVsIsospeedTikzContent = TikzPlotter.PlotTikzComparison(
            superIsospeedVsIsospeedValues,IsospeedMethodLabel, SuperIsospeedMethodLabel,
            new TikzPlotterSettings { FontSize = FontSize.tiny });
        File.WriteAllText(superIsospeedVsIsospeedPath, superIsospeedVsIsospeedTikzContent);
    }
}

List<(decimal x, decimal y)> GetComparisonValues(List<BenchmarkResult> samples, string method1, string method2)
{
    var samplesByTestCase = samples.GroupBy(br => br.Pair);
    var comparisonValues = samplesByTestCase
        .Select(g =>
        {
            var method1Sample = g.First(br => br.Method == method1)!;
            var m1TimeString = method1Sample.Median ?? method1Sample.Mean;
            var m1Time = TimeStringToDecimal(m1TimeString);

            var method2Sample = g.First(br => br.Method == method2)!;
            var m2TimeString = method2Sample.Median ?? method2Sample.Mean;
            var m2Time = TimeStringToDecimal(m2TimeString);

            return (x: m1Time, y: m2Time);
        })
        .ToList();
    return comparisonValues;
}

List<(decimal x, decimal y)> GetComparisonValues_Best(List<BenchmarkResult> samples, string method1, List<string> otherMethods)
{
    var samplesByTestCase = samples.GroupBy(br => br.Pair);
    var comparisonValues = samplesByTestCase
        .Select(g =>
        {
            var method1Sample = g.First(br => br.Method == method1)!;
            var m1TimeString = method1Sample.Median ?? method1Sample.Mean;
            var m1Time = TimeStringToDecimal(m1TimeString);

            var otherSamples = otherMethods.Select(m => {
                var mSample = g.First(br => br.Method == m)!;
                var mTimeString = mSample.Median ?? mSample.Mean;
                var mTime = TimeStringToDecimal(mTimeString);
                return mTime;
            });
            return (x: otherSamples.Min(), y: m1Time);
        })
        .ToList();
    return comparisonValues;
}

decimal TimeStringToDecimal(string s)
{
    var raw = decimal.Parse(s.Split(" ")[0]);
    return s.EndsWith(" μs") ? raw / 1000:
        s.EndsWith(" ms") ? raw :
        s.EndsWith(" s") ? raw * 1_000 :
        throw new InvalidOperationException($"Unrecognized unit: {s}");
}
