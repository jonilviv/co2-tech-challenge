using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TechChallenge.Calculator.Api.Services;

public class CalculationService : ICalculationService
{
    private const int __fifteenMinutesInSeconds = 15 * 60;
    private readonly ILogger<CalculationService> _logger;

    public CalculationService(ILogger<CalculationService> logger)
    {
        _logger = logger;
    }

    public double CalculateTotalEmissions(
        IReadOnlyList<MeasurementResponse> measurements,
        IReadOnlyList<EmissionResponse> emissions)
    {
        if (measurements.Count == 0)
        {
            return 0.0;
        }

        // Create a dictionary for fast emission factor lookup
        Dictionary<long, double> emissionFactors = emissions.ToDictionary(e => e.Timestamp, e => e.KgPerWattHr);

        // Group measurements into 15-minute periods
        List<Period> periods = GroupMeasurementsIntoPeriods(measurements);

        double totalEmissions = 0.0;

        foreach (Period period in periods)
        {
            // Calculate average watts for the period
            var averageWatts = period.Measurements.Average(m => m.Watts);

            // Convert to kWh: averageWatts / 4 (15 mins = 1/4 hour) / 1000 (W to kW)
            double kWh = averageWatts / 4.0 / 1000.0;

            // Get the emission factor for this period (use the period start timestamp)
            // Emission factors are provided at 15-minute intervals
            if (!emissionFactors.TryGetValue(period.PeriodStart, out var emissionFactor))
            {
                // If exact timestamp not found, find the closest one
                EmissionResponse? closestEmission = emissions
                    .OrderBy(e => Math.Abs(e.Timestamp - period.PeriodStart))
                    .FirstOrDefault();

                if (closestEmission == null)
                {
                    _logger.LogWarning("No emission factor found for period starting at {PeriodStart}, skipping", period.PeriodStart);

                    continue;
                }

                emissionFactor = closestEmission.KgPerWattHr;
            }

            double periodEmissions = kWh * emissionFactor;
            totalEmissions += periodEmissions;

            _logger.LogDebug(
                "Period {PeriodStart}-{PeriodEnd}: {KWh} kWh * {Factor} = {Emissions} kg CO2",
                period.PeriodStart,
                period.PeriodEnd,
                kWh,
                emissionFactor,
                periodEmissions);
        }

        return totalEmissions;
    }

    private static List<Period> GroupMeasurementsIntoPeriods(IReadOnlyList<MeasurementResponse> measurements)
    {
        var periods = new List<Period>();
        var sortedMeasurements = measurements.OrderBy(m => m.Timestamp).ToList();

        if (sortedMeasurements.Count == 0)
        {
            return periods;
        }

        // Calculate the first period start (aligned to 15-minute boundary)
        var firstTimestamp = sortedMeasurements[0].Timestamp;
        var firstPeriodStart = AlignToFifteenMinutes(firstTimestamp);

        long currentPeriodStart = firstPeriodStart;
        var currentPeriodMeasurements = new List<MeasurementResponse>();

        foreach (MeasurementResponse measurement in sortedMeasurements)
        {
            long periodEnd = currentPeriodStart + __fifteenMinutesInSeconds;

            // Check if measurement belongs to current period
            if (measurement.Timestamp >= currentPeriodStart &&
                measurement.Timestamp < periodEnd)
            {
                currentPeriodMeasurements.Add(measurement);
            }
            else
            {
                // Save current period if it has measurements
                if (currentPeriodMeasurements.Count > 0)
                {
                    var period = new Period(currentPeriodStart, periodEnd, currentPeriodMeasurements);
                    periods.Add(period);
                }

                // Start new period
                currentPeriodStart = AlignToFifteenMinutes(measurement.Timestamp);
                currentPeriodMeasurements = new List<MeasurementResponse> { measurement };
            }
        }

        // Add the last period
        if (currentPeriodMeasurements.Count > 0)
        {
            long periodEnd = currentPeriodStart + __fifteenMinutesInSeconds;
            var period = new Period(currentPeriodStart, periodEnd, currentPeriodMeasurements);
            periods.Add(period);
        }

        return periods;
    }

    private static long AlignToFifteenMinutes(long timestamp)
    {
        long fifteenMinutes = timestamp / __fifteenMinutesInSeconds * __fifteenMinutesInSeconds;

        return fifteenMinutes;
    }

    private record Period
    {
        public Period(long periodStart, long periodEnd, List<MeasurementResponse> measurements)
        {
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            Measurements = measurements;
        }

        public long PeriodStart { get; }

        public long PeriodEnd { get; }

        public List<MeasurementResponse> Measurements { get; }
    }
}