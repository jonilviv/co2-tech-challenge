using System;

namespace TechChallenge.DataSimulator;

public class PointsProvider : BasePointsProvider
{
    private readonly int _minTimestampIncrement;
    private readonly int _maxTimestampIncrement;

    public PointsProvider(int minTimestampIncrement,
        int maxTimestampIncrement,
        IValueCalculator<SeededContext, double> calculator) : base(calculator)
    {
        _minTimestampIncrement = minTimestampIncrement;
        _maxTimestampIncrement = maxTimestampIncrement;
    }

    protected override int GetTimestampIncrement(int seed)
    {
        var random = new Random(seed);

        int timestampIncrement = random.Next(_minTimestampIncrement, _maxTimestampIncrement);

        return timestampIncrement;
    }
}