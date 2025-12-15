using System.Threading;
using System.Threading.Tasks;
using TechChallenge.Calculator.Api.DTOs;

namespace TechChallenge.Calculator.Api.Services;

public interface IEmissionsCalculationOrchestrator
{
    Task<CalculateEmissionsResponse> CalculateEmissionsAsync(CalculateEmissionsRequest request, CancellationToken cancellationToken = default);
}