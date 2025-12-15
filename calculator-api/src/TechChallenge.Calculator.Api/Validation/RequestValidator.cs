using TechChallenge.Calculator.Api.DTOs;

namespace TechChallenge.Calculator.Api.Validation;

public class RequestValidator : IRequestValidator
{
    public ValidationResult Validate(CalculateEmissionsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return new ValidationResult(false, "UserId is required");
        }

        if (request.From >= request.To)
        {
            return new ValidationResult(false, "Invalid request time frame: 'from' must be less than 'to'");
        }

        if (request.From < 0 || request.To < 0)
        {
            return new ValidationResult(false, "Timestamps must be non-negative");
        }

        return new ValidationResult(true);
    }
}