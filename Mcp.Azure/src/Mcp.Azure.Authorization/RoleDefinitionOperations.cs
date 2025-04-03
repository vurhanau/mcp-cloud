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
                definition.Data.Name ?? string.Empty,
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
        
        var parameters = new AuthorizationRoleDefinitionData
        {
            RoleName = name,
            Description = description,
            RoleType = AuthorizationRoleType.CustomRole
        };

        var permission = new RoleDefinitionPermission();
        foreach (var action in actions)
        {
            permission.Actions.Add(action);
        }

        parameters.Permissions.Add(permission);

        var result = await roleDefinitions.CreateOrUpdateAsync(
            WaitUntil.Completed,
            new ResourceIdentifier($"{scope}/providers/Microsoft.Authorization/roleDefinitions/{Guid.NewGuid()}"),
            parameters
        );

        return new RoleDefinition(
            result.Value.Data.Name ?? string.Empty,
            result.Value.Data.Description ?? string.Empty,
            result.Value.Data.RoleType.ToString() ?? string.Empty,
            result.Value.Data.Permissions?.SelectMany(p => p.Actions ?? new List<string>()).ToList() ?? new List<string>()
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
        
        var resourceId = new ResourceIdentifier(scope);
        var roleDefinitions = armClient.GetAuthorizationRoleDefinitions(resourceId);
        var roleDefinitionId = new ResourceIdentifier($"{scope}/providers/Microsoft.Authorization/roleDefinitions/{roleDefinitionName}");
        var roleDefinition = await roleDefinitions.GetAsync(roleDefinitionId);

        await roleDefinition.Value.DeleteAsync(WaitUntil.Completed);
    }
} 