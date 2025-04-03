using Xunit;
using Moq;
using FluentAssertions;
using Mcp.Azure.Authorization;
using Azure.ResourceManager.Authorization;
using Azure.ResourceManager;
using Azure.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotenv.net;

namespace Mcp.Azure.Authorization.Tests;

public class AzureAuthorizationToolsTests
{
    private readonly string _tenantId;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _subscriptionId;

    public AzureAuthorizationToolsTests()
    {
        // Load environment variables from .env file
        var envFile = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT_FILE") ?? ".env";
        Console.WriteLine($"Loading environment variables from: {envFile}");
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { envFile }));
        
        _tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID") 
            ?? throw new InvalidOperationException("AZURE_TENANT_ID environment variable is not set");
        _clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID") 
            ?? throw new InvalidOperationException("AZURE_CLIENT_ID environment variable is not set");
        _clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET") 
            ?? throw new InvalidOperationException("AZURE_CLIENT_SECRET environment variable is not set");
        _subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID") 
            ?? throw new InvalidOperationException("AZURE_SUBSCRIPTION_ID environment variable is not set");
    }

    [Fact]
    public async Task ListRoleAssignments_ShouldReturnRoleAssignments()
    {
        // Arrange
        var scope = $"/subscriptions/{_subscriptionId}";

        // Act
        var result = await AzureAuthorizationTools.ListRoleAssignments(_tenantId, _clientId, _clientSecret, scope);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateRoleAssignment_ShouldCreateNewRoleAssignment()
    {
        // Arrange
        var scope = $"/subscriptions/{_subscriptionId}";
        var principalId = Environment.GetEnvironmentVariable("TEST_PRINCIPAL_ID") 
            ?? throw new InvalidOperationException("TEST_PRINCIPAL_ID environment variable is not set");
        var roleDefinitionId = Environment.GetEnvironmentVariable("TEST_ROLE_DEFINITION_ID") 
            ?? throw new InvalidOperationException("TEST_ROLE_DEFINITION_ID environment variable is not set");

        // Act
        var result = await AzureAuthorizationTools.CreateRoleAssignment(
            _tenantId, _clientId, _clientSecret, scope, principalId, roleDefinitionId);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteRoleAssignment_ShouldDeleteExistingRoleAssignment()
    {
        // Arrange
        var scope = $"/subscriptions/{_subscriptionId}";
        var roleAssignmentName = Environment.GetEnvironmentVariable("TEST_ROLE_ASSIGNMENT_NAME") 
            ?? throw new InvalidOperationException("TEST_ROLE_ASSIGNMENT_NAME environment variable is not set");

        // Act
        await AzureAuthorizationTools.DeleteRoleAssignment(
            _tenantId, _clientId, _clientSecret, scope, roleAssignmentName);

        // Assert
        // No exception means success
    }
} 