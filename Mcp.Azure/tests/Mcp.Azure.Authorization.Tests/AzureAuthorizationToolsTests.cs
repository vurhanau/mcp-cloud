using Xunit;
using FluentAssertions;
using dotenv.net;
using Azure.Identity;
using Mcp.Azure.Tests.Common;
using Azure.Core;

namespace Mcp.Azure.Authorization.Tests;

public class AzureAuthorizationToolsTests
{
    private readonly string _tenantId;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _subscriptionId;
    private readonly string _principalId;
    private readonly AzureResourceManager _arm;

    public AzureAuthorizationToolsTests()
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var envFile = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT_FILE") ?? Path.Combine(homeDirectory, ".env");
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: [envFile]));
        
        _tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID") 
            ?? throw new InvalidOperationException("AZURE_TENANT_ID environment variable is not set");
        _clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID") 
            ?? throw new InvalidOperationException("AZURE_CLIENT_ID environment variable is not set");
        _clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET") 
            ?? throw new InvalidOperationException("AZURE_CLIENT_SECRET environment variable is not set");
        _subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID") 
            ?? throw new InvalidOperationException("AZURE_SUBSCRIPTION_ID environment variable is not set");
        _principalId = Environment.GetEnvironmentVariable("TEST_PRINCIPAL_ID")
            ?? throw new InvalidOperationException("TEST_PRINCIPAL_ID environment variable is not set");

        var credential = new ClientSecretCredential(_tenantId, _clientId, _clientSecret);
        _arm = new AzureResourceManager(credential);
    }

    [Fact]
    public async Task TestRoleAssignmentTools()
    {
        var roleDefinitionId = $"/subscriptions/{_subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/acdd72a7-3385-48ef-bd42-f606fba81ae7";
        var scope = await _arm.CreateTestResourceGroupAsync(_subscriptionId);
        try
        {
            var ra = await AzureAuthorizationTools.CreateRoleAssignment(_tenantId, _clientId, _clientSecret, scope, _principalId, roleDefinitionId);
            ra.Should().NotBeNull();

            var list = await AzureAuthorizationTools.ListRoleAssignments(_tenantId, _clientId, _clientSecret, scope);
            list.Should().NotBeNull();
            list.Should().ContainSingle(item => item.Id == ra.Id);

            await AzureAuthorizationTools.DeleteRoleAssignment(_tenantId, _clientId, _clientSecret, scope, ra.Name);
            var listAfterDelete = await AzureAuthorizationTools.ListRoleAssignments(_tenantId, _clientId, _clientSecret, scope);
            listAfterDelete.Should().NotBeNull();
            listAfterDelete.Should().NotContain(item => item.Id == ra.Id);
        }
        finally
        {
            await _arm.DeleteTestResourceGroupAsync(scope);
        }
    }

    [Fact]
    public async Task TestRoleDefinitionTools()
    {
        var scope = await _arm.CreateTestResourceGroupAsync(_subscriptionId);
        try
        {
            var name = ResourceNames.GenerateRoleDefinitionName();
            var rd = await AzureAuthorizationTools.CreateRoleDefinition(
                _tenantId, _clientId, _clientSecret, scope, name, "Test role description", ["Microsoft.Compute/virtualMachines/read"]);
            rd.Should().NotBeNull();

            var list = await AzureAuthorizationTools.ListRoleDefinitions(_tenantId, _clientId, _clientSecret, scope);
            list.Should().NotBeNull();
            list.Should().ContainSingle(item => item.RoleName == rd.RoleName);

            await AzureAuthorizationTools.DeleteRoleDefinition(_tenantId, _clientId, _clientSecret, scope, rd.Name);
            var listAfterDelete = await AzureAuthorizationTools.ListRoleDefinitions(_tenantId, _clientId, _clientSecret, scope);
            listAfterDelete.Should().NotBeNull();
            listAfterDelete.Should().NotContain(item => item.RoleName == rd.RoleName);
        }
        finally
        {
            await _arm.DeleteTestResourceGroupAsync(scope);
        }
    }
}