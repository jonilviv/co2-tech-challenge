using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechChallenge.Calculator.Api.DTOs;
using TechChallenge.Calculator.Api.Services;
using TechChallenge.Calculator.Api.Validation;

namespace TechChallenge.Calculator.Api.Controllers;

[ApiController]
[Produces("application/json")]
public class EmissionsController : ControllerBase
{
    private readonly IEmissionsCalculationOrchestrator _orchestrator;
    private readonly IRequestValidator _validator;
    private readonly ILogger<EmissionsController> _logger;

    public EmissionsController(
        IEmissionsCalculationOrchestrator orchestrator,
        IRequestValidator validator,
        ILogger<EmissionsController> logger)
    {
        _orchestrator = orchestrator;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Calculates total CO2 emissions for a user within a specified timeframe
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="from">Start timestamp (Unix seconds)</param>
    /// <param name="to">End timestamp (Unix seconds)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total CO2 emissions in kg</returns>
    [HttpGet("/calculate/{userId}")]
    [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CalculateEmissions(
        [FromRoute] string userId,
        [FromQuery] long from,
        [FromQuery] long to,
        CancellationToken cancellationToken)
    {
        var dictionary = new Dictionary<string, object>
        {
            ["userId"] = userId,
            ["from"] = from,
            ["to"] = to,
        };

        using (_logger.BeginScope(dictionary))
        {
            var request = new CalculateEmissionsRequest(userId, from, to);
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for user {UserId}: {ErrorMessage}", userId, validationResult.ErrorMessage);

                return BadRequest(new ErrorResponse(validationResult.ErrorMessage!));
            }

            try
            {
                CalculateEmissionsResponse response = await _orchestrator.CalculateEmissionsAsync(request, cancellationToken);

                return Ok(response.TotalEmissionsKg);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling external API for user {UserId}", userId);

                var errorResponse = new ErrorResponse("Error communicating with external services", ex.Message);

                return StatusCode(StatusCodes.Status502BadGateway, errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calculating emissions for user {UserId}", userId);
                var errorResponse = new ErrorResponse("An unexpected error occurred", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}