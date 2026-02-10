using WeatherApi.Core.Domain.Entities;
using WeatherApi.Core.Ports;

namespace WeatherApi.Core.UseCases;

public class GetWeatherForecastUseCase
{
    private readonly IWeatherService _weatherService;

    public GetWeatherForecastUseCase(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<WeatherForecast?> ExecuteAsync(string city, int days = 5, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty", nameof(city));
        }

        if (days < 1 || days > 10)
        {
            throw new ArgumentException("Days must be between 1 and 10", nameof(days));
        }

        return await _weatherService.GetForecastAsync(city, days, cancellationToken);
    }
}
