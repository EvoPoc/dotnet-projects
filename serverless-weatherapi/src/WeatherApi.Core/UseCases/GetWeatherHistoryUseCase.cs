using WeatherApi.Core.Domain.Entities;
using WeatherApi.Core.Ports;

namespace WeatherApi.Core.UseCases;

public class GetWeatherHistoryUseCase
{
    private readonly IWeatherRepository _repository;

    public GetWeatherHistoryUseCase(IWeatherRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<WeatherData>> ExecuteAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        if (count < 1 || count > 100)
        {
            throw new ArgumentException("Count must be between 1 and 100", nameof(count));
        }

        return await _repository.GetRecentWeatherDataAsync(count, cancellationToken);
    }
}
