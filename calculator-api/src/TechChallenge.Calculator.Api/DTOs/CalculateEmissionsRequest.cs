namespace TechChallenge.Calculator.Api.DTOs;

public record CalculateEmissionsRequest
{
    public CalculateEmissionsRequest(string userId, long from, long to)
    {
        UserId = userId;
        From = from;
        To = to;
    }

    public string UserId { get; init; }
    public long From { get; init; }
    public long To { get; init; }

    public void Deconstruct(out string userId, out long from, out long to)
    {
        userId = UserId;
        from = From;
        to = To;
    }
}