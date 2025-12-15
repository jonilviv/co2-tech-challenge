using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using TechChallenge.DataSimulator;

namespace TechChallenge.Emissions.Api.Data;

public class CalculationBasedEmissionsRepository : IEmissionsRepository
{
    private readonly IPointsProvider _pointsProvider;
    private readonly TimeProvider _timeProvider;

    public CalculationBasedEmissionsRepository(IPointsProvider pointsProvider,
        TimeProvider timeProvider,
        ILogger<CalculationBasedEmissionsRepository> logger)
    {
        _pointsProvider = pointsProvider;
        _timeProvider = timeProvider;
    }

    private const string Kye = "emissions";
    private const double Factor = 10;

    public async IAsyncEnumerable<Emission> GetAsync(
        long from,
        long to,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        DateTimeOffset dataLimit = _timeProvider.GetUtcNow().AddDays(1);
        to = Math.Min(to, dataLimit.ToUnixTimeSeconds());
        int userSeed = CalculateSeed(Kye);
        IEnumerable<Point> enumerable = _pointsProvider.GetPoints(from, to, userSeed, Factor);

        foreach (Point point in enumerable)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var emission = new Emission
            {
                Timestamp = point.Timestamp,
                KgPerWattHr = point.Value
            };

            yield return emission;
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