using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TechChallenge.Calculator.Api.DTOs;

namespace TechChallenge.Calculator.Api.Services;

public class EmissionsCalculationOrchestrator : IEmissionsCalculationOrchestrator
{
    private readonly IMeasurementsApiClient _measurementsClient;
    private readonly IEmissionsApiClient _emissionsClient;
    private readonly ICalculationService _calculationService;
    private readonly ILogger<EmissionsCalculationOrchestrator> _logger;

    public EmissionsCalculationOrchestrator(
        IMeasurementsApiClient measurementsClient,
        IEmissionsApiClient emissionsClient,
        ICalculationService calculationService,
        ILogger<EmissionsCalculationOrchestrator> logger)
    {
        _measurementsClient = measurementsClient;
        _emissionsClient = emissionsClient;
        _calculationService = calculationService;
        _logger = logger;
    }

    public async Task<CalculateEmissionsResponse> CalculateEmissionsAsync(
        CalculateEmissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Calculating emissions for user {UserId} from {From} to {To}",
            request.UserId,
            request.From,
            request.To);

        // Fetch measurements and emissions in parallel
        var measurementsTask = _measurementsClient.GetMeasurementsAsync(
            request.UserId,
            request.From,
            request.To,
            cancellationToken);

        var emissionsTask = _emissionsClient.GetEmissionsAsync(
            request.From,
            request.To,
            cancellationToken);

        await Task.WhenAll(measurementsTask, emissionsTask);

        var measurements = await measurementsTask;
        var emissions = await emissionsTask;

        if (measurements.Count == 0)
        {
            _logger.LogWarning(
                "No measurements found for user {UserId} in the specified timeframe",
                request.UserId);

            return new CalculateEmissionsResponse(
                totalEmissionsKg: 0.0,
                userId: request.UserId,
                from: request.From,
                to: request.To);
        }

        // Calculate total emissions
        var totalEmissions = _calculationService.CalculateTotalEmissions(measurements, emissions);

        _logger.LogInformation(
            "Calculated total emissions: {TotalEmissions} kg CO2 for user {UserId}",
            totalEmissions,
            request.UserId);

        return new CalculateEmissionsResponse(
            totalEmissionsKg: totalEmissions,
            userId: request.UserId,
            from: request.From,
            to: request.To);
    }
}