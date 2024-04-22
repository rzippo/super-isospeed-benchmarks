using Unipi.Nancy.Numerics;

namespace max_plus_convolution_benchmark;

public static class RngRationals
{
    /// <summary>
    /// Returns an infinite stream of non-zero random Rationals.
    /// </summary>
    /// <param name="seed">Seed of the random generator.</param>
    /// <param name="max">Numerator and denominator will be upper-bounded by this value.</param>
    /// <param name="integers">If true, the denominator will always be 1.</param>
    /// <returns></returns>
    public static IEnumerable<Rational> New(int seed, int max = 1000, bool integers = true)
    {
        var rng = new Random(seed);
        while (true)
        {
            var n = rng.Next(max) + 1;
            var d = integers ? 1 : rng.Next(max) + 1;
            yield return new Rational(n, d);
        }
    }

    /// <summary>
    /// Takes n elements from source, and returns them as a list.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static List<Rational> TakeNext(this IEnumerator<Rational> source, int n)
    {
        var rationals = new List<Rational>();
        for (int i = 0; i < n; i++)
        {
            if(source.MoveNext())
                rationals.Add(source.Current);
        }

        return rationals;
    }

    /// <summary>
    /// Returns the next rational.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Rational TakeNext(this IEnumerator<Rational> source)
    {
        if(source.MoveNext())
            return source.Current;
        else
            throw new InvalidOperationException();
    }


}