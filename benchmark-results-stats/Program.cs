// See https://aka.ms/new-console-template for more information

using System.Globalization;
using benchmark_results_model;
using CsvHelper;

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

var speedupsTable = new List<SpeedupsTableRow>();
var outputCsvPath = Path.Combine(resultsDirectoryPath, "speedups.csv");

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
        var directVsIsospeedPercentileValues = PercentilesUtil.GetSpeedupsPercentiles(directVsIsospeedValues, percentiles);
        var directVsIsospeedPercentilesString = string.Join("; ", directVsIsospeedPercentileValues);
        speedupsTable.Add(new SpeedupsTableRow {
            BenchmarkName = benchmarkName, 
            FirstMethod = DirectTableLabel, 
            SecondMethod = IsospeedTableLabel, 
            TenthPercentile = directVsIsospeedPercentileValues[0],
            FirstQuartile = directVsIsospeedPercentileValues[1],
            Median = directVsIsospeedPercentileValues[2],
            ThirdQuartile = directVsIsospeedPercentileValues[3],
            NinentiethPercentile = directVsIsospeedPercentileValues[4] 
        });

        //Console.WriteLine($"{benchmarkName}; {DirectTableLabel}; {IsospeedTableLabel}; {directVsIsospeedPercentilesString}");

        var directVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.SuperIsospeedBenchmarksLabel);
        var directVsSuperIsospeedPercentileValues = PercentilesUtil.GetSpeedupsPercentiles(directVsSuperIsospeedValues, percentiles);
        var directVsSuperIsospeedPercentilesString = string.Join("; ", directVsSuperIsospeedPercentileValues);
        speedupsTable.Add(new SpeedupsTableRow {
            BenchmarkName = benchmarkName, 
            FirstMethod = DirectTableLabel, 
            SecondMethod = SuperIsospeedTableLabel, 
            TenthPercentile = directVsSuperIsospeedPercentileValues[0],
            FirstQuartile = directVsSuperIsospeedPercentileValues[1],
            Median = directVsSuperIsospeedPercentileValues[2],
            ThirdQuartile = directVsSuperIsospeedPercentileValues[3],
            NinentiethPercentile = directVsSuperIsospeedPercentileValues[4] 
        });

        //Console.WriteLine($"{benchmarkName}; {DirectTableLabel}; {SuperIsospeedTableLabel}; {directVsSuperIsospeedPercentilesString}");
    }

    if (hasInverse)
    {
        var inverseVsIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.InverseBenchmarksLabel, BenchmarkResults.IsospeedBenchmarksLabel);
        var inverseVsIsospeedPercentileValues = PercentilesUtil.GetSpeedupsPercentiles(inverseVsIsospeedValues, percentiles);
        var inverseVsIsospeedPercentilesString = string.Join("; ", inverseVsIsospeedPercentileValues);
        speedupsTable.Add(new SpeedupsTableRow {
            BenchmarkName = benchmarkName, 
            FirstMethod = InverseTableLabel, 
            SecondMethod = IsospeedTableLabel, 
            TenthPercentile = inverseVsIsospeedPercentileValues[0],
            FirstQuartile = inverseVsIsospeedPercentileValues[1],
            Median = inverseVsIsospeedPercentileValues[2],
            ThirdQuartile = inverseVsIsospeedPercentileValues[3],
            NinentiethPercentile = inverseVsIsospeedPercentileValues[4] 
        });

        //Console.WriteLine($"{benchmarkName}; {InverseTableLabel}; {IsospeedTableLabel}; {inverseVsIsospeedPercentilesString}");

        var inverseVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.InverseBenchmarksLabel, BenchmarkResults.SuperIsospeedBenchmarksLabel);
        var inverseVsSuperIsospeedPercentileValues = PercentilesUtil.GetSpeedupsPercentiles(inverseVsSuperIsospeedValues, percentiles);
        var inverseVsSuperIsospeedPercentilesString = string.Join("; ", inverseVsSuperIsospeedPercentileValues);
        speedupsTable.Add(new SpeedupsTableRow {
            BenchmarkName = benchmarkName, 
            FirstMethod = InverseTableLabel, 
            SecondMethod = SuperIsospeedTableLabel, 
            TenthPercentile = inverseVsSuperIsospeedPercentileValues[0],
            FirstQuartile = inverseVsSuperIsospeedPercentileValues[1],
            Median = inverseVsSuperIsospeedPercentileValues[2],
            ThirdQuartile = inverseVsSuperIsospeedPercentileValues[3],
            NinentiethPercentile = inverseVsSuperIsospeedPercentileValues[4] 
        });

        //Console.WriteLine($"{benchmarkName}; {InverseTableLabel}; {SuperIsospeedTableLabel}; {inverseVsSuperIsospeedPercentilesString}");
    }

    if (hasDirect && hasInverse)
    {
        var inverseVsDirectValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.InverseBenchmarksLabel, BenchmarkResults.DirectBenchmarksLabel);
        var inverseVsDirectPercentileValues = PercentilesUtil.GetSpeedupsPercentiles(inverseVsDirectValues, percentiles);
        var inverseVsDirectPercentilesString = string.Join("; ", inverseVsDirectPercentileValues);
        speedupsTable.Add(new SpeedupsTableRow {
            BenchmarkName = benchmarkName, 
            FirstMethod = InverseTableLabel, 
            SecondMethod = DirectTableLabel, 
            TenthPercentile = inverseVsDirectPercentileValues[0],
            FirstQuartile = inverseVsDirectPercentileValues[1],
            Median = inverseVsDirectPercentileValues[2],
            ThirdQuartile = inverseVsDirectPercentileValues[3],
            NinentiethPercentile = inverseVsDirectPercentileValues[4] 
        });

        //Console.WriteLine($"{benchmarkName}; {InverseTableLabel}; {DirectTableLabel}; {inverseVsDirectPercentilesString}");
    }

    if (hasDirect && hasInverse)
    {
        var bestVsIsospeedValues = BenchmarkResults.GetComparisonValues_Best(samples, BenchmarkResults.IsospeedBenchmarksLabel,
            new List<string> { BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.InverseBenchmarksLabel });
        var bestVsIsospeedPercentileValues = PercentilesUtil.GetSpeedupsPercentiles(bestVsIsospeedValues, percentiles);
        var bestVsIsospeedPercentilesString = string.Join("; ", bestVsIsospeedPercentileValues);
        speedupsTable.Add(new SpeedupsTableRow {
            BenchmarkName = benchmarkName, 
            FirstMethod = BestTableLabel, 
            SecondMethod = IsospeedTableLabel, 
            TenthPercentile = bestVsIsospeedPercentileValues[0],
            FirstQuartile = bestVsIsospeedPercentileValues[1],
            Median = bestVsIsospeedPercentileValues[2],
            ThirdQuartile = bestVsIsospeedPercentileValues[3],
            NinentiethPercentile = bestVsIsospeedPercentileValues[4] 
        });

        //Console.WriteLine($"{benchmarkName}; {BestTableLabel}; {IsospeedTableLabel}; {bestVsIsospeedPercentilesString}");

        var bestVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues_Best(samples, BenchmarkResults.SuperIsospeedBenchmarksLabel,
            new List<string> { BenchmarkResults.DirectBenchmarksLabel, BenchmarkResults.InverseBenchmarksLabel });
        var bestVsSuperIsospeedPercentileValues = PercentilesUtil.GetSpeedupsPercentiles(bestVsSuperIsospeedValues, percentiles);
        var bestVsSuperIsospeedPercentilesString = string.Join("; ", bestVsSuperIsospeedPercentileValues);
        speedupsTable.Add(new SpeedupsTableRow {
            BenchmarkName = benchmarkName, 
            FirstMethod = BestTableLabel, 
            SecondMethod = SuperIsospeedTableLabel, 
            TenthPercentile = bestVsSuperIsospeedPercentileValues[0],
            FirstQuartile = bestVsSuperIsospeedPercentileValues[1],
            Median = bestVsSuperIsospeedPercentileValues[2],
            ThirdQuartile = bestVsSuperIsospeedPercentileValues[3],
            NinentiethPercentile = bestVsSuperIsospeedPercentileValues[4] 
        });

        //Console.WriteLine($"{benchmarkName}; {BestTableLabel}; {SuperIsospeedTableLabel}; {bestVsSuperIsospeedPercentilesString}");
    }

    var isospeedVsSuperIsospeedValues = BenchmarkResults.GetComparisonValues(samples, BenchmarkResults.IsospeedBenchmarksLabel, BenchmarkResults.SuperIsospeedBenchmarksLabel);
    var isospeedVsSuperIsospeedPercentileValues = PercentilesUtil.GetSpeedupsPercentiles(isospeedVsSuperIsospeedValues, percentiles);
    var isospeedVsSuperIsospeedPercentilesString = string.Join("; ", isospeedVsSuperIsospeedPercentileValues);
    speedupsTable.Add(new SpeedupsTableRow {
        BenchmarkName = benchmarkName, 
        FirstMethod = IsospeedTableLabel, 
        SecondMethod = SuperIsospeedTableLabel, 
        TenthPercentile = isospeedVsSuperIsospeedPercentileValues[0],
        FirstQuartile = isospeedVsSuperIsospeedPercentileValues[1],
        Median = isospeedVsSuperIsospeedPercentileValues[2],
        ThirdQuartile = isospeedVsSuperIsospeedPercentileValues[3],
        NinentiethPercentile = isospeedVsSuperIsospeedPercentileValues[4] 
    });

    //Console.WriteLine($"{benchmarkName}; {IsospeedTableLabel}; {SuperIsospeedTableLabel}; {isospeedVsSuperIsospeedPercentilesString}");
}

// write table to csv
using (var writer = new StreamWriter(outputCsvPath))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(speedupsTable);
}
