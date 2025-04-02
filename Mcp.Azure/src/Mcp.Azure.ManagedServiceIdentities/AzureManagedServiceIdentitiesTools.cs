using Azure.Identity;
using Azure.ResourceManager.ManagedServiceIdentities;
using Azure.ResourceManager;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Azure.Core;

namespace Mcp.Azure.ManagedServiceIdentities;

[McpServerToolType]
public static class AzureManagedServiceIdentitiesTools
{
    [McpServerTool, Description("Gets the list of Azure Managed Identities in the specified subscription.")]
    public static IEnumerable<AzureManagedServiceIdentity> ListManagedIdentities(
        [Description("The tenant ID to use for authentication")] string tenantId,
        [Description("The client ID to use for authentication")] string clientId,
        [Description("The client secret to use for authentication")] string clientSecret,
        [Description("The subscription ID to list managed identities for")] string subscriptionId)
    {
        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Azure credentials not found in configuration. Please check your .env file.");
        }

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var armClient = new ArmClient(credential, subscriptionId);
        
        var identities = new List<AzureManagedServiceIdentity>();
        var resourceIdentifier = new ResourceIdentifier($"/subscriptions/{subscriptionId}");
        var subscription = armClient.GetSubscriptionResource(resourceIdentifier);
        var userAssignedIdentities = subscription.GetUserAssignedIdentities();
        
        foreach (var identity in userAssignedIdentities)
        {
            var resourceId = identity.Data.Id;
            var resourceGroupName = resourceId.Parent?.Parent?.Name ?? string.Empty;
            
            identities.Add(new AzureManagedServiceIdentity
            {
                Name = identity.Data.Name,
                ResourceGroupName = resourceGroupName,
                PrincipalId = identity.Data.PrincipalId?.ToString(),
                ClientId = identity.Data.ClientId?.ToString(),
                TenantId = identity.Data.TenantId?.ToString(),
                Location = identity.Data.Location.Name,
                Tags = identity.Data.Tags
            });
        }

        return identities;
    }
}
