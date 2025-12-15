using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;
using TechChallenge.Calculator.Api.Configuration;

namespace TechChallenge.Calculator.Api.Services;

public class RetryPolicyFactory : IRetryPolicyFactory
{
    private readonly RetryPolicyConfiguration _configuration;

    public RetryPolicyFactory(IOptions<RetryPolicyConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public IAsyncPolicy<HttpResponseMessage> CreateHttpRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(
                retryCount: _configuration.MaxRetries,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(_configuration.BaseDelaySeconds, retryAttempt)));
    }
}