using WeatherApi.Core.Domain.Entities;

namespace WeatherApi.Core.Ports;

/// <summary>
/// External weather service port for fetching real-time weather data
/// </summary>
public interface IWeatherService
{
    Task<WeatherData?> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken = default);
    Task<WeatherForecast?> GetForecastAsync(string city, int days, CancellationToken cancellationToken = default);
}
