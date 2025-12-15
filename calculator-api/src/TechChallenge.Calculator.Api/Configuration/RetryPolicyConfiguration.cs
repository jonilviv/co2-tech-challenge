namespace TechChallenge.Calculator.Api.Configuration;

public class RetryPolicyConfiguration
{
    public int MaxRetries { get; set; } = 3;

    public double BaseDelaySeconds { get; set; } = 2.0;
}