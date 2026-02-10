using System.Text.Json;
using WeatherApi.Core.Domain.Entities;
using WeatherApi.Core.Ports;

namespace WeatherApi.Core.Infrastructure.Adapters.External;

public class OpenWeatherMapService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

    public OpenWeatherMapService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<WeatherData?> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/weather?q={city}&appid={_apiKey}&units=metric";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var json = JsonDocument.Parse(content);
        var root = json.RootElement;

        return new WeatherData
        {
            Id = Guid.NewGuid().ToString(),
            City = root.GetProperty("name").GetString() ?? city,
            Country = root.GetProperty("sys").GetProperty("country").GetString() ?? "",
            Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
            FeelsLike = root.GetProperty("main").GetProperty("feels_like").GetDouble(),
            Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
            WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
            Description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
            Icon = root.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "",
            Timestamp = DateTime.UtcNow,
            Source = "OpenWeatherMap"
        };
    }

    public async Task<WeatherForecast?> GetForecastAsync(string city, int days, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/forecast?q={city}&appid={_apiKey}&units=metric&cnt={days * 8}";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var json = JsonDocument.Parse(content);
        var root = json.RootElement;

        var forecastDays = new List<ForecastDay>();
        var dailyData = new Dictionary<DateTime, List<JsonElement>>();

        foreach (var item in root.GetProperty("list").EnumerateArray())
        {
            var dt = DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).DateTime;
            var date = dt.Date;

            if (!dailyData.ContainsKey(date))
            {
                dailyData[date] = new List<JsonElement>();
            }
            dailyData[date].Add(item);
        }

        foreach (var (date, items) in dailyData.Take(days))
        {
            var temps = items.Select(i => i.GetProperty("main").GetProperty("temp").GetDouble()).ToList();
            var forecastDay = new ForecastDay
            {
                Date = date,
                MinTemperature = temps.Min(),
                MaxTemperature = temps.Max(),
                Description = items[0].GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
                Humidity = items[0].GetProperty("main").GetProperty("humidity").GetInt32(),
                WindSpeed = items[0].GetProperty("wind").GetProperty("speed").GetDouble()
            };
            forecastDays.Add(forecastDay);
        }

        return new WeatherForecast
        {
            Id = Guid.NewGuid().ToString(),
            City = root.GetProperty("city").GetProperty("name").GetString() ?? city,
            Days = forecastDays,
            Timestamp = DateTime.UtcNow
        };
    }
}
