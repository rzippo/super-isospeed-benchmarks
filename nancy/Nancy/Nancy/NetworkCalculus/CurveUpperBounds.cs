using System;
using System.Collections.Generic;
using System.Linq;
using Unipi.Nancy.MinPlusAlgebra;

namespace Unipi.Nancy.NetworkCalculus;

/// <summary>
/// Provides upper-bounding methods
/// </summary>
public static class CurveUpperBounds
{
    /// <summary>
    /// Upper-bounds the given curve with a <see cref="SigmaRhoArrivalCurve"/>.
    /// </summary>
    /// <param name="curve"></param>
    /// <returns>The upper-bound.</returns>
    public static SigmaRhoArrivalCurve SigmaRhoUpperBound(this Curve curve)
    {
        if (curve is SigmaRhoArrivalCurve)
            return (SigmaRhoArrivalCurve)curve;

        var rate = curve.PseudoPeriodSlope;
        var burst = CornerPoints()
            .Select(p => p.Value - p.Time * rate)
            .Append(0)
            .Max();

        return new SigmaRhoArrivalCurve(sigma: burst, rho: rate);

        List<Point> CornerPoints()
        {
            var cornerPoints = new List<Point>();

            foreach (var element in curve.BaseSequence.Elements)
            {
                switch (element)
                {
                    case Point point:
                        cornerPoints.Add(point);
                        break;

                    case Segment segment:
                        cornerPoints.Add(new Point(
                            time: segment.StartTime,
                            value: segment.RightLimitAtStartTime
                        ));
                        cornerPoints.Add(new Point(
                            time: segment.EndTime,
                            value: segment.LeftLimitAtEndTime
                        ));
                        break;

                    default:
                        throw new InvalidCastException();
                }
            }

            return cornerPoints;
        }
    }

    // todo: write tests

    /// <summary>
    /// Upper-bounds the given curve with a <see cref="RateLatencyServiceCurve"/>.
    /// </summary>
    /// <param name="curve"></param>
    /// <returns>The upper-bound.</returns>
    public static RateLatencyServiceCurve RateLatencyUpperBound(this Curve curve)
    {
        if (curve is RateLatencyServiceCurve dr)
            return dr;
        if (curve.PseudoPeriodSlope.IsInfinite)
            throw new ArgumentException("Trying to upper bound a curve with infinite sustained rate");
        if (curve.RightLimitAt(0) > 0)
            throw new ArgumentException("Trying to upper bound a curve with infinite sustained rate");    

        var rate = curve.PseudoPeriodSlope;
        var latency = CornerPoints()
            .Where(p => p.Value > 0)
            .Select(p => -(p.Value - p.Time * rate)/rate)
            .Min();

        if (latency >= 0)
        {
            return new RateLatencyServiceCurve(rate, latency);
        }
        else
        {
            throw new ArgumentException("Cannot upper bound the curve with a non-negative latency and finite rate");    
        }

        List<Point> CornerPoints()
        {
            var cornerPoints = new List<Point>();

            foreach (var element in curve.BaseSequence.Elements)
            {
                switch (element)
                {
                    case Point point:
                        if (!point.IsOrigin)
                            cornerPoints.Add(point);
                        break;

                    case Segment segment:
                        cornerPoints.Add(new Point(
                            time: segment.StartTime,
                            value: segment.RightLimitAtStartTime
                        ));
                        cornerPoints.Add(new Point(
                            time: segment.EndTime,
                            value: segment.LeftLimitAtEndTime
                        ));
                        break;

                    default:
                        throw new InvalidCastException();
                }
            }

            return cornerPoints;
        }
    }
}