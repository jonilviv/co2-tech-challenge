using System.Collections.Generic;

namespace TechChallenge.Calculator.Api.Services;

public interface ICalculationService
{
    double CalculateTotalEmissions(IReadOnlyList<MeasurementResponse> measurements, IReadOnlyList<EmissionResponse> emissions);
}