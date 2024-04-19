// See https://aka.ms/new-console-template for more information

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using iso_bench_to_tikz;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

// ParseMinPlusIsoBenchmark("./minp-2024-02-23");
// ParseMinPlusIsoBenchmark("./minp-2024-02-23-2");
// ParseMinPlusIsoBenchmark("./minp-2024-02-26");
ParseMinPlusIsoBenchmark("./minp-2024-02-29");

// ParseMaxPlusIsoBenchmark("./maxp-2023-03-29");

void ParseMinPlusIsoBenchmark(string resultsPath)
{
    var DirectMethod = "Direct";
    var InverseMethod = "Inverse";
    var IsospeedMethod = "Isospeed";
    var SuperIsospeedMethod = "SuperIsospeed";

    var DirectMethodLabel = "direct";
    var InverseMethodLabel = "inverse";
    var IsospeedMethodLabel = "isospeed";
    var SuperIsospeedMethodLabel = "super-isospeed";
    var BestMethodLabel = "best";

    ComparisonByCurveType("IsoConvolutionBalancedStaircaseBenchmarks");
    ComparisonByCurveType("IsoConvolutionHorizontalStaircaseBenchmarks");
    ComparisonByCurveType("IsoConvolutionVerticalStaircaseBenchmarks");
    ComparisonByCurveType("IsoConvolutionHorizontalKTradeoffStaircaseBenchmarks");

    void ComparisonByCurveType(string type)
    {
        var csvPath = Path.Combine(resultsPath, $"{type}-report.csv");
        if (!File.Exists(csvPath))
            return;

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
            var directVsIsospeedValues = GetComparisonValues(samples, DirectMethod, IsospeedMethod);
            var directVsIsospeedTikz = TikzPlotter.PlotTikzComparison(
                directVsIsospeedValues, DirectMethodLabel, IsospeedMethodLabel,
                new TikzPlotterSettings{ FontSize = FontSize.tiny });
            var directVsIsospeedOutTikz = Path.Combine(resultsPath, $"{type}-direct-isospeed.tikz");
            File.WriteAllText(directVsIsospeedOutTikz, directVsIsospeedTikz);

            var directVsSuperIsospeedValues = GetComparisonValues(samples, DirectMethod, SuperIsospeedMethod);
            var directVsSuperIsospeedTikz = TikzPlotter.PlotTikzComparison(
                directVsSuperIsospeedValues, DirectMethodLabel, SuperIsospeedMethodLabel,
                new TikzPlotterSettings{ FontSize = FontSize.tiny });
            var directVsSuperIsospeedOutTikz = Path.Combine(resultsPath, $"{type}-direct-superIsospeed.tikz");
            File.WriteAllText(directVsSuperIsospeedOutTikz, directVsSuperIsospeedTikz);
        }

        if (hasInverse)
        {
            var inverseVsIsospeedValues = GetComparisonValues(samples, InverseMethod, IsospeedMethod);
            var inverseVsIsospeedTikz = TikzPlotter.PlotTikzComparison(
                inverseVsIsospeedValues, InverseMethodLabel, IsospeedMethodLabel,
                new TikzPlotterSettings{ FontSize = FontSize.tiny });
            var inverseVsIsospeedOutTikz = Path.Combine(resultsPath, $"{type}-inverse-isospeed.tikz");
            File.WriteAllText(inverseVsIsospeedOutTikz, inverseVsIsospeedTikz);

            var inverseVsSuperIsospeedValues = GetComparisonValues(samples, InverseMethod, SuperIsospeedMethod);
            var inverseVsSuperIsospeedTikz = TikzPlotter.PlotTikzComparison(
                inverseVsSuperIsospeedValues, InverseMethodLabel, SuperIsospeedMethodLabel,
                new TikzPlotterSettings{ FontSize = FontSize.tiny });
            var inverseVsSuperIsospeedOutTikz = Path.Combine(resultsPath, $"{type}-inverse-superIsospeed.tikz");
            File.WriteAllText(inverseVsSuperIsospeedOutTikz, inverseVsSuperIsospeedTikz);
        }

        if (hasDirect && hasInverse)
        {
            var inverseVsDirectValues = GetComparisonValues(samples, InverseMethod, DirectMethod);
            var inverseVsDirectTikz = TikzPlotter.PlotTikzComparison(
                inverseVsDirectValues, InverseMethodLabel, DirectMethodLabel,
                new TikzPlotterSettings{ FontSize = FontSize.tiny });
            var inverseVsDirectOutTikz = Path.Combine(resultsPath, $"{type}-inverse-direct.tikz");
            File.WriteAllText(inverseVsDirectOutTikz, inverseVsDirectTikz);
        }

        if (hasDirect && hasInverse)
        {
            var isospeedVsBestValues = GetComparisonValues_Best(samples, IsospeedMethod,
                new List<string> { DirectMethod, InverseMethod });
            var isospeedVsBestTikz = TikzPlotter.PlotTikzComparison(
                isospeedVsBestValues, BestMethodLabel, IsospeedMethodLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            var isospeedVsBestOutTikz = Path.Combine(resultsPath, $"{type}-best-isospeed.tikz");
            File.WriteAllText(isospeedVsBestOutTikz, isospeedVsBestTikz);

            var superIsospeedVsBestValues = GetComparisonValues_Best(samples, SuperIsospeedMethod,
                new List<string> { DirectMethod, InverseMethod });
            var superIsospeedVsBestTikz = TikzPlotter.PlotTikzComparison(
                superIsospeedVsBestValues, BestMethodLabel, SuperIsospeedMethodLabel,
                new TikzPlotterSettings { FontSize = FontSize.tiny });
            var superIsospeedVsBestOutTikz = Path.Combine(resultsPath, $"{type}-best-superIsospeed.tikz");
            File.WriteAllText(superIsospeedVsBestOutTikz, superIsospeedVsBestTikz);
        }

        var superIsospeedVsIsospeedValues = GetComparisonValues(samples, IsospeedMethod, SuperIsospeedMethod);
        var superIsospeedVsIsospeedTikz = TikzPlotter.PlotTikzComparison(
            superIsospeedVsIsospeedValues,IsospeedMethodLabel, SuperIsospeedMethodLabel,
            new TikzPlotterSettings { FontSize = FontSize.tiny });
        var superIsospeedVsIsospeedOutTikz = Path.Combine(resultsPath, $"{type}-superIsospeed-isospeed.tikz");
        File.WriteAllText(superIsospeedVsIsospeedOutTikz, superIsospeedVsIsospeedTikz);
    }
}

void ParseMaxPlusIsoBenchmark(string resultsPath)
{
    var DirectMethod = "Direct";
    var IsospeedMethod = "Isospeed";
    var InversionMethod = "Inversion";

    var DirectMethodLabel = "direct";
    var IsospeedMethodLabel = "isospeed";
    var InversionMethodLabel = "inverse";
    var BestMethodLabel = "best";

    ComparisonByCurveType("IsoMaxPlusConvolutionBalancedStaircaseBenchmarks");
    ComparisonByCurveType("IsoMaxPlusConvolutionHorizontalStaircaseBenchmarks");
    ComparisonByCurveType("IsoMaxPlusConvolutionVerticalStaircaseBenchmarks");
    ComparisonByCurveType("IsoMaxPlusConvolutionHorizontalKTradeoffStaircaseBenchmarks");

    void ComparisonByCurveType(string type)
    {
        var csvPath = Path.Combine(resultsPath, $"{type}-report.csv");
        using var reader = new StreamReader(csvPath);

        var delimiter = File.ReadLines(csvPath).First().StartsWith("Method,") ? "," : ";";
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter
        };
        using var csv = new CsvReader(reader, config);

        var samples = csv.GetRecords<BenchmarkResult>().ToList();
        var maxpVsIsoValues = GetComparisonValues(samples, DirectMethod, IsospeedMethod);
        var maxpVsIsoTikz = TikzPlotter.PlotTikzComparison(
            maxpVsIsoValues, DirectMethodLabel, IsospeedMethodLabel);
        var maxpVsIsoOutTikz = Path.Combine(resultsPath, $"{type}-maxp-iso.tikz");
        File.WriteAllText(maxpVsIsoOutTikz, maxpVsIsoTikz);

        var minpVsIsoValues = GetComparisonValues(samples, InversionMethod, IsospeedMethod);
        var minpVsIsoTikz = TikzPlotter.PlotTikzComparison(
            minpVsIsoValues, InversionMethodLabel, IsospeedMethodLabel);
        var minpVsIsoOutTikz = Path.Combine(resultsPath, $"{type}-minp-iso.tikz");
        File.WriteAllText(minpVsIsoOutTikz, minpVsIsoTikz);

        var minpVsMaxpValues = GetComparisonValues(samples, InversionMethod, DirectMethod);
        var minpVsMaxpTikz = TikzPlotter.PlotTikzComparison(
            minpVsMaxpValues, InversionMethodLabel, DirectMethodLabel);
        var minpVsMaxpOutTikz = Path.Combine(resultsPath, $"{type}-minp-maxp.tikz");
        File.WriteAllText(minpVsMaxpOutTikz, minpVsMaxpTikz);

        var isoVsBestValues = GetComparisonValues_Best(samples, IsospeedMethod, new List<string>{ DirectMethod, InversionMethod });
        var isoVsBestTikz = TikzPlotter.PlotTikzComparison(
            isoVsBestValues, BestMethodLabel, IsospeedMethodLabel);
        var isoVsBestOutTikz = Path.Combine(resultsPath, $"{type}-best-iso.tikz");
        File.WriteAllText(isoVsBestOutTikz, isoVsBestTikz);
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
            var m1Time = timeStringToDecimal(m1TimeString);

            var method2Sample = g.First(br => br.Method == method2)!;
            var m2TimeString = method2Sample.Median ?? method2Sample.Mean;
            var m2Time = timeStringToDecimal(m2TimeString);

            return (x: m1Time, y: m2Time);
        })
        .ToList();
    return comparisonValues;

    decimal timeStringToDecimal(string s)
    {
        var raw = decimal.Parse(s.Split(" ")[0]);
        return s.EndsWith(" μs") ? raw / 1000:
            s.EndsWith(" ms") ? raw :
            s.EndsWith(" s") ? raw * 1_000 :
            throw new InvalidOperationException($"Unrecognized unit: {s}");
    }
}

List<(decimal x, decimal y)> GetComparisonValues_Best(List<BenchmarkResult> samples, string method1, List<string> otherMethods)
{
    var samplesByTestCase = samples.GroupBy(br => br.Pair);
    var comparisonValues = samplesByTestCase
        .Select(g =>
        {
            var method1Sample = g.First(br => br.Method == method1)!;
            var m1TimeString = method1Sample.Median ?? method1Sample.Mean;
            var m1Time = timeStringToDecimal(m1TimeString);

            var otherSamples = otherMethods.Select(m => {
                var mSample = g.First(br => br.Method == m)!;
                var mTimeString = mSample.Median ?? mSample.Mean;
                var mTime = timeStringToDecimal(mTimeString);
                return mTime;
            });
            return (x: otherSamples.Min(), y: m1Time);
        })
        .ToList();
    return comparisonValues;

    decimal timeStringToDecimal(string s)
    {
        var raw = decimal.Parse(s.Split(" ")[0]);
        return s.EndsWith(" μs") ? raw / 1000:
            s.EndsWith(" ms") ? raw :
            s.EndsWith(" s") ? raw * 1_000 :
            throw new InvalidOperationException($"Unrecognized unit: {s}");
    }
}