// See https://aka.ms/new-console-template for more information

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace benchmark_results_model;

public static class BenchmarkResults
{
    public static string DirectBenchmarksLabel = "Direct";
    public static string InverseBenchmarksLabel = "Inverse";
    public static string IsospeedBenchmarksLabel = "Isospeed";
    public static string SuperIsospeedBenchmarksLabel = "SuperIsospeed";

    public static List<BenchmarkResult> ParseCsv(string csvPath)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        if (!File.Exists(csvPath))
            throw new InvalidOperationException($"File \"{csvPath}\" does not exist!");

        using var reader = new StreamReader(csvPath);

        var delimiter = File.ReadLines(csvPath).First().StartsWith("Method,") ? "," : ";";
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter
        };
        using var csv = new CsvReader(reader, config);

        var samples = csv.GetRecords<BenchmarkResult>().ToList();

        return samples;
    }
    
    public static List<(decimal x, decimal y)> GetComparisonValues(List<BenchmarkResult> samples, string method1, string method2)
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
    
    public static List<(decimal x, decimal y)> GetComparisonValues_Best(List<BenchmarkResult> samples, string method1, List<string> otherMethods)
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
    
    public static decimal TimeStringToDecimal(string s)
    {
        var raw = decimal.Parse(s.Split(" ")[0]);
        return s.EndsWith(" μs") ? raw / 1000:
            s.EndsWith(" ms") ? raw :
            s.EndsWith(" s") ? raw * 1_000 :
            throw new InvalidOperationException($"Unrecognized unit: {s}");
    }
}