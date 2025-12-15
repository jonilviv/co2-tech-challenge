using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.Net.Http;
using TechChallenge.Calculator.Api.Configuration;
using TechChallenge.Calculator.Api.Services;
using TechChallenge.Calculator.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure API options
builder.Services.Configure<MeasurementsApiOptions>(builder.Configuration.GetSection("MeasurementsApi"));
builder.Services.Configure<EmissionsApiOptions>(builder.Configuration.GetSection("EmissionsApi"));

// Configure retry policies
builder.Services.Configure<MeasurementsApiRetryPolicyConfiguration>(builder.Configuration.GetSection("RetryPolicy:MeasurementsApi"));
builder.Services.Configure<EmissionsApiRetryPolicyConfiguration>(builder.Configuration.GetSection("RetryPolicy:EmissionsApi"));
builder.Services.Configure<RetryPolicyConfiguration>(builder.Configuration.GetSection("RetryPolicy:Default"));

// Register retry policy factory
builder.Services.AddSingleton<IRetryPolicyFactory, RetryPolicyFactory>();

// Configure HTTP clients with retry policies
builder.Services.AddHttpClient<IMeasurementsApiClient, MeasurementsApiClient>()
    .AddPolicyHandler((serviceProvider, _) =>
    {
        IRetryPolicyFactory factory = serviceProvider.GetRequiredService<IRetryPolicyFactory>();
        IAsyncPolicy<HttpResponseMessage> httpRetryPolicy = factory.CreateHttpRetryPolicy();

        return httpRetryPolicy;
    });

builder.Services.AddHttpClient<IEmissionsApiClient, EmissionsApiClient>()
    .AddPolicyHandler((serviceProvider, _) =>
    {
        IRetryPolicyFactory factory = serviceProvider.GetRequiredService<IRetryPolicyFactory>();
        IAsyncPolicy<HttpResponseMessage> httpRetryPolicy = factory.CreateHttpRetryPolicy();

        return httpRetryPolicy;
    });

// Register services
builder.Services.AddScoped<ICalculationService, CalculationService>();
builder.Services.AddScoped<IEmissionsCalculationOrchestrator, EmissionsCalculationOrchestrator>();
builder.Services.AddScoped<IRequestValidator, RequestValidator>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();