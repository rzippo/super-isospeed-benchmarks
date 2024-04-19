﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.MinPlusAlgebra.Json;
using Unipi.Nancy.Numerics;

namespace Unipi.Nancy.NetworkCalculus.Json;

/// <exclude />
/// <summary>
/// Custom Newtonsoft.Json JsonConverter for <see cref="ConcaveCurve"/>.
/// </summary>
public class ConcaveCurveNewtonsoftJsonConverter : JsonConverter
{
    private const string TypeName = "type";

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(ConcaveCurve));
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);

        serializer.Converters.Add(new RationalNewtonsoftJsonConverter());

        Sequence? sequence = jo[CurveNewtonsoftJsonConverter.BaseSequenceName]?.ToObject<Sequence>();
        if (sequence == null)
            throw new InvalidOperationException("Invalid JSON format.");
        Rational periodStart = jo[CurveNewtonsoftJsonConverter.PseudoPeriodStartName]!.ToObject<Rational>();
        Rational periodLength = jo[CurveNewtonsoftJsonConverter.PseudoPeriodLengthName]!.ToObject<Rational>();
        Rational periodHeight = jo[CurveNewtonsoftJsonConverter.PseudoPeriodHeightName]!.ToObject<Rational>();

        ConcaveCurve curve = new ConcaveCurve(
            baseSequence: sequence,
            pseudoPeriodStart: periodStart,
            pseudoPeriodLength: periodLength,
            pseudoPeriodHeight: periodHeight
        );
        return curve;
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        ConcaveCurve curve = (ConcaveCurve) value;

        serializer.Converters.Add(new RationalNewtonsoftJsonConverter());

        JObject jo = new JObject
        {
            { TypeName, JToken.FromObject(ConcaveCurve.TypeCode, serializer) },
            { CurveNewtonsoftJsonConverter.BaseSequenceName, JToken.FromObject(curve.BaseSequence, serializer) },
            { CurveNewtonsoftJsonConverter.PseudoPeriodStartName, JToken.FromObject(curve.PseudoPeriodStart, serializer) },
            { CurveNewtonsoftJsonConverter.PseudoPeriodLengthName, JToken.FromObject(curve.PseudoPeriodLength, serializer) },
            { CurveNewtonsoftJsonConverter.PseudoPeriodHeightName, JToken.FromObject(curve.PseudoPeriodHeight, serializer) }
        };

        jo.WriteTo(writer);
    }
}