using CsvHelper.Configuration.Attributes;

public record SpeedupsTableRow
{
    public string BenchmarkName {get; set;}
    
    [Name("Baseline algorithm")]
    public string FirstMethod {get; set;}
    [Name("Comparison algorithm")]
    public string SecondMethod {get; set;}
    
    [Name("Speedup 10th percentile")]
    public decimal TenthPercentile {get; set;}
    
    [Name("Speedup 25th percentile")]
    public decimal FirstQuartile {get; set;}
    
    [Name("Speedup 50th percentile")]
    public decimal Median {get; set;}
    
    [Name("Speedup 75th percentile")]
    public decimal ThirdQuartile {get; set;}
    
    [Name("Speedup 90th percentile")]
    public decimal NinentiethPercentile {get; set;}
}