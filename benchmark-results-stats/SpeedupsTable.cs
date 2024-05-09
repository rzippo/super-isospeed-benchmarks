
public static class SpeedupsTable
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method1Runtimes">The baseline method.</param>
    /// <param name="method2Runtimes">The comparison method -- it should be faster.</param>
    /// <param name="percentiles">The desired percentiles. Each should be between 0 and 100.</param>
    public static List<decimal> GetSpeedupsPercentiles(List<decimal> method1Runtimes, List<decimal> method2Runtimes, List<int> percentiles)
    {
        if(method1Runtimes.Count != method2Runtimes.Count)
            throw new ArgumentException("The two runtime lists must be of the same length.");

        var pairedRuntimes = Enumerable.Zip(method1Runtimes, method2Runtimes)
            .Select(p => (x: p.First, y: p.Second))
            .ToList();

        return GetSpeedupsPercentiles(pairedRuntimes, percentiles);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method1Runtimes">The baseline method.</param>
    /// <param name="method2Runtimes">The comparison method -- it should be faster.</param>
    /// <param name="percentiles">The desired percentiles. Each should be between 0 and 100.</param>
    public static List<decimal> GetSpeedupsPercentiles(List<(decimal x, decimal y)> pairedRuntimes, List<int> percentiles)
    {
        if(percentiles.Any(p => p < 0 || p > 100))
            throw new ArgumentException("Percentiles must be between 0 and 100.");

        var N = pairedRuntimes.Count;

        var speedups = pairedRuntimes
            .Select(pair => pair.x / pair.y)
            .Order()
            .ToList();

        var results = new List<decimal>();
        foreach(var percentile in percentiles)
        {
            var index = (int) Math.Ceiling((decimal) percentile * N / 100);
            results.Add(speedups[index]);
        }
        return results;
    }
}