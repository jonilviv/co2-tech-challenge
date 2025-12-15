using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using TechChallenge.Common.Exceptions;
using TechChallenge.DataSimulator;

namespace TechChallenge.Measurements.Api.Data;

public class CalculationBasedUserHardcodedMeasurementsRepository : IMeasurementsRepository
{
    private readonly TimeProvider _timeProvider;
    private readonly IPointsProvider _pointsProvider;
    private readonly ILogger<CalculationBasedUserHardcodedMeasurementsRepository> _logger;

    public CalculationBasedUserHardcodedMeasurementsRepository(TimeProvider timeProvider,
        IPointsProvider pointsProvider,
        ILogger<CalculationBasedUserHardcodedMeasurementsRepository> logger)
    {
        _timeProvider = timeProvider;
        _pointsProvider = pointsProvider;
        _logger = logger;
    }

    private static readonly IReadOnlyDictionary<string, double> UserHardcodedFactors =
        new Dictionary<string, double>
        {
            { "alpha", 113.23 },
            { "betta", 214.34 },
            { "gamma", 115.45 },
            { "delta", 136.56 },
            { "epsilon", 517.67 },
            { "zeta", 218.78 },
            { "eta", 619.89 },
            { "theta", 120.00 },
        };

    public async IAsyncEnumerable<Measurement> GetMeasurementsAsync(
        string userId,
        long from,
        long to,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!UserHardcodedFactors.ContainsKey(userId))
        {
            _logger.LogWarning("User {userId} was not found", userId);

            throw new NotFoundException("User was not found");
        }

        DateTimeOffset dataLimit = _timeProvider.GetUtcNow();
        to = Math.Min(to, dataLimit.ToUnixTimeSeconds());

        int userSeed = CalculateSeed(userId);
        double factor = UserHardcodedFactors[userId];

        IEnumerable<Point> enumerable = _pointsProvider.GetPoints(from, to, userSeed, factor);

        foreach (Point point in enumerable)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var measurement = new Measurement
            {
                UserId = userId,
                Timestamp = point.Timestamp,
                Watts = point.Value
            };

            yield return measurement;
        }
    }

    private static int CalculateSeed(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA256.HashData(bytes);
        int calculateSeed = BitConverter.ToInt32(hashBytes, 0);

        return calculateSeed;
    }
}