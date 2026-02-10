# Serverless Weather API

A serverless weather API built with .NET 10, AWS Lambda, DynamoDB, and deployed using AWS CDK. This project demonstrates hexagonal architecture principles with clean separation of concerns.

## Architecture

This project follows **Hexagonal Architecture** (Ports and Adapters):

```
├── src/
│   ├── WeatherApi.Core/           # Domain & Use Cases (Business Logic)
│   │   ├── Domain/Entities/       # Domain models
│   │   ├── Ports/                 # Interfaces (Repository, Service)
│   │   └── UseCases/              # Application business logic
│   ├── WeatherApi.Infrastructure/ # Adapters (External implementations)
│   │   ├── Adapters/
│   │   │   ├── Persistence/       # DynamoDB repository
│   │   │   └── External/          # OpenWeatherMap API client
│   │   └── DependencyInjection.cs
│   └── WeatherApi.Lambda/         # API Entry Point (AWS Lambda)
│       └── Program.cs             # Minimal API endpoints
├── tests/
│   └── WeatherApi.Tests/          # Unit tests
└── infrastructure/                 # AWS CDK Infrastructure as Code
    ├── lib/
    │   └── weather-api-stack.ts   # CDK stack definition
    └── bin/
        └── app.ts                 # CDK app entry point
```

## Features

- ✅ **Hexagonal Architecture**: Clean separation between domain, application, and infrastructure layers
- ✅ **.NET 10 Minimal API**: Modern, performant API endpoints
- ✅ **AWS Lambda**: Serverless compute with automatic scaling
- ✅ **DynamoDB**: NoSQL database for weather data storage
- ✅ **Dependency Injection**: Built-in DI container
- ✅ **Swagger UI**: Interactive API documentation at `/swagger`
- ✅ **Unit Tests**: XUnit with Moq and FluentAssertions
- ✅ **Infrastructure as Code**: AWS CDK for reproducible deployments
- ✅ **OpenWeatherMap Integration**: Real-time weather data
- ✅ **Request Logging**: Middleware logs method, path, status, and duration

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 18+](https://nodejs.org/) (for AWS CDK)
- [AWS CLI](https://aws.amazon.com/cli/) configured with credentials
- [AWS CDK](https://docs.aws.amazon.com/cdk/latest/guide/getting_started.html) installed: `npm install -g aws-cdk`
- OpenWeatherMap API Key (get free key at [openweathermap.org](https://openweathermap.org/api))

## Quick Start

### 1. Clone and Setup

```bash
cd serverless-weatherapi
```

### 2. Build the .NET Solution

```bash
dotnet restore
dotnet build
dotnet test
```

### 3. Publish Lambda Function

```bash
cd src/WeatherApi.Lambda
dotnet publish -c Release -o bin/Release/net10.0/publish
cd ../..
```

### 4. Deploy Infrastructure with CDK

```bash
cd infrastructure
npm install

# Set your OpenWeatherMap API key
export OPENWEATHER_API_KEY="your_api_key_here"

# Deploy to AWS
cdk bootstrap  # First time only
cdk deploy
```

The CDK will output:
- **ApiUrl**: Your API endpoint
- **SwaggerUrl**: Swagger UI documentation
- **TableName**: DynamoDB table name

## API Endpoints

### Get Current Weather
```http
GET /api/weather/current/{city}
```

Example:
```bash
curl https://your-api-url/api/weather/current/London
```

### Get Weather Forecast
```http
GET /api/weather/forecast/{city}?days=5
```

Example:
```bash
curl "https://your-api-url/api/weather/forecast/Paris?days=5"
```

### Get Weather History
```http
GET /api/weather/history?count=10
```

### Delete Weather Data
```http
DELETE /api/weather/{id}
```

### Health Check
```http
GET /health
```

### Swagger UI
```http
GET /swagger
```

## Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `OPENWEATHER_API_KEY` | OpenWeatherMap API key | Yes |
| `TABLE_NAME` | DynamoDB table name | Yes (auto-set by CDK) |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET environment | No (defaults to Production) |

## Development

### Running Tests

```bash
dotnet test
```

### Local Development

For local testing (requires AWS credentials configured):

```bash
cd src/WeatherApi.Lambda
export OPENWEATHER_API_KEY="your_key"
export TABLE_NAME="WeatherData"
dotnet run
```

### CDK Commands

```bash
cd infrastructure

# Show what will be deployed
cdk diff

# Deploy stack
cdk deploy

# Destroy stack
cdk destroy

# Synthesize CloudFormation template
cdk synth
```

## Project Structure Explained

### Core Layer (Domain)
- **Entities**: `WeatherData`, `WeatherForecast` - Domain models
- **Ports**: Interfaces defining contracts (`IWeatherRepository`, `IWeatherService`)
- **Use Cases**: Business logic (`GetCurrentWeatherUseCase`, `GetWeatherForecastUseCase`)

### Infrastructure Layer (Adapters)
- **DynamoDbWeatherRepository**: DynamoDB implementation of `IWeatherRepository`
- **OpenWeatherMapService**: External API client implementing `IWeatherService`
- **DependencyInjection**: Wires up all dependencies

### API Layer (Lambda)
- **Program.cs**: Minimal API endpoints with Swagger configuration
- **RequestLoggingMiddleware**: Logs request and response details
- Maps HTTP requests to use cases
- Handles serialization, validation, error responses

## Technology Stack

- **.NET 10**: Latest version with minimal API
- **AWS Lambda**: Serverless compute
- **Amazon DynamoDB**: Managed NoSQL database
- **API Gateway HTTP API**: API endpoint management
- **AWS CDK**: Infrastructure as Code (TypeScript)
- **Swashbuckle**: OpenAPI/Swagger documentation
- **XUnit**: Testing framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library

## Design Patterns

- **Hexagonal Architecture (Ports & Adapters)**
- **Dependency Injection**
- **Repository Pattern**
- **Use Case Pattern**
- **Factory Pattern** (HttpClient)

## Performance Considerations

- Caching: Weather data is cached in DynamoDB for 30 minutes
- Lambda cold starts: ~1-2 seconds (optimized with AWS Lambda SnapStart compatible)
- DynamoDB: On-demand billing for cost optimization
- API Gateway: HTTP API for lower latency than REST API

## Cost Estimation

With AWS Free Tier:
- Lambda: 1M requests/month free
- DynamoDB: 25GB storage + 200M requests/month free
- API Gateway: 1M requests/month free (12 months)

Estimated cost after free tier: ~$1-5/month for low-moderate traffic

## Security

- IAM roles with least-privilege permissions
- API key for OpenWeatherMap stored as environment variable
- CORS enabled for frontend integration
- DynamoDB encryption at rest (default)

## Monitoring

- AWS CloudWatch Logs: Lambda execution logs
- AWS X-Ray: Distributed tracing (enabled)
- CloudWatch Metrics: Lambda invocations, duration, errors

## Contributing

1. Follow hexagonal architecture principles
2. Add tests for new use cases
3. Update CDK stack for infrastructure changes
4. Document API changes in Swagger annotations

## License

MIT License

## Author

Evopoc Team

## Support

For issues or questions, please open an issue in the repository.
