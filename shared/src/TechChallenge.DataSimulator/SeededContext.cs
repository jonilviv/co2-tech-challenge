namespace TechChallenge.DataSimulator;

public record SeededContext
{
    public SeededContext(long timestamp, int seed, double factor)
    {
        Timestamp = timestamp;
        Seed = seed;
        Factor = factor;
    }

    public long Timestamp { get; init; }
    public int Seed { get; init; }
    public double Factor { get; init; }

    public void Deconstruct(out long Timestamp, out int seed, out double factor)
    {
        Timestamp = this.Timestamp;
        seed = Seed;
        factor = Factor;
    }
}