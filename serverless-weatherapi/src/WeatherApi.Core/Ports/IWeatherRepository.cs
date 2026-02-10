using WeatherApi.Core.Domain.Entities;

namespace WeatherApi.Core.Ports;

/// <summary>
/// Repository port for persisting weather data
/// </summary>
public interface IWeatherRepository
{
    Task<WeatherData?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<WeatherData?> GetByCityAsync(string city, CancellationToken cancellationToken = default);
    Task<IEnumerable<WeatherData>> GetRecentWeatherDataAsync(int count, CancellationToken cancellationToken = default);
    Task SaveAsync(WeatherData weatherData, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
