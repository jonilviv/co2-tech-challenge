# Implementation Notes

## Overview
This document contains implementation details, assumptions, and important notes about the Calculator API solution.

## Architecture

### Calculator API
The Calculator API is a .NET 8.0 minimal API project that:
- Consumes the Measurements API to get energy consumption data (Watts)
- Consumes the Emissions API to get CO2 emission factors (kg per kWh)
- Calculates total CO2 emissions for a user within a specified timeframe
- Handles chaos scenarios (failures and delays) with retry policies

### Key Components

1. **HTTP Clients** (`MeasurementsApiClient`, `EmissionsApiClient`)
   - Implement retry logic using Polly
   - Handle transient failures and timeouts
   - Measurements API: 5 retries with exponential backoff (handles 30% failure rate)
   - Emissions API: 3 retries with extended timeout (30s) to handle 15s delays

2. **Calculation Service** (`CalculationService`)
   - Groups measurements into 15-minute periods
   - Calculates average watts per period
   - Converts to kWh: `averageWatts / 4 / 1000` (15 mins = 1/4 hour, W to kW)
   - Multiplies kWh by emission factor to get CO2 per period
   - Sums all periods to get total emissions

3. **API Endpoint** (`/calculate/{userId}`)
   - Accepts: `userId`, `from` (timestamp), `to` (timestamp)
   - Returns: total CO2 emissions in kg (double)
   - Fetches measurements and emissions in parallel for performance

## Assumptions

1. **Emission Factor Units**: The API response field is named `KgPerWattHr`, but based on the README specification ("kg per kWh"), I assume the actual value represents kg per kWh, not kg per watt-hour.

2. **15-Minute Alignment**: Measurements are grouped into 15-minute periods aligned to boundaries (e.g., 14:00, 14:15, 14:30, etc.). The period start timestamp is used to match with emission factors.

3. **Missing Emission Factors**: If an exact emission factor timestamp is not found for a period, the closest available emission factor is used. If no emission factors are available, that period is skipped with a warning.

4. **Resolution Consistency**: As stated in the README, we assume the measurement resolution does not change for each user and there are no missing data points.

5. **Parallel API Calls**: Measurements and emissions are fetched in parallel to optimize performance, especially for long timeframes (up to 2 weeks).

## Chaos Handling

### Measurements API (30% failure rate)
- Implemented retry policy with 5 attempts
- Exponential backoff: 2s, 4s, 8s, 16s, 32s
- Handles HTTP errors and transient failures

### Emissions API (50% chance of 15s delay)
- Extended HTTP client timeout to 30 seconds
- Retry policy with 3 attempts
- Exponential backoff: 2s, 4s, 8s
- Handles timeouts and HTTP errors

## Docker Configuration

### Fixed Issues
1. **Emissions API Dockerfile**: Changed base image from .NET 6.0 to 8.0 for consistency
2. **Measurements API Dockerfile**: Added missing `WORKDIR /src` before COPY command

### Docker Compose
- All services run on a shared network (`co2-network`)
- Calculator API depends on both Measurements and Emissions APIs
- Port mappings:
  - Measurements API: 5153:8080
  - Emissions API: 5139:8080
  - Calculator API: 5173:8080

## Performance Optimizations

1. **Parallel API Calls**: Measurements and emissions are fetched simultaneously
2. **Efficient Grouping**: Measurements are grouped into periods in a single pass
3. **Dictionary Lookup**: Emission factors are stored in a dictionary for O(1) lookup
4. **Early Returns**: Empty measurements return 0.0 immediately

## Testing Considerations

- The API can be tested using Swagger UI at `/swagger`
- Example endpoint: `GET /calculate/alpha?from=1000000&to=2000000`
- For local development, ensure Measurements API runs on port 5153 and Emissions API on port 5139
- For Docker, use service names: `measurements-api:8080` and `emissions-api:8080`

## Future Improvements

1. Caching emission factors (they change every 15 minutes, so cache could be useful for repeated requests)
2. Batch processing for very long timeframes
3. Streaming responses for large datasets
4. More sophisticated error handling and circuit breakers
5. Metrics and observability (OpenTelemetry)

