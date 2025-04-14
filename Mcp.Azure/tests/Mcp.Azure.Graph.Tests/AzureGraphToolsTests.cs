using Mcp.Azure.Tests.Common;
using FluentAssertions;

namespace Mcp.Azure.Graph.Tests;

public class AzureGraphToolsTests
{
    private readonly AzureTestConfig _config;

    public AzureGraphToolsTests()
    {
        _config = AzureTestConfigLoader.FromDotEnv();
    }

    [Fact]
    public async Task TestGraphTools()
    {
        var tenantId = _config.TenantId;
        var clientId = _config.ClientId;
        var clientSecret = _config.ClientSecret;
        var servicePrincipals = await AzureGraphTools.ListServicePrincipals(tenantId, clientId, clientSecret);
        servicePrincipals.Should().NotBeNull();
        servicePrincipals.Should().NotBeEmpty();
        servicePrincipals.Should().ContainSingle(sp => sp.AppId == _config.ClientId);
    }
}
