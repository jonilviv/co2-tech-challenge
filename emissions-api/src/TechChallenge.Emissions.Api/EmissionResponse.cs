namespace TechChallenge.Emissions.Api;

public record EmissionResponse
{
    public EmissionResponse(long timestamp, double kgPerWattHr)
    {
        Timestamp = timestamp;
        KgPerWattHr = kgPerWattHr;
    }

    public long Timestamp { get; init; }
    public double KgPerWattHr { get; init; }

    public void Deconstruct(out long Timestamp, out double kgPerWattHr)
    {
        Timestamp = this.Timestamp;
        kgPerWattHr = KgPerWattHr;
    }
}