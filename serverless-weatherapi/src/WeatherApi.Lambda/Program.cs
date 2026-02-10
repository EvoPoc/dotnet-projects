using Amazon.Lambda.AspNetCoreServer;
using WeatherApi.Infrastructure;
using WeatherApi.Core.UseCases;
using WeatherApi.Lambda.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Lambda support
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Add Infrastructure and Use Cases
var tableName = Environment.GetEnvironmentVariable("TABLE_NAME") ?? "WeatherData";
var weatherApiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");
builder.Services.AddInfrastructure(tableName, weatherApiKey);

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Weather API",
        Version = "v1",
        Description = "Serverless Weather API built with .NET 8 and AWS Lambda"
    });
});

var app = builder.Build();

// Log basic HTTP request/response details
app.UseMiddleware<RequestLoggingMiddleware>();

// Root endpoint - redirect to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
    .ExcludeFromDescription();

// Configure Swagger UI on dedicated endpoint
app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API V1");
    c.RoutePrefix = "swagger";
});

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health")
    .Produces(200);

// Weather endpoints
app.MapGet("/api/weather/current/{city}", async (string city, GetCurrentWeatherUseCase useCase, CancellationToken ct) =>
{
    try
    {
        var weather = await useCase.ExecuteAsync(city, ct);
        return weather != null ? Results.Ok(weather) : Results.NotFound(new { message = $"Weather data not found for city: {city}" });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
})
.WithName("GetCurrentWeather")
.WithTags("Weather")
.Produces(200)
.Produces(404)
.Produces(400);

app.MapGet("/api/weather/forecast/{city}", async (string city, int days, GetWeatherForecastUseCase useCase, CancellationToken ct) =>
{
    try
    {
        var forecast = await useCase.ExecuteAsync(city, days, ct);
        return forecast != null ? Results.Ok(forecast) : Results.NotFound(new { message = $"Forecast not found for city: {city}" });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
})
.WithName("GetWeatherForecast")
.WithTags("Weather")
.Produces(200)
.Produces(404)
.Produces(400);

app.MapGet("/api/weather/history", async (int count, GetWeatherHistoryUseCase useCase, CancellationToken ct) =>
{
    try
    {
        var history = await useCase.ExecuteAsync(count, ct);
        return Results.Ok(history);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
})
.WithName("GetWeatherHistory")
.WithTags("Weather")
.Produces(200)
.Produces(400);

app.MapDelete("/api/weather/{id}", async (string id, DeleteWeatherDataUseCase useCase, CancellationToken ct) =>
{
    try
    {
        await useCase.ExecuteAsync(id, ct);
        return Results.NoContent();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
})
.WithName("DeleteWeatherData")
.WithTags("Weather")
.Produces(204)
.Produces(400);

app.Run();
