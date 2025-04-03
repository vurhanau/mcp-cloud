using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Mcp.Azure.ResourceManager;

[McpServerToolType]
public static class ResourceManagerTools
{
    [McpServerTool, Description("Gets the list of Azure Resource Groups in the specified subscription.")]
    public static async Task<IEnumerable<ResourceGroup>> ListResourceGroups(
        string? subscriptionId = null,
        string? tenantId = null,
        string? clientId = null,
        string? clientSecret = null
    )
    {
        var credential = new ClientSecretCredential(
            tenantId ?? throw new ArgumentNullException(nameof(tenantId)),
            clientId ?? throw new ArgumentNullException(nameof(clientId)),
            clientSecret ?? throw new ArgumentNullException(nameof(clientSecret))
        );

        var armClient = new ArmClient(credential);
        var subscription = await armClient.GetDefaultSubscriptionAsync();
        var resourceGroups = subscription.GetResourceGroups();

        var result = new List<ResourceGroup>();
        await foreach (var group in resourceGroups.GetAllAsync())
        {
            result.Add(new ResourceGroup(
                group.Data.Name,
                group.Data.Location,
                group.Data.ManagedBy,
                group.Data.Tags?.ToDictionary(t => t.Key, t => t.Value)
            ));
        }

        return result;
    }

    [McpServerTool, Description("Creates a new Azure Resource Group.")]
    public static async Task<ResourceGroup> CreateResourceGroup(
        string name,
        string location,
        string? subscriptionId = null,
        string? tenantId = null,
        string? clientId = null,
        string? clientSecret = null,
        Dictionary<string, string>? tags = null
    )
    {
        var credential = new ClientSecretCredential(
            tenantId ?? throw new ArgumentNullException(nameof(tenantId)),
            clientId ?? throw new ArgumentNullException(nameof(clientId)),
            clientSecret ?? throw new ArgumentNullException(nameof(clientSecret))
        );

        var armClient = new ArmClient(credential);
        var subscription = await armClient.GetDefaultSubscriptionAsync();
        var resourceGroups = subscription.GetResourceGroups();

        var parameters = new ResourceGroupData(location);
        if (tags != null)
        {
            foreach (var tag in tags)
            {
                parameters.Tags[tag.Key] = tag.Value;
            }
        }

        var result = await resourceGroups.CreateOrUpdateAsync(
            WaitUntil.Completed,
            name,
            parameters
        );

        return new ResourceGroup(
            result.Value.Data.Name,
            result.Value.Data.Location,
            result.Value.Data.ManagedBy,
            result.Value.Data.Tags?.ToDictionary(t => t.Key, t => t.Value)
        );
    }

    [McpServerTool, Description("Deletes an Azure Resource Group.")]
    public static async Task DeleteResourceGroup(
        string name,
        string? subscriptionId = null,
        string? tenantId = null,
        string? clientId = null,
        string? clientSecret = null
    )
    {
        var credential = new ClientSecretCredential(
            tenantId ?? throw new ArgumentNullException(nameof(tenantId)),
            clientId ?? throw new ArgumentNullException(nameof(clientId)),
            clientSecret ?? throw new ArgumentNullException(nameof(clientSecret))
        );

        var armClient = new ArmClient(credential);
        var subscription = await armClient.GetDefaultSubscriptionAsync();
        var resourceGroups = subscription.GetResourceGroups();
        var resourceGroup = await resourceGroups.GetAsync(name);

        await resourceGroup.Value.DeleteAsync(WaitUntil.Completed);
    }
}