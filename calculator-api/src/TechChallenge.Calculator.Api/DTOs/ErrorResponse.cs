namespace TechChallenge.Calculator.Api.DTOs;

public record ErrorResponse
{
    public ErrorResponse(string message, string? details = null)
    {
        Message = message;
        Details = details;
    }

    public string Message { get; init; }
    public string? Details { get; init; }

    public void Deconstruct(out string message, out string? details)
    {
        message = Message;
        details = Details;
    }
}