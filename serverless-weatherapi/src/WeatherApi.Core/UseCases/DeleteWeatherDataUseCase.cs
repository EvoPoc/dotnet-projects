using WeatherApi.Core.Ports;

namespace WeatherApi.Core.UseCases;

public class DeleteWeatherDataUseCase
{
    private readonly IWeatherRepository _repository;

    public DeleteWeatherDataUseCase(IWeatherRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("ID cannot be empty", nameof(id));
        }

        await _repository.DeleteAsync(id, cancellationToken);
    }
}
