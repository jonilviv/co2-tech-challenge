using System.Collections.Generic;

namespace TechChallenge.DataSimulator;

public abstract class BasePointsProvider : IPointsProvider
{
    private readonly IValueCalculator<SeededContext, double> _calculator;

    protected BasePointsProvider(IValueCalculator<SeededContext, double> calculator)
    {
        _calculator = calculator;
    }

    public IEnumerable<Point> GetPoints(
        long fromTimestamp,
        long toTimestamp,
        int seed,
        double factor)
    {
        int step = GetTimestampIncrement(seed);

        long start =
            fromTimestamp % step == 0
                ? fromTimestamp
                : (fromTimestamp / step + 1) * step;

        for (long timestamp = start; timestamp <= toTimestamp; timestamp += step)
        {
            SeededContext calculationContext = new SeededContext(timestamp, seed, factor);
            double calculate = _calculator.Calculate(calculationContext);
            var point = new Point(timestamp, calculate);

            yield return point;
        }
    }

    protected abstract int GetTimestampIncrement(int seed);
}