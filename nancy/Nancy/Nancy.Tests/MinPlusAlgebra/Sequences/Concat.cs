using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace Unipi.Nancy.Tests.MinPlusAlgebra.Sequences;

public class Concat
{
    private readonly ITestOutputHelper output;

    public Concat(ITestOutputHelper output)
    {
        this.output = output;
    }

    public static string[] JsonTestCases =
    {
        "Unipi.Nancy.Tests/MinPlusAlgebra/Sequences/JsonConcatTests/1.json"
    };

    public static IEnumerable<object[]> GetJsonTestCases()
    {
        foreach (var elementsName in JsonTestCases)
        {
            string json = EmbeddedResourceDataAttribute.ReadManifestData(elementsName);
            var sequences = JsonConvert.DeserializeObject<Sequence[]>(json, new RationalNewtonsoftJsonConverter())!;

            yield return new object[] { sequences };
        }
    }

    [Theory]
    [MemberData(nameof(GetJsonTestCases))]
    public void JsonConcatTest(Sequence[] sequences)
    {
        var sequence = Sequence.Concat(sequences);
    }
}