using FluentAssertions;
using Mcp.Azure.Tests.Common;
using Xunit;

namespace Mcp.Azure.ManagedServiceIdentities.Tests;

public class AzureManagedServiceIdentitiesToolsTests
{
    private readonly AzureTestConfig _config;

    private readonly AzureResourceManager _arm;

    public AzureManagedServiceIdentitiesToolsTests()
    {
        _config = AzureTestConfigLoader.FromDotEnv();
        _arm = new AzureResourceManager(_config.Credential);
    }

    [Fact]
    public async Task TestManagedServiceIdentityTools()
    {
        var subscriptionId = _config.SubscriptionId;
        var tenantId = _config.TenantId;
        var clientId = _config.ClientId;
        var clientSecret = _config.ClientSecret;

        var resourceGroupId = await _arm.CreateTestResourceGroupAsync(subscriptionId);
        var resourceGroupName = resourceGroupId.Split('/').Last();
        try
        {
            var name = ResourceNames.GenerateManagedServiceIdentityName();
            var msi = await AzureManagedServiceIdentitiesTools.CreateManagedServiceIdentity(
                tenantId, clientId, clientSecret, subscriptionId, resourceGroupName, name);
            msi.Should().NotBeNull();

            var list = await AzureManagedServiceIdentitiesTools.ListManagedServiceIdentities(
                tenantId, clientId, clientSecret, subscriptionId);
            list.Should().NotBeNull();
            list.Should().ContainSingle(item => item.PrincipalId == msi.PrincipalId);
        }
        finally
        {
            await _arm.DeleteTestResourceGroupAsync(resourceGroupId);
        }
    }
}
