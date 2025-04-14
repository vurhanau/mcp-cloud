using Xunit;
using FluentAssertions;
using Mcp.Azure.Tests.Common;

namespace Mcp.Azure.Authorization.Tests;

public class AzureAuthorizationToolsTests
{
    private readonly AzureTestConfig _config;

    private readonly AzureResourceManager _arm;

    public AzureAuthorizationToolsTests()
    {
        _config = AzureTestConfigLoader.FromDotEnv();
        _arm = new AzureResourceManager(_config.Credential);
    }

    [Fact]
    public async Task TestRoleAssignmentTools()
    {
        var roleDefinitionId = $"/subscriptions/{_config.SubscriptionId}/providers/Microsoft.Authorization/roleDefinitions/acdd72a7-3385-48ef-bd42-f606fba81ae7";
        var scope = await _arm.CreateTestResourceGroupAsync(_config.SubscriptionId);
        var tenantId = _config.TenantId;
        var clientId = _config.ClientId;
        var clientSecret = _config.ClientSecret;
        try
        {
            var ra = await AzureAuthorizationTools.CreateRoleAssignment(
                tenantId, clientId, clientSecret, scope, _config.PrincipalId, roleDefinitionId);
            ra.Should().NotBeNull();

            var list = await AzureAuthorizationTools.ListRoleAssignments(
                _config.TenantId, _config.ClientId, _config.ClientSecret, scope);
            list.Should().NotBeNull();
            list.Should().ContainSingle(item => item.Id == ra.Id);

            await AzureAuthorizationTools.DeleteRoleAssignment(
                tenantId, clientId, clientSecret, scope, ra.Name);
            var listAfterDelete = await AzureAuthorizationTools.ListRoleAssignments(
                tenantId, clientId, clientSecret, scope);
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
        var scope = await _arm.CreateTestResourceGroupAsync(_config.SubscriptionId);
        var tenantId = _config.TenantId;
        var clientId = _config.ClientId;
        var clientSecret = _config.ClientSecret;
        try
        {
            var name = ResourceNames.GenerateRoleDefinitionName();
            var rd = await AzureAuthorizationTools.CreateRoleDefinition(
                tenantId, clientId, clientSecret, scope, name, "Test role description", ["Microsoft.Compute/virtualMachines/read"]);
            rd.Should().NotBeNull();

            var list = await AzureAuthorizationTools.ListRoleDefinitions(
                tenantId, clientId, clientSecret, scope);
            list.Should().NotBeNull();
            list.Should().ContainSingle(item => item.RoleName == rd.RoleName);

            await AzureAuthorizationTools.DeleteRoleDefinition(
                tenantId, clientId, clientSecret, scope, rd.Name);
            var listAfterDelete = await AzureAuthorizationTools.ListRoleDefinitions(
                tenantId, clientId, clientSecret, scope);
            listAfterDelete.Should().NotBeNull();
            listAfterDelete.Should().NotContain(item => item.RoleName == rd.RoleName);
        }
        finally
        {
            await _arm.DeleteTestResourceGroupAsync(scope);
        }
    }
}