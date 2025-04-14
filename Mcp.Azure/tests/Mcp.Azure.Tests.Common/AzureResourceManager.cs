using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Mcp.Azure.Tests.Common;

public class AzureResourceManager
{
    private readonly ArmClient _armClient;

    public AzureResourceManager(TokenCredential credential)
    {
        _armClient = new ArmClient(credential);
    }

    /// <summary>
    /// Creates a new resource group in the specified subscription.
    /// </summary>
    /// <returns>
    /// Fully qualified resource ID for the resource. <br/>
    /// Example: /subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/{resourceProviderNamespace}/{resourceType}/{resourceName}.
    /// </returns>
    public async Task<string> CreateTestResourceGroupAsync(string subscriptionId)
    {
        var subscription = _armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{subscriptionId}"));

        var resourceGroupData = new ResourceGroupData(AzureLocation.WestUS);
        var resourceGroupName = ResourceNames.GenerateResourceGroupName();
        var resourceGroup = await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, resourceGroupName, resourceGroupData);

        return resourceGroup.Value.Data.Id.ToString();
    }

    public async Task DeleteTestResourceGroupAsync(string resourceGroupId)
    {
        var resourceGroup = new ResourceIdentifier(resourceGroupId);
        var resourceGroupResource = _armClient.GetResourceGroupResource(resourceGroup);
        await resourceGroupResource.DeleteAsync(WaitUntil.Completed);
    }
}
