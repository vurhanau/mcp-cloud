using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Authorization;
using Azure.ResourceManager;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Mcp.Azure.Authorization;

[McpServerToolType]
public static class AzureAuthorizationTools
{
    [McpServerTool, Description("Gets the list of Azure RBAC role assignments for the specified scope.")]
    public static async Task<IEnumerable<RoleAssignment>> ListRoleAssignments(
        [Description("The tenant ID to use for authentication")] string tenantId,
        [Description("The client ID to use for authentication")] string clientId,
        [Description("The client secret to use for authentication")] string clientSecret,
        [Description("The scope to list role assignments for (e.g., subscription ID, resource group name, or resource ID)")] string scope)
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
}
