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

public class EmissionsApiClient : IEmissionsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly ILogger<EmissionsApiClient> _logger;

    public EmissionsApiClient(
        HttpClient httpClient,
        IOptions<EmissionsApiOptions> options,
        IOptions<EmissionsApiRetryPolicyConfiguration> retryConfig,
        ILogger<EmissionsApiClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        EmissionsApiRetryPolicyConfiguration config = retryConfig.Value;
        _httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
        _logger = logger;

        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: config.MaxRetries,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(config.BaseDelaySeconds, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Retry {RetryCount} after {Delay}ms for Emissions API",
                        retryCount,
                        timespan.TotalMilliseconds);
                });
    }

    public async Task<IReadOnlyList<EmissionResponse>> GetEmissionsAsync(long from, long to, CancellationToken cancellationToken = default)
    {
        string requestUri = $"/emissions?from={from}&to={to}";

        var response = await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Requesting emissions from {From} to {To}", from, to);
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(requestUri, cancellationToken);

            return httpResponseMessage;
        });

        response.EnsureSuccessStatusCode();

        IReadOnlyList<EmissionResponse>? emissions = await response.Content.ReadFromJsonAsync<IReadOnlyList<EmissionResponse>>(cancellationToken: cancellationToken);

        return emissions ?? Array.Empty<EmissionResponse>();
    }
}