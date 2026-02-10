#!/bin/bash

# Script to run tests with coverage and generate HTML report

echo "Running tests with coverage..."

# Clean previous results
rm -rf tests/WeatherApi.Tests/TestResults/
rm -rf coverage/

# Run tests with coverage
dotnet test tests/WeatherApi.Tests/WeatherApi.Tests.csproj \
  --collect:"XPlat Code Coverage" \
  --settings coverlet.runsettings \
  --results-directory tests/WeatherApi.Tests/TestResults/

# Find the coverage file
COVERAGE_FILE=$(find tests/WeatherApi.Tests/TestResults -name "coverage.cobertura.xml" | head -n 1)

if [ -z "$COVERAGE_FILE" ]; then
  echo "Error: Coverage file not found"
  exit 1
fi

echo "Coverage file: $COVERAGE_FILE"

# Generate HTML report
echo "Generating HTML coverage report..."
~/.dotnet/tools/reportgenerator \
  -reports:"$COVERAGE_FILE" \
  -targetdir:"coverage" \
  -reporttypes:"Html;TextSummary"

# Display summary
echo ""
echo "Coverage Summary:"
cat coverage/Summary.txt

echo ""
echo "HTML report generated at: coverage/index.html"
echo "Open it with: xdg-open coverage/index.html"
