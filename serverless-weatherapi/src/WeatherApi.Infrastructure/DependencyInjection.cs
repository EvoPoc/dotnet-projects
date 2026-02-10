using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Core.Ports;
using WeatherApi.Core.Infrastructure.Adapters.Persistence;
using WeatherApi.Core.Infrastructure.Adapters.External;
using WeatherApi.Core.UseCases;

namespace WeatherApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string tableName,
        string? weatherApiKey = null)
    {
        // Register AWS Services
        services.AddAWSService<IAmazonDynamoDB>();

        // Register Repositories
        services.AddSingleton<IWeatherRepository>(sp =>
        {
            var dynamoDb = sp.GetRequiredService<IAmazonDynamoDB>();
            return new DynamoDbWeatherRepository(dynamoDb, tableName);
        });

        // Register External Services
        var apiKey = weatherApiKey ?? Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") ?? "";
        services.AddHttpClient<IWeatherService, OpenWeatherMapService>()
            .ConfigureHttpClient((sp, client) =>
            {
                // HTTP client configuration if needed
            });
        services.AddScoped<IWeatherService>(sp =>
        {
            var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(OpenWeatherMapService));
            return new OpenWeatherMapService(httpClient, apiKey);
        });

        // Register Use Cases
        services.AddScoped<GetCurrentWeatherUseCase>();
        services.AddScoped<GetWeatherForecastUseCase>();
        services.AddScoped<GetWeatherHistoryUseCase>();
        services.AddScoped<DeleteWeatherDataUseCase>();

        return services;
    }
}
