using Polly;
using System.Net.Http;

namespace TechChallenge.Calculator.Api.Services;

public interface IRetryPolicyFactory
{
    IAsyncPolicy<HttpResponseMessage> CreateHttpRetryPolicy();
}