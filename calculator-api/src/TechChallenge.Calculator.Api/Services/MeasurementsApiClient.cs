using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TechChallenge.Calculator.Api.Configuration;

namespace TechChallenge.Calculator.Api.Services;

public class MeasurementsApiClient : IMeasurementsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly ILogger<MeasurementsApiClient> _logger;

    public MeasurementsApiClient(
        HttpClient httpClient,
        IOptions<MeasurementsApiOptions> options,
        IOptions<MeasurementsApiRetryPolicyConfiguration> retryConfig,
        ILogger<MeasurementsApiClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        _logger = logger;

        MeasurementsApiRetryPolicyConfiguration config = retryConfig.Value;
        // Retry policy for handling 30% failure rate
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: config.MaxRetries,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(config.BaseDelaySeconds, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    string warnMsg = $"Retry {retryCount} after {timespan.TotalMilliseconds}ms for Measurements API";
                    _logger.LogWarning(warnMsg);
                });
    }

    public async Task<IReadOnlyList<MeasurementResponse>> GetMeasurementsAsync(string userId, long from, long to, CancellationToken cancellationToken = default)
    {
        var requestUri = $"/measurements/{userId}?from={from}&to={to}";

        using HttpResponseMessage response = await _retryPolicy.ExecuteAsync(async () =>
          {
              _logger.LogInformation("Requesting measurements for user {UserId} from {From} to {To}", userId, from, to);
              HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(requestUri, cancellationToken);

              return httpResponseMessage;
          });

        response.EnsureSuccessStatusCode();

        IReadOnlyList<MeasurementResponse>? measurements = await response.Content.ReadFromJsonAsync<IReadOnlyList<MeasurementResponse>>(cancellationToken: cancellationToken);

        IReadOnlyList<MeasurementResponse> readOnlyList = measurements ?? Array.Empty<MeasurementResponse>();

        return readOnlyList;
    }
}