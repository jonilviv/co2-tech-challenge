using TechChallenge.Calculator.Api.DTOs;

namespace TechChallenge.Calculator.Api.Validation;

public interface IRequestValidator
{
    ValidationResult Validate(CalculateEmissionsRequest request);
}