using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TechChallenge.Calculator.Api.Services;

public interface IMeasurementsApiClient
{
    Task<IReadOnlyList<MeasurementResponse>> GetMeasurementsAsync(string userId, long from, long to, CancellationToken cancellationToken = default);
}