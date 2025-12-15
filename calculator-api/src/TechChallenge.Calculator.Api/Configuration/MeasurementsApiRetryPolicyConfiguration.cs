namespace TechChallenge.Calculator.Api.Configuration;

public class MeasurementsApiRetryPolicyConfiguration
{
    public int MaxRetries { get; set; } = 5;

    public double BaseDelaySeconds { get; set; } = 2.0;
}