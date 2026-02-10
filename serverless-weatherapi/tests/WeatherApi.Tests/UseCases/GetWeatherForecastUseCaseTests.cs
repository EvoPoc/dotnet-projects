using FluentAssertions;
using Moq;
using WeatherApi.Core.Domain.Entities;
using WeatherApi.Core.Ports;
using WeatherApi.Core.UseCases;

namespace WeatherApi.Tests.UseCases;

public class GetWeatherForecastUseCaseTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly GetWeatherForecastUseCase _useCase;

    public GetWeatherForecastUseCaseTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _useCase = new GetWeatherForecastUseCase(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCity_ReturnsForecast()
    {
        // Arrange
        var city = "Berlin";
        var days = 5;
        var expectedForecast = new WeatherForecast
        {
            Id = "1",
            City = city,
            Days = new List<ForecastDay>
            {
                new() { Date = DateTime.Today.AddDays(1), MinTemperature = 10, MaxTemperature = 20 }
            },
            Timestamp = DateTime.UtcNow
        };

        _weatherServiceMock.Setup(s => s.GetForecastAsync(city, days, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedForecast);

        // Act
        var result = await _useCase.ExecuteAsync(city, days);

        // Assert
        result.Should().NotBeNull();
        result!.City.Should().Be(city);
        result.Days.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public async Task ExecuteAsync_WithInvalidDays_ThrowsArgumentException(int days)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync("Madrid", days));
    }
}
