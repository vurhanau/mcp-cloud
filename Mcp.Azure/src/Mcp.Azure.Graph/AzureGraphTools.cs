using Azure.Identity;
using Microsoft.Graph;
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Mcp.Azure.Graph;

[McpServerToolType]
public static class AzureGraphTools
{
    [McpServerTool, Description("Gets the list of Azure Service Principals in the specified tenant.")]
    public static async Task<IReadOnlyList<ServicePrincipal>> ListServicePrincipals(
        [Description("The tenant ID to use for authentication")] string tenantId,
        [Description("The client ID to use for authentication")] string clientId,
        [Description("The client secret to use for authentication")] string clientSecret)
    {
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var graphClient = new GraphServiceClient(credential);

        var servicePrincipals = await graphClient.ServicePrincipals
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select = new[] { "id", "displayName", "appId", "appOwnerOrganizationId", "description" };
            });

        if (servicePrincipals?.Value == null)
        {
            return Array.Empty<ServicePrincipal>();
        }

        return servicePrincipals.Value.Select(sp => new ServicePrincipal(
            sp.Id ?? string.Empty,
            sp.DisplayName ?? string.Empty,
            sp.AppId ?? string.Empty,
            sp.AppOwnerOrganizationId ?? Guid.Empty,
            sp.Description ?? string.Empty
        )).ToList();
    }
} 