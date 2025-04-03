using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Authorization;
using Azure.ResourceManager.Authorization.Models;
using Azure.ResourceManager;

namespace Mcp.Azure.Authorization;

internal static class RoleAssignmentOperations
{
    public static async Task<IEnumerable<RoleAssignment>> ListRoleAssignments(
        string tenantId,
        string clientId,
        string clientSecret,
        string scope)
    {
        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Azure credentials not found in configuration. Please check your .env file.");
        }

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var armClient = new ArmClient(credential);
        
        var assignments = new List<RoleAssignment>();
        var resourceId = new ResourceIdentifier(scope);
        var roleAssignments = armClient.GetRoleAssignments(resourceId);
        
        await foreach (var assignment in roleAssignments.GetAllAsync())
        {
            assignments.Add(new RoleAssignment
            {
                Name = assignment.Data.Name ?? string.Empty,
                PrincipalId = assignment.Data.PrincipalId?.ToString() ?? string.Empty,
                PrincipalType = assignment.Data.PrincipalType?.ToString() ?? string.Empty,
                RoleDefinitionId = assignment.Data.RoleDefinitionId?.ToString() ?? string.Empty,
                Scope = assignment.Data.Scope?.ToString() ?? string.Empty
            });
        }

        return assignments;
    }

    public static async Task<RoleAssignment> CreateRoleAssignment(
        string tenantId,
        string clientId,
        string clientSecret,
        string scope,
        string principalId,
        string roleDefinitionId)
    {
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var armClient = new ArmClient(credential);
        
        var resourceId = new ResourceIdentifier(scope);
        var roleAssignments = armClient.GetRoleAssignments(resourceId);
        
        var parameters = new RoleAssignmentCreateOrUpdateContent(
            new ResourceIdentifier(roleDefinitionId),
            new Guid(principalId)
        );

        var result = await roleAssignments.CreateOrUpdateAsync(
            WaitUntil.Completed,
            Guid.NewGuid().ToString(),
            parameters
        );

        return new RoleAssignment
        {
            Name = result.Value.Data.Name ?? string.Empty,
            PrincipalId = result.Value.Data.PrincipalId?.ToString() ?? string.Empty,
            PrincipalType = result.Value.Data.PrincipalType?.ToString() ?? string.Empty,
            RoleDefinitionId = result.Value.Data.RoleDefinitionId?.ToString() ?? string.Empty,
            Scope = result.Value.Data.Scope?.ToString() ?? string.Empty
        };
    }

    public static async Task DeleteRoleAssignment(
        string tenantId,
        string clientId,
        string clientSecret,
        string scope,
        string roleAssignmentName)
    {
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var armClient = new ArmClient(credential);
        
        var resourceId = new ResourceIdentifier(scope);
        var roleAssignments = armClient.GetRoleAssignments(resourceId);
        var roleAssignment = await roleAssignments.GetAsync(roleAssignmentName);

        await roleAssignment.Value.DeleteAsync(WaitUntil.Completed);
    }
} 