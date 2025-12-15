namespace TechChallenge.Calculator.Api.Validation
{
    public record ValidationResult
    {
        public ValidationResult(bool isValid, string? errorMessage = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public bool IsValid { get; init; }
        public string? ErrorMessage { get; init; }

        public void Deconstruct(out bool isValid, out string? errorMessage)
        {
            isValid = IsValid;
            errorMessage = ErrorMessage;
        }
    }
}