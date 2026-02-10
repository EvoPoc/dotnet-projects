namespace WeatherApi.Core.Domain.Entities;

public class WeatherForecast
{
    public string Id { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public List<ForecastDay> Days { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

public class ForecastDay
{
    public DateTime Date { get; set; }
    public double MinTemperature { get; set; }
    public double MaxTemperature { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
}
