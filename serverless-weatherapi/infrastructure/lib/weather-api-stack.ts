import * as cdk from 'aws-cdk-lib';
import * as lambda from 'aws-cdk-lib/aws-lambda';
import * as apigateway from 'aws-cdk-lib/aws-apigatewayv2';
import * as dynamodb from 'aws-cdk-lib/aws-dynamodb';
import * as iam from 'aws-cdk-lib/aws-iam';
import { HttpLambdaIntegration } from 'aws-cdk-lib/aws-apigatewayv2-integrations';
import { Construct } from 'constructs';

export class WeatherApiStack extends cdk.Stack {
    constructor(scope: Construct, id: string, props?: cdk.StackProps) {
        super(scope, id, props);

        // DynamoDB Table
        const weatherTable = new dynamodb.Table(this, 'WeatherDataTable', {
            tableName: 'WeatherData',
            partitionKey: {
                name: 'Id',
                type: dynamodb.AttributeType.STRING
            },
            billingMode: dynamodb.BillingMode.PAY_PER_REQUEST,
            removalPolicy: cdk.RemovalPolicy.DESTROY, // For development only
            pointInTimeRecoverySpecification: {
                pointInTimeRecoveryEnabled: true
            },
        });

        // Add GSI for querying by city
        weatherTable.addGlobalSecondaryIndex({
            indexName: 'CityIndex',
            partitionKey: {
                name: 'City',
                type: dynamodb.AttributeType.STRING
            },
            projectionType: dynamodb.ProjectionType.ALL,
        });

        // Lambda Function
        const weatherLambda = new lambda.Function(this, 'WeatherApiFunction', {
            runtime: lambda.Runtime.DOTNET_10,
            handler: 'WeatherApi.Lambda',
            code: lambda.Code.fromAsset('../src/WeatherApi.Lambda/bin/Release/net10.0/publish'),
            memorySize: 512,
            timeout: cdk.Duration.seconds(30),
            environment: {
                TABLE_NAME: weatherTable.tableName,
                OPENWEATHER_API_KEY: process.env.OPENWEATHER_API_KEY || '',
                ASPNETCORE_ENVIRONMENT: 'Production'
            },
            tracing: lambda.Tracing.ACTIVE,
        });

        // Grant DynamoDB permissions to Lambda
        weatherTable.grantReadWriteData(weatherLambda);

        // API Gateway HTTP API
        const httpApi = new apigateway.HttpApi(this, 'WeatherHttpApi', {
            apiName: 'WeatherApi',
            description: 'Serverless Weather API',
            corsPreflight: {
                allowOrigins: ['*'],
                allowMethods: [apigateway.CorsHttpMethod.GET, apigateway.CorsHttpMethod.POST,
                apigateway.CorsHttpMethod.DELETE, apigateway.CorsHttpMethod.OPTIONS],
                allowHeaders: ['*'],
            },
        });

        // Lambda Integration
        const lambdaIntegration = new HttpLambdaIntegration('LambdaIntegration', weatherLambda);

        // Add routes
        httpApi.addRoutes({
            path: '/{proxy+}',
            methods: [apigateway.HttpMethod.ANY],
            integration: lambdaIntegration,
        });

        // Outputs
        new cdk.CfnOutput(this, 'ApiUrl', {
            value: httpApi.url!,
            description: 'Weather API URL',
            exportName: 'WeatherApiUrl',
        });

        new cdk.CfnOutput(this, 'SwaggerUrl', {
            value: `${httpApi.url}swagger`,
            description: 'Swagger UI URL',
            exportName: 'SwaggerUrl',
        });

        new cdk.CfnOutput(this, 'TableName', {
            value: weatherTable.tableName,
            description: 'DynamoDB Table Name',
            exportName: 'WeatherTableName',
        });
    }
}
