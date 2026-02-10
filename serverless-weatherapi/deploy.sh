#!/bin/bash

# Deployment script for Weather API

set -e

echo "========================================="
echo "Weather API Deployment Script"
echo "========================================="

# Check prerequisites
command -v dotnet >/dev/null 2>&1 || { echo "❌ .NET SDK is required but not installed."; exit 1; }
command -v node >/dev/null 2>&1 || { echo "❌ Node.js is required but not installed."; exit 1; }
command -v cdk >/dev/null 2>&1 || { echo "❌ AWS CDK is required but not installed. Run: npm install -g aws-cdk"; exit 1; }

# Check for OpenWeather API key
if [ -z "$OPENWEATHER_API_KEY" ]; then
    echo "❌ OPENWEATHER_API_KEY environment variable is not set"
    echo "   Get your free API key from: https://openweathermap.org/api"
    echo "   Then export it: export OPENWEATHER_API_KEY='your_key_here'"
    exit 1
fi

echo "✅ Prerequisites check passed"

# Build .NET solution
echo ""
echo "📦 Building .NET solution..."
dotnet restore
dotnet build -c Release

# Run tests
echo ""
echo "🧪 Running tests..."
dotnet test --no-build -c Release

# Publish Lambda function
echo ""
echo "📦 Publishing Lambda function..."
cd src/WeatherApi.Lambda
dotnet publish -c Release -o bin/Release/net10.0/publish
cd ../..

# Install CDK dependencies
echo ""
echo "📦 Installing CDK dependencies..."
cd infrastructure
npm install

# Deploy with CDK
echo ""
echo "🚀 Deploying to AWS..."
cdk deploy --require-approval never

echo ""
echo "========================================="
echo "✅ Deployment completed successfully!"
echo "========================================="
echo ""
echo "Check the outputs above for:"
echo "  - ApiUrl: Your API endpoint"
echo "  - SwaggerUrl: Swagger UI documentation"
echo ""
