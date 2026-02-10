namespace WeatherApi.Core.Domain.Entities;

public class WeatherData
{
    public string Id { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Source { get; set; } = string.Empty;
}
