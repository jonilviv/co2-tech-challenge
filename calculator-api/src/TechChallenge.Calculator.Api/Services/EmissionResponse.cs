namespace TechChallenge.Calculator.Api.Services
{
    public record EmissionResponse
    {
        public EmissionResponse(long timestamp, double kgPerWattHr)
        {
            Timestamp = timestamp;
            KgPerWattHr = kgPerWattHr;
        }

        public long Timestamp { get; init; }
        public double KgPerWattHr { get; init; }

        public void Deconstruct(out long timestamp, out double kgPerWattHr)
        {
            timestamp = Timestamp;
            kgPerWattHr = KgPerWattHr;
        }
    }
}