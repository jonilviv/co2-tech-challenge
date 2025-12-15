using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TechChallenge.Calculator.Api.Services;

public interface IEmissionsApiClient
{
    Task<IReadOnlyList<EmissionResponse>> GetEmissionsAsync(long from, long to, CancellationToken cancellationToken = default);
}