namespace TechChallenge.Calculator.Api.Services
{
    public record MeasurementResponse
    {
        public MeasurementResponse(long timestamp, double watts)
        {
            Timestamp = timestamp;
            Watts = watts;
        }

        public long Timestamp { get; init; }
        public double Watts { get; init; }

        public void Deconstruct(out long timestamp, out double watts)
        {
            timestamp = Timestamp;
            watts = Watts;
        }
    }
}