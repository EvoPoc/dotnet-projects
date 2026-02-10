using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using WeatherApi.Core.Domain.Entities;
using WeatherApi.Core.Ports;

namespace WeatherApi.Core.Infrastructure.Adapters.Persistence;

public class DynamoDbWeatherRepository : IWeatherRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;

    public DynamoDbWeatherRepository(IAmazonDynamoDB dynamoDb, string tableName)
    {
        _dynamoDb = dynamoDb;
        _tableName = tableName;
    }

    public async Task<WeatherData?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id } }
            }
        };

        var response = await _dynamoDb.GetItemAsync(request, cancellationToken);

        return response.Item.Count > 0 ? MapToWeatherData(response.Item) : null;
    }

    public async Task<WeatherData?> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        var request = new QueryRequest
        {
            TableName = _tableName,
            IndexName = "CityIndex",
            KeyConditionExpression = "City = :city",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":city", new AttributeValue { S = city } }
            },
            ScanIndexForward = false,
            Limit = 1
        };

        var response = await _dynamoDb.QueryAsync(request, cancellationToken);

        return response.Items.Count > 0 ? MapToWeatherData(response.Items[0]) : null;
    }

    public async Task<IEnumerable<WeatherData>> GetRecentWeatherDataAsync(int count, CancellationToken cancellationToken = default)
    {
        var request = new ScanRequest
        {
            TableName = _tableName,
            Limit = count
        };

        var response = await _dynamoDb.ScanAsync(request, cancellationToken);

        return response.Items
            .Select(MapToWeatherData)
            .OrderByDescending(w => w.Timestamp)
            .Take(count);
    }

    public async Task SaveAsync(WeatherData weatherData, CancellationToken cancellationToken = default)
    {
        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = weatherData.Id } },
                { "City", new AttributeValue { S = weatherData.City } },
                { "Country", new AttributeValue { S = weatherData.Country } },
                { "Temperature", new AttributeValue { N = weatherData.Temperature.ToString() } },
                { "FeelsLike", new AttributeValue { N = weatherData.FeelsLike.ToString() } },
                { "Humidity", new AttributeValue { N = weatherData.Humidity.ToString() } },
                { "WindSpeed", new AttributeValue { N = weatherData.WindSpeed.ToString() } },
                { "Description", new AttributeValue { S = weatherData.Description } },
                { "Icon", new AttributeValue { S = weatherData.Icon } },
                { "Timestamp", new AttributeValue { S = weatherData.Timestamp.ToString("o") } },
                { "Source", new AttributeValue { S = weatherData.Source } }
            }
        };

        await _dynamoDb.PutItemAsync(request, cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id } }
            }
        };

        await _dynamoDb.DeleteItemAsync(request, cancellationToken);
    }

    private static WeatherData MapToWeatherData(Dictionary<string, AttributeValue> item)
    {
        return new WeatherData
        {
            Id = item["Id"].S,
            City = item["City"].S,
            Country = item["Country"].S,
            Temperature = double.Parse(item["Temperature"].N),
            FeelsLike = double.Parse(item["FeelsLike"].N),
            Humidity = int.Parse(item["Humidity"].N),
            WindSpeed = double.Parse(item["WindSpeed"].N),
            Description = item["Description"].S,
            Icon = item["Icon"].S,
            Timestamp = DateTime.Parse(item["Timestamp"].S),
            Source = item["Source"].S
        };
    }
}
