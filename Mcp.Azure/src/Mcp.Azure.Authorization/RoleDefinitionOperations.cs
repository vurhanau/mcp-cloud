using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Authorization;
using Azure.ResourceManager.Authorization.Models;
using Azure.ResourceManager;

namespace Mcp.Azure.Authorization;

internal static class RoleDefinitionOperations
{
    public static async Task<IEnumerable<RoleDefinition>> ListRoleDefinitions(
        string tenantId,
        string clientId,
        string clientSecret,
        string scope)
    {
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var armClient = new ArmClient(credential);
        
        var definitions = new List<RoleDefinition>();
        var resourceId = new ResourceIdentifier(scope);
        var roleDefinitions = armClient.GetAuthorizationRoleDefinitions(resourceId);
        
        await foreach (var definition in roleDefinitions.GetAllAsync())
        {
            definitions.Add(new RoleDefinition(
                definition.Data.Id.ToString(),
                definition.Data.Name,
                definition.Data.RoleName ?? string.Empty,
                definition.Data.Description ?? string.Empty,
                definition.Data.RoleType.ToString() ?? string.Empty,
                definition.Data.Permissions?.SelectMany(p => p.Actions ?? new List<string>()).ToList() ?? new List<string>()
            ));
        }

        return definitions;
    }

    public static async Task<RoleDefinition> CreateRoleDefinition(
        string tenantId,
        string clientId,
        string clientSecret,
        string scope,
        string name,
        string description,
        List<string> actions)
    {
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var armClient = new ArmClient(credential);
        
        var resourceId = new ResourceIdentifier(scope);
        var roleDefinitions = armClient.GetAuthorizationRoleDefinitions(resourceId);
        
        // https://github.com/Azure/azure-sdk-for-net/blob/eb8e9a2dcbc7953f558bb08552a904d2ef85458f/sdk/authorization/Azure.ResourceManager.Authorization/tests/Scenario/RoleDefinitionCollectionTests.cs
        var data = new AuthorizationRoleDefinitionData()
        {
            RoleName = name,
            Description = description,
            RoleType = AuthorizationRoleType.CustomRole,
            AssignableScopes = { scope }
        };
        var permission = new RoleDefinitionPermission();
        foreach (var action in actions)
        {
            permission.Actions.Add(action);
        }
        data.Permissions.Add(permission);

        var id = Guid.NewGuid().ToString();
        var result = await roleDefinitions.CreateOrUpdateAsync(WaitUntil.Completed, new ResourceIdentifier(id), data);

        return new RoleDefinition(
            result.Value.Data.Id.ToString(),
            result.Value.Data.Name,
            result.Value.Data.RoleName,
            result.Value.Data.Description,
            result.Value.Data.RoleType.ToString() ?? string.Empty,
            result.Value.Data.Permissions?.SelectMany(p => p.Actions ?? []).ToList() ?? []
        );
    }

    public static async Task DeleteRoleDefinition(
        string tenantId,
        string clientId,
        string clientSecret,
        string scope,
        string roleDefinitionName)
    {
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var armClient = new ArmClient(credential);
        
        var scopeId = new ResourceIdentifier(scope);
        var roleDefinitions = armClient.GetAuthorizationRoleDefinitions(scopeId);
        var resourceId = new ResourceIdentifier(roleDefinitionName);
        var roleDefinition = await roleDefinitions.GetAsync(resourceId);

        await roleDefinition.Value.DeleteAsync(WaitUntil.Completed);
    }
}
