namespace TechChallenge.Calculator.Api.Configuration;

public class EmissionsApiRetryPolicyConfiguration
{
    public int MaxRetries { get; set; } = 3;

    public double BaseDelaySeconds { get; set; } = 2.0;

    public int TimeoutSeconds { get; set; } = 30;
}