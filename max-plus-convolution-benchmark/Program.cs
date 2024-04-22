// This program uses top-level statements
// See https://aka.ms/new-console-template for more information

using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.Numerics;

using max_plus_convolution_benchmark;

{
    // Code to retrieve and report the benchmark configuration

#if SANITY_CHECK
    var sanityCheck = true;
#else
    var sanityCheck = false;
#endif

    var jobAnnotation = typeof(MaxPlusConvolutionBalancedStaircaseBenchmarks)
        .GetCustomAttributes(typeof(SimpleJobAttribute), false)
        .FirstOrDefault() as SimpleJobAttribute;
    var iterationCount = jobAnnotation!.Config.GetJobs().First().Run.IterationCount;
    var warmupCount = jobAnnotation.Config.GetJobs().First().Run.WarmupCount;

    Console.WriteLine("(max,+) convolution benchmarks");
    Console.WriteLine("Current config:\n" +
                      $"\tsanityCheck: {sanityCheck}" +
                      $"\tnumberOfPairs: {Globals.TEST_COUNT}\n" +
                      $"\trngSeed: {Globals.RNG_SEED}\n" +
                      $"\trngMaxInteger: {Globals.RNG_MAX}\n" +
                      $"\tlargestExtensionsFilterMode: {Globals.FILTER_EXTENSION}\n" +
                      $"\tlargestExtensionsLowerThreshold: {Globals.LARGE_EXTENSION_LCM_LOWER_THRESHOLD}\n" +
                      $"\tlargestExtensionsUpperThreshold: {Globals.LARGE_EXTENSION_LCM_UPPER_THRESHOLD}\n" +
                      $"\titerationCount: {iterationCount}\n" +
                      $"\twarmupCount: {warmupCount}\n" +
                      $"");
}

#if SANITY_CHECK
SanityCheck(Globals.TEST_COUNT);
#endif

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args,
        DefaultConfig.Instance
            .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(10000))
            .WithCultureInfo(CultureInfo.InvariantCulture)
#if PROFILE
            .AddDiagnoser(new EtwProfiler())
#endif
    );

void SanityCheck(int n = 100)
{
    Console.WriteLine("Running sanity check");

    {
        Console.WriteLine("MaxPlusConvolutionBalancedStaircaseBenchmarks");
        var testsCount = MaxPlusConvolutionBalancedStaircaseBenchmarks.TestsCount;
        MaxPlusConvolutionBalancedStaircaseBenchmarks.TestsCount = n;
        var benchmarkRunner = new MaxPlusConvolutionBalancedStaircaseBenchmarks() { UseParallelism = true };
        foreach (var pair in MaxPlusConvolutionBalancedStaircaseBenchmarks.StairCurvePairs)
        {
            benchmarkRunner.Pair = pair;
            if (!pair.a.IsRightContinuous)
            {
                Console.WriteLine($"NOT RIGHT-CONTINUOUS:");
                Console.WriteLine(pair.a.ToCodeString());
            }

            if (!pair.b.IsRightContinuous)
            {
                Console.WriteLine($"NOT RIGHT-CONTINUOUS:");
                Console.WriteLine(pair.b.ToCodeString());
            }

            try
            {
#if ALL_ALGS
                var direct = benchmarkRunner.Direct();
                var inverse = benchmarkRunner.Inverse();
#endif
                var isospeed = benchmarkRunner.Isospeed();
                var superisospeed = benchmarkRunner.SuperIsospeed();
                if (
#if ALL_ALGS
                    !Curve.Equivalent(direct, inverse) || !Curve.Equivalent(inverse, isospeed) ||
#endif
                    !Curve.Equivalent(isospeed, superisospeed)
                )
                {
                    Console.WriteLine();
                    Console.WriteLine($"SANITY FAIL:");
                    Console.WriteLine(pair.a.ToCodeString());
                    Console.WriteLine(pair.b.ToCodeString());
                }
                else
                {
                    Console.Write(".");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(pair.a.ToCodeString());
                Console.WriteLine(pair.b.ToCodeString());
                throw;
            }
        }

        MaxPlusConvolutionBalancedStaircaseBenchmarks.TestsCount = testsCount;
        Console.WriteLine();
        Console.WriteLine();
    }

    {
        Console.WriteLine("MaxPlusConvolutionHorizontalStaircaseBenchmarks");
        var testsCount = MaxPlusConvolutionHorizontalStaircaseBenchmarks.TestsCount;
        MaxPlusConvolutionHorizontalStaircaseBenchmarks.TestsCount = n;
        var benchmarkRunner = new MaxPlusConvolutionHorizontalStaircaseBenchmarks() { UseParallelism = true };
        foreach (var pair in MaxPlusConvolutionHorizontalStaircaseBenchmarks.StairCurvePairs)
        {
            benchmarkRunner.Pair = pair;
            if (!pair.a.IsRightContinuous)
            {
                Console.WriteLine($"NOT RIGHT-CONTINUOUS:");
                Console.WriteLine(pair.a.ToCodeString());
            }

            if (!pair.b.IsRightContinuous)
            {
                Console.WriteLine($"NOT RIGHT-CONTINUOUS:");
                Console.WriteLine(pair.b.ToCodeString());
            }

            try
            {
#if ALL_ALGS
                var direct = benchmarkRunner.Direct();
                var inverse = benchmarkRunner.Inverse();
#endif
                var isospeed = benchmarkRunner.Isospeed();
                var superIsospeed = benchmarkRunner.SuperIsospeed();
                if (
#if ALL_ALGS
                    !Curve.Equivalent(direct, isospeed) || !Curve.Equivalent(inverse, isospeed) ||
#endif
                    !Curve.Equivalent(isospeed, superIsospeed)
                )
                {
                    Console.WriteLine();
                    Console.WriteLine($"SANITY FAIL:");
                    Console.WriteLine(pair.a.ToCodeString());
                    Console.WriteLine(pair.b.ToCodeString());
                }
                else
                {
                    Console.Write(".");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(pair.a.ToCodeString());
                Console.WriteLine(pair.b.ToCodeString());
                throw;
            }
        }

        MaxPlusConvolutionHorizontalStaircaseBenchmarks.TestsCount = testsCount;
        Console.WriteLine();
        Console.WriteLine();
    }

    {
        Console.WriteLine("MaxPlusConvolutionVerticalStaircaseBenchmarks");
        var testsCount = MaxPlusConvolutionVerticalStaircaseBenchmarks.TestsCount;
        MaxPlusConvolutionVerticalStaircaseBenchmarks.TestsCount = n;
        var benchmarkRunner = new MaxPlusConvolutionVerticalStaircaseBenchmarks() { UseParallelism = true };
        foreach (var pair in MaxPlusConvolutionVerticalStaircaseBenchmarks.StairCurvePairs)
        {
            benchmarkRunner.Pair = pair;
            if (!benchmarkRunner.Pair.a.IsRightContinuous)
            {
                Console.WriteLine($"NOT RIGHT-CONTINUOUS:");
                Console.WriteLine(benchmarkRunner.Pair.a.ToCodeString());
            }

            if (!benchmarkRunner.Pair.b.IsRightContinuous)
            {
                Console.WriteLine($"NOT RIGHT-CONTINUOUS:");
                Console.WriteLine(benchmarkRunner.Pair.b.ToCodeString());
            }

            try
            {
#if ALL_ALGS
                var direct = benchmarkRunner.Direct();
                var inverse = benchmarkRunner.Inverse();
#endif
                var isospeed = benchmarkRunner.Isospeed();
                var superIsospeed = benchmarkRunner.SuperIsospeed();
                if (
#if ALL_ALGS
                    !Curve.Equivalent(direct, inverse) || !Curve.Equivalent(inverse, isospeed) ||
#endif
                    !Curve.Equivalent(isospeed, superIsospeed)
                )
                {
                    Console.WriteLine();
                    Console.WriteLine($"SANITY FAIL:");
                    Console.WriteLine(pair.a.ToCodeString());
                    Console.WriteLine(pair.b.ToCodeString());
                }
                else
                {
                    Console.Write(".");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(pair.a.ToCodeString());
                Console.WriteLine(pair.b.ToCodeString());
                throw;
            }
        }

        MaxPlusConvolutionVerticalStaircaseBenchmarks.TestsCount = testsCount;
        Console.WriteLine();
        Console.WriteLine();
    }

    {
        Console.WriteLine("MaxPlusConvolutionHorizontalKTradeoffStaircaseBenchmarks");
        var testsCount = MaxPlusConvolutionHorizontalKTradeoffStaircaseBenchmarks.TestsCount;
        MaxPlusConvolutionHorizontalKTradeoffStaircaseBenchmarks.TestsCount = n;
        var benchmarkRunner = new MaxPlusConvolutionHorizontalKTradeoffStaircaseBenchmarks() { UseParallelism = true };
        foreach (var pair in MaxPlusConvolutionHorizontalKTradeoffStaircaseBenchmarks.StairCurvePairs)
        {
            benchmarkRunner.Pair = pair;
            if (!pair.a.IsRightContinuous)
            {
                Console.WriteLine($"NOT RIGHT-CONTINUOUS:");
                Console.WriteLine(pair.a.ToCodeString());
            }

            if (!pair.b.IsRightContinuous)
            {
                Console.WriteLine($"NOT RIGHT-CONTINUOUS:");
                Console.WriteLine(pair.b.ToCodeString());
            }

            try
            {
#if ALL_ALGS
                var direct = benchmarkRunner.Direct();
                var inverse = benchmarkRunner.Inverse();
#endif
                var isospeed = benchmarkRunner.Isospeed();
                var superIsospeed = benchmarkRunner.SuperIsospeed();
                if (
#if ALL_ALGS
                    !Curve.Equivalent(direct, isospeed) || !Curve.Equivalent(inverse, isospeed) ||
#endif
                    !Curve.Equivalent(isospeed, superIsospeed)
                )
                {
                    Console.WriteLine();
                    Console.WriteLine($"SANITY FAIL:");
                    Console.WriteLine(pair.a.ToCodeString());
                    Console.WriteLine(pair.b.ToCodeString());
                }
                else
                {
                    Console.Write(".");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(pair.a.ToCodeString());
                Console.WriteLine(pair.b.ToCodeString());
                throw;
            }
        }

        MaxPlusConvolutionHorizontalKTradeoffStaircaseBenchmarks.TestsCount = testsCount;
        Console.WriteLine();
        Console.WriteLine();
    }
}

namespace max_plus_convolution_benchmark
{
    public static class Globals {
        public const int TEST_COUNT = 100;

        public const int RNG_SEED = 4321;

        public const int RNG_MAX = 100;

        public const FILTER_VARIATION_ENUM FILTER_VARIATION = FILTER_VARIATION_ENUM.NO_FILTER;

        public enum FILTER_VARIATION_ENUM {
            NO_FILTER,
            FILTER_LARGER,
            FILTER_SMALLER
        };

        public const int LARGE_VARIATION_MULTIPLIER = 100;

        public const FILTER_EXTENSION_ENUM FILTER_EXTENSION = FILTER_EXTENSION_ENUM.FILTER_BOTH;

        public enum FILTER_EXTENSION_ENUM {
            // Does not use thresholds on largest k to filter pairs
            NO_FILTER,
            // Filters out pairs whose largest k is above the upper threshold
            FILTER_LARGER,
            // Filters out pairs whose largest k is below the lower threshold
            FILTER_SMALLER,
            // Filters out pairs whose largest k is above the upper threshold or below the lower threshold
            FILTER_BOTH
        };

        public const int LARGE_EXTENSION_LCM_UPPER_THRESHOLD = 500;
        public const int LARGE_EXTENSION_LCM_LOWER_THRESHOLD = 50;

        public static bool FilterByVariation(Curve f, Curve g)
        {
            if(FILTER_VARIATION == FILTER_VARIATION_ENUM.NO_FILTER)
                return true;

            var d_f = f.PseudoPeriodLength;
            var d_g = g.PseudoPeriodLength;
            var lcm_d = Rational.LeastCommonMultiple(d_f, d_g);
            var d_growth = Rational.Max(lcm_d / d_f, lcm_d / d_g);

            var c_f = f.PseudoPeriodHeight;
            var c_g = g.PseudoPeriodHeight;
            var lcm_c = Rational.LeastCommonMultiple(c_f, c_g);
            var c_growth = Rational.Max(lcm_c / c_f, lcm_c / c_g);

            if(FILTER_VARIATION == FILTER_VARIATION_ENUM.FILTER_LARGER)
                return d_growth >= LARGE_VARIATION_MULTIPLIER * c_growth || c_growth >= LARGE_VARIATION_MULTIPLIER * d_growth;
            else
                return d_growth <= LARGE_VARIATION_MULTIPLIER * c_growth || c_growth <= LARGE_VARIATION_MULTIPLIER * d_growth;
        }

        public static bool FilterByLargestExtension(Curve f, Curve g)
        {
            if(FILTER_EXTENSION == FILTER_EXTENSION_ENUM.NO_FILTER)
                return true;

            var d_f = f.PseudoPeriodLength;
            var d_g = g.PseudoPeriodLength;
            var lcm_d = Rational.LeastCommonMultiple(d_f, d_g);
            var d_growth = Rational.Max(lcm_d / d_f, lcm_d / d_g);

            var c_f = f.PseudoPeriodHeight;
            var c_g = g.PseudoPeriodHeight;
            var lcm_c = Rational.LeastCommonMultiple(c_f, c_g);
            var c_growth = Rational.Max(lcm_c / c_f, lcm_c / c_g);

            if (FILTER_EXTENSION == FILTER_EXTENSION_ENUM.FILTER_SMALLER)
            {
                return Rational.Max(d_growth, c_growth) >= LARGE_EXTENSION_LCM_LOWER_THRESHOLD;
            }
            else if (FILTER_EXTENSION == FILTER_EXTENSION_ENUM.FILTER_LARGER)
            {
                return Rational.Max(d_growth, c_growth) <= LARGE_EXTENSION_LCM_UPPER_THRESHOLD;
            }
            else
            {
                var maxExtension = Rational.Max(d_growth, c_growth);
                var largerCheck = maxExtension >= LARGE_EXTENSION_LCM_LOWER_THRESHOLD;
                var smallerCheck = maxExtension <= LARGE_EXTENSION_LCM_UPPER_THRESHOLD;
                return largerCheck && smallerCheck;
            }
        }

        public static ComputationSettings NoIsospeedSettings = ComputationSettings.Default() with
        {
            UseParallelism = false,
            UseConvolutionIsomorphismOptimization = false,
            UseBySequenceConvolutionIsomorphismOptimization = false
        };

        public static ComputationSettings IsospeedSettings = ComputationSettings.Default() with
        {
            UseParallelism = false,
            UseConvolutionIsomorphismOptimization = true,
            UseConvolutionSuperIsomorphismOptimization = false,
            UseBySequenceConvolutionIsomorphismOptimization = true
        };

        public static ComputationSettings SuperIsospeedSettings = ComputationSettings.Default() with
        {
            UseParallelism = false,
            UseConvolutionIsomorphismOptimization = true,
            UseConvolutionSuperIsomorphismOptimization = true,
            UseBySequenceConvolutionIsomorphismOptimization = true
        };
    }

#if PROFILE
[EtwProfiler]
#endif
    [SimpleJob()]
    public class MaxPlusConvolutionBalancedStaircaseBenchmarks
    {
        public static int TestsCount = Globals.TEST_COUNT;
        public static int MaxNumeratorDenominator = Globals.RNG_MAX;
        public static int RngSeed = Globals.RNG_SEED;

        [ParamsSource(nameof(StairCurvePairs))]
        public (Curve a, Curve b) Pair { get; set; }

        public bool UseParallelism { get; set; } = false;

#if ALL_ALGS
        [Benchmark]
#endif
        public Curve Direct()
        {
            var (a, b) = Pair;
            var result = Curve.MaxPlusConvolution(a, b, Globals.NoIsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

#if ALL_ALGS
        [Benchmark]
#endif
        public Curve Inverse()
        {
            var (a, b) = Pair;
            var a_lpi = a.LowerPseudoInverse();
            var b_lpi = b.LowerPseudoInverse();
            var result = Curve.Convolution(a_lpi, b_lpi, Globals.NoIsospeedSettings with {UseParallelism = UseParallelism}).UpperPseudoInverse();
            return result;
        }

        // [Benchmark(Baseline = true)]
        [Benchmark]
        public Curve Isospeed()
        {
            var (a, b) = Pair;
            var result = Curve.MaxPlusConvolution(a, b, Globals.IsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

        [Benchmark]
        public Curve SuperIsospeed()
        {
            var (a, b) = Pair;
            var result = Curve.MaxPlusConvolution(a, b, Globals.SuperIsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

        public static IEnumerable<(Curve a, Curve b)> StairCurvePairs =>
            RngStairCurves(RngSeed, MaxNumeratorDenominator)
                .Chunk(2)
                .Select(c => (f: (Curve) c[0], g: (Curve) c[1]))
                .Where(p => Globals.FilterByLargestExtension(p.f, p.g))
                .Where(p => Globals.FilterByVariation(p.f, p.g))
                .Take(TestsCount);

        public static IEnumerable<Curve> RngStairCurves(int seed, int max = 1000)
        {
            using var rngRationals = RngRationals.New(seed, max).GetEnumerator();
            rngRationals.MoveNext();
            while (true)
            {
                var a = rngRationals.Current;
                rngRationals.MoveNext();
                var b = rngRationals.Current;
                rngRationals.MoveNext();
                yield return new ConstantJumpsStaircaseCurve(
                    new List<Rational>(),
                    new List<Rational>{a, b}
                );
            }
        }
    }

#if PROFILE
[EtwProfiler]
#endif
    [SimpleJob()]
    public class MaxPlusConvolutionHorizontalStaircaseBenchmarks
    {
        public static int TestsCount = Globals.TEST_COUNT;
        public static int MaxNumeratorDenominator = Globals.RNG_MAX;
        public static int RngSeed = Globals.RNG_SEED;

        [ParamsSource(nameof(StairCurvePairs))]
        public (Curve a, Curve b) Pair { get; set; }

        public bool UseParallelism { get; set; } = false;

#if ALL_ALGS
        [Benchmark]
#endif
        public Curve Direct()
        {
            var (a, b) = Pair;
            var result = Curve.MaxPlusConvolution(a, b, Globals.NoIsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

#if ALL_ALGS
        [Benchmark]
#endif
        public Curve Inverse()
        {
            var (a, b) = Pair;
            var a_lpi = a.LowerPseudoInverse();
            var b_lpi = b.LowerPseudoInverse();
            var result = Curve.Convolution(a_lpi, b_lpi, Globals.NoIsospeedSettings with {UseParallelism = UseParallelism}).UpperPseudoInverse();
            return result;
        }

        // [Benchmark(Baseline = true)]
        [Benchmark]
        public Curve Isospeed()
        {
            var (a, b) = Pair;
            var result = Curve.MaxPlusConvolution(a, b, Globals.IsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

        [Benchmark]
        public Curve SuperIsospeed()
        {
            var (a, b) = Pair;
            var result = Curve.MaxPlusConvolution(a, b, Globals.SuperIsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

        public static IEnumerable<(Curve a, Curve b)> StairCurvePairs =>
            RngStairCurves(RngSeed, MaxNumeratorDenominator)
                .Chunk(2)
                .Select(c => (f: (Curve) c[0], g: (Curve) c[1]))
                .Where(p => Globals.FilterByLargestExtension(p.f, p.g))
                .Where(p => Globals.FilterByVariation(p.f, p.g))
                .Take(TestsCount);

        public static IEnumerable<Curve> RngStairCurves(int seed, int max = 1000)
        {
            using var rngRationals = RngRationals.New(seed, max).GetEnumerator();
            rngRationals.MoveNext();
            while (true)
            {
                var parameters = rngRationals.TakeNext(6);
                yield return new ManyConstantsStaircaseCurve(
                    new List<Rational>(),
                    parameters
                );
            }
        }
    }

#if PROFILE
[EtwProfiler]
#endif
    [SimpleJob()]
    public class MaxPlusConvolutionVerticalStaircaseBenchmarks
    {
        public static int TestsCount = Globals.TEST_COUNT;
        public static int MaxNumeratorDenominator = Globals.RNG_MAX;
        public static int RngSeed = Globals.RNG_SEED;

        [ParamsSource(nameof(StairCurvePairs))]
        public (Curve a, Curve b) Pair { get; set; }

        public bool UseParallelism { get; set; } = false;

#if ALL_ALGS
        [Benchmark]
#endif
        public Curve Direct()
        {
            var (a, b) = Pair;
            var result = Curve.MaxPlusConvolution(a, b, Globals.NoIsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

#if ALL_ALGS
        [Benchmark]
#endif
        public Curve Inverse()
        {
            var (a, b) = Pair;
            var a_lpi = a.LowerPseudoInverse();
            var b_lpi = b.LowerPseudoInverse();
            var result = Curve.Convolution(a_lpi, b_lpi, Globals.NoIsospeedSettings with {UseParallelism = UseParallelism}).UpperPseudoInverse();
            return result;
        }

        // [Benchmark(Baseline = true)]
        [Benchmark]
        public Curve Isospeed()
        {
            var (a, b) = Pair;
            var result = Curve.MaxPlusConvolution(a, b, Globals.IsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

        [Benchmark]
        public Curve SuperIsospeed()
        {
            var (a, b) = Pair;
            var result = Curve.MaxPlusConvolution(a, b, Globals.SuperIsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

        public static IEnumerable<(Curve a, Curve b)> StairCurvePairs =>
            RngStairCurves(RngSeed, MaxNumeratorDenominator)
                .Chunk(2)
                .Select(c => (f: (Curve) c[0], g: (Curve) c[1]))
                .Where(p => Globals.FilterByLargestExtension(p.f, p.g))
                .Where(p => Globals.FilterByVariation(p.f, p.g))
                .Take(TestsCount);

        public static IEnumerable<Curve> RngStairCurves(int seed, int max = 1000)
        {
            using var rngRationals = RngRationals.New(seed, max).GetEnumerator();
            rngRationals.MoveNext();
            while (true)
            {
                var tParams = rngRationals.TakeNext(3);
                var pParams = rngRationals.TakeNext(6);
                yield return new ManyJumpsStaircaseCurve(
                    tParams,
                    pParams
                );
            }
        }
    }

#if PROFILE
[EtwProfiler]
#endif
    [SimpleJob()]
    public class MaxPlusConvolutionHorizontalKTradeoffStaircaseBenchmarks
    {
        public static int TestsCount = Globals.TEST_COUNT;
        public static int MaxNumeratorDenominator = Globals.RNG_MAX;
        public static int RngSeed = Globals.RNG_SEED;

        [ParamsSource(nameof(StairCurvePairs))]
        public (Curve a, Curve b) Pair { get; set; }

        public bool UseParallelism { get; set; } = false;

#if ALL_ALGS
        [Benchmark]
#endif
        public Curve Direct()
        {
            var (a, b) = Pair;
            var result = Curve.Convolution(a, b, Globals.NoIsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

#if ALL_ALGS
        [Benchmark]
#endif
        public Curve Inverse()
        {
            var (a, b) = Pair;
            var a_upi = a.UpperPseudoInverse();
            var b_upi = b.UpperPseudoInverse();
            var result = Curve.MaxPlusConvolution(a_upi, b_upi, Globals.NoIsospeedSettings with {UseParallelism = UseParallelism}).LowerPseudoInverse();
            return result;
        }

        // [Benchmark(Baseline = true)]
        [Benchmark]
        public Curve Isospeed()
        {
            var (a, b) = Pair;
            var result = Curve.Convolution(a, b, Globals.IsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

        [Benchmark]
        public Curve SuperIsospeed()
        {
            var (a, b) = Pair;
            var result = Curve.Convolution(a, b, Globals.SuperIsospeedSettings with {UseParallelism = UseParallelism});
            return result;
        }

        public static IEnumerable<(Curve a, Curve b)> StairCurvePairs =>
            RngStairCurves(RngSeed, MaxNumeratorDenominator)
                .Where(p => Globals.FilterByLargestExtension(p.f, p.g))
                .Where(p => Globals.FilterByVariation(p.f, p.g))
                .Take(TestsCount);

        public static IEnumerable<(Curve f, Curve g)> RngStairCurves(int seed, int max = 1000)
        {
            using var rngRationals = RngRationals.New(seed, max).GetEnumerator();
            rngRationals.MoveNext();
            while (true)
            {
                var fs1l = rngRationals.TakeNext();
                var fs1s = rngRationals.TakeNext();
                var fc1l = rngRationals.TakeNext();
                var fs2l = rngRationals.TakeNext();
                var fs2s = rngRationals.TakeNext();
                var fc2l = rngRationals.TakeNext();

                var d_f = fs1l + fc1l + fs2l + fc2l;
                var c_f = fs1l * fs1s + fs2l * fs2s;

                var kc1 = rngRationals.TakeNext().Numerator;
                var kc2 = rngRationals.TakeNext().Numerator;
                var kcf = kc1 < kc2 ? kc1 : kc2;
                var kcg = kc1 < kc2 ? kc2 : kc1;

                var c_g = (c_f * kcf) / kcg;

                var kd1 = rngRationals.TakeNext().Numerator;
                var kd2 = rngRationals.TakeNext().Numerator;
                var kdf = kd1 < kd2 ? kd2 : kd1;
                var kdg = kd1 < kd2 ? kd1 : kd2;

                var d_g = (d_f * kdf) / kdg;

                var gs1l = 1 * d_g / 10;
                var gs1s = (c_g / 3) / gs1l;
                var gc1l = 2 * d_g / 10;
                var gs2l = 4 * d_g / 10;
                var gs2s = (2 * c_g / 3) / gs2l;
                var gc2l = 3 * d_g / 10;

                var f = new ManyConstantsStaircaseCurve(
                    new List<Rational>(),
                    new List<Rational>{fs1l, fs1s, fc1l, fs2l, fs2s, fc2l}
                );
                var g = new ManyConstantsStaircaseCurve(
                    new List<Rational>(),
                    new List<Rational>{gs1l, gs1s, gc1l, gs2l, gs2s, gc2l}
                );
                yield return (f, g);
            }
        }
    }
}