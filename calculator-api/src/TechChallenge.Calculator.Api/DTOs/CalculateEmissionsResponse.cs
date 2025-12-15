namespace TechChallenge.Calculator.Api.DTOs;

public record CalculateEmissionsResponse
{
    public CalculateEmissionsResponse(double totalEmissionsKg, string userId, long from, long to)
    {
        TotalEmissionsKg = totalEmissionsKg;
        UserId = userId;
        From = from;
        To = to;
    }

    public double TotalEmissionsKg { get; init; }
    public string UserId { get; init; }
    public long From { get; init; }
    public long To { get; init; }

    public void Deconstruct(out double totalEmissionsKg, out string userId, out long from, out long to)
    {
        totalEmissionsKg = TotalEmissionsKg;
        userId = UserId;
        from = From;
        to = To;
    }
}