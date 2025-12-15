namespace TechChallenge.Measurements.Api;

public record MeasurementResponse
{
    public MeasurementResponse(long timestamp, double watts)
    {
        Timestamp = timestamp;
        Watts = watts;
    }

    public long Timestamp { get; init; }
    public double Watts { get; init; }

    public void Deconstruct(out long Timestamp, out double watts)
    {
        Timestamp = this.Timestamp;
        watts = Watts;
    }
}