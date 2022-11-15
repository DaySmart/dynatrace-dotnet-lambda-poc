using Dynatrace.OpenTelemetry;
using Dynatrace.OpenTelemetry.Instrumentation.AwsLambda;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Scheduling.Application.Interfaces;
using Scheduling.Application.Mappings;
using Scheduling.Application.Services;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AWSLambda;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scheduling.Infrastructure.Interfaces;
using Scheduling.Infrastructure.Repositories;
using Scheduling.Reservation.Extensions;
using Scheduling.Reservation.Interfaces;
using Scheduling.Reservation.Models;
using Scheduling.Reservation.Services;

var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true)
    .AddEnvironmentVariables();

#region DI Container Registration

// Bootstrap Dependency Injection Container

// Grab reference to container
var services = builder.Services;

// Add IHttpContextAccessor resolver
services.AddHttpContextAccessor();
services.AddSwaggerGen();

services.AddAutoMapper(typeof(ApplicationMappingsProfile));
// Define an OpenTelemetry resource
// A resource represents a collection of attributes describing the
// service. This collection of attributes will be associated with all
// telemetry generated from this service (traces, metrics, logs).
var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService("ds-scheduling-reservation")
    .AddTelemetrySdk();
 
services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    // Step 1. Declare the resource to be used by this tracer provider.
    tracerProviderBuilder
        .SetResourceBuilder(resourceBuilder)
        .AddAWSLambdaConfigurations(c => c.DisableAwsXRayContextExtraction = true)
        .AddAWSInstrumentation(c => c.SuppressDownstreamInstrumentation = true);

    // Step 2. Configure the SDK to listen to the following auto-instrumentation
    tracerProviderBuilder
        .AddAspNetCoreInstrumentation(options =>
        {
            options.RecordException = true;
        })
        .AddHttpClientInstrumentation();
 
    //Add Dynatrace exporter
    tracerProviderBuilder.AddDynatrace()
        .AddDynatraceAwsSdkInjection()
        .AddOtlpExporter(exporterOptions =>
        {
            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
            exporterOptions.Headers = $"Authorization=Api-Token {Environment.GetEnvironmentVariable("DT_CONNECTION_AUTH_TOKEN")}";
            exporterOptions.Endpoint = new Uri("https://ivw36740.live.dynatrace.com/api/v2/otlp/v1/traces");
        });
});

// Configure the OpenTelemetry SDK for metrics
services.AddOpenTelemetryMetrics(meterProviderBuilder =>
{
    // Step 1. Declare the resource to be used by this meter provider.
    meterProviderBuilder
        .SetResourceBuilder(resourceBuilder);

    // Step 2. Configure the SDK to listen to the following auto-instrumentation
    meterProviderBuilder
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation();
 
    //Add Dynatrace exporter
    meterProviderBuilder
        .AddOtlpExporter(exporterOptions =>
        {
            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
            exporterOptions.Headers = $"Authorization=Api-Token {Environment.GetEnvironmentVariable("DT_CONNECTION_AUTH_TOKEN")}";
            exporterOptions.Endpoint = new Uri("https://ivw36740.live.dynatrace.com/api/v2/otlp/v1/traces");
        });
});

// Configure the OpenTelemetry SDK for logs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.ParseStateValues = true;
    options.IncludeScopes = true;

    options
        .SetResourceBuilder(resourceBuilder);
     
    //Add Dynatrace exporter
    options
        .AddOtlpExporter(exporterOptions =>
        {
            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
            exporterOptions.Headers = $"Authorization=Api-Token {Environment.GetEnvironmentVariable("DT_CONNECTION_AUTH_TOKEN")}";
            exporterOptions.Endpoint = new Uri("https://ivw36740.live.dynatrace.com/api/v2/otlp/v1/traces");
        });
});

// Add Controllers to the container.
services.AddControllers();

services.Configure<AwsConfigurationSettings>(
    builder.Configuration.GetSection(AwsConfigurationSettings.SettingsKey));

// services.AddAwsSpecificDependencies();
// services.AddSingleton<ICloudConfiguration, AwsCloudConfiguration>();

// Setup container with app services
services.AddScoped<IReservationService, ReservationService>();
services.AddScoped<IAvailabilityService, AvailabilityService>();
services.AddScoped<IReservationRepository, ReservationRepository>();


// Setup configuration sections for use with IOptions
services.Configure<ReservationDatabaseSettings>(
    builder.Configuration.GetSection(ReservationDatabaseSettings.SettingsKey));

services.AddRepositoryServices();

BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.Document));

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

#endregion

#region Configure Application

// Configure Application

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Scheduling Availability Service");

app.Run();

#endregion
