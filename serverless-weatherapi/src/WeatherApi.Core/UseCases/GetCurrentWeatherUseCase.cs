using WeatherApi.Core.Domain.Entities;
using WeatherApi.Core.Ports;

namespace WeatherApi.Core.UseCases;

public class GetCurrentWeatherUseCase
{
    private readonly IWeatherService _weatherService;
    private readonly IWeatherRepository _repository;

    public GetCurrentWeatherUseCase(IWeatherService weatherService, IWeatherRepository repository)
    {
        _weatherService = weatherService;
        _repository = repository;
    }

    public async Task<WeatherData?> ExecuteAsync(string city, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty", nameof(city));
        }

        // Try to get from cache/repository first
        var cachedData = await _repository.GetByCityAsync(city, cancellationToken);
        if (cachedData != null && (DateTime.UtcNow - cachedData.Timestamp).TotalMinutes < 30)
        {
            return cachedData;
        }

        // Fetch from external service
        var weatherData = await _weatherService.GetCurrentWeatherAsync(city, cancellationToken);

        if (weatherData != null)
        {
            // Cache the new data
            await _repository.SaveAsync(weatherData, cancellationToken);
        }

        return weatherData;
    }
}
