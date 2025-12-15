namespace TechChallenge.DataSimulator;

public record Point
{
    public Point(long timestamp, double value)
    {
        Timestamp = timestamp;
        Value = value;
    }

    public long Timestamp { get; init; }
    public double Value { get; init; }

    public void Deconstruct(out long Timestamp, out double value)
    {
        Timestamp = this.Timestamp;
        value = Value;
    }
}