using FluentAssertions;
using Moq;
using WeatherApi.Core.Domain.Entities;
using WeatherApi.Core.Ports;
using WeatherApi.Core.UseCases;

namespace WeatherApi.Tests.UseCases;

public class GetCurrentWeatherUseCaseTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly Mock<IWeatherRepository> _repositoryMock;
    private readonly GetCurrentWeatherUseCase _useCase;

    public GetCurrentWeatherUseCaseTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _repositoryMock = new Mock<IWeatherRepository>();
        _useCase = new GetCurrentWeatherUseCase(_weatherServiceMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCity_ReturnsWeatherData()
    {
        // Arrange
        var city = "London";
        var expectedWeather = new WeatherData
        {
            Id = "1",
            City = city,
            Country = "GB",
            Temperature = 15.5,
            Description = "Partly cloudy",
            Timestamp = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByCityAsync(city, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherData?)null);

        _weatherServiceMock.Setup(s => s.GetCurrentWeatherAsync(city, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWeather);

        // Act
        var result = await _useCase.ExecuteAsync(city);

        // Assert
        result.Should().NotBeNull();
        result!.City.Should().Be(city);
        result.Temperature.Should().Be(15.5);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<WeatherData>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyCity_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(string.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_WithCachedData_ReturnsCachedData()
    {
        // Arrange
        var city = "Paris";
        var cachedWeather = new WeatherData
        {
            Id = "2",
            City = city,
            Country = "FR",
            Temperature = 18.0,
            Description = "Sunny",
            Timestamp = DateTime.UtcNow.AddMinutes(-10) // Recent data
        };

        _repositoryMock.Setup(r => r.GetByCityAsync(city, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedWeather);

        // Act
        var result = await _useCase.ExecuteAsync(city);

        // Assert
        result.Should().Be(cachedWeather);
        _weatherServiceMock.Verify(s => s.GetCurrentWeatherAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
