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
        return await RoleAssignmentOperations.ListRoleAssignments(tenantId, clientId, clientSecret, scope);
    }

    [McpServerTool, Description("Creates a new Azure RBAC role assignment.")]
    public static async Task<RoleAssignment> CreateRoleAssignment(
        [Description("The tenant ID to use for authentication")] string tenantId,
        [Description("The client ID to use for authentication")] string clientId,
        [Description("The client secret to use for authentication")] string clientSecret,
        [Description("The scope to create the role assignment for (e.g., subscription ID, resource group name, or resource ID)")] string scope,
        [Description("The principal ID to assign the role to")] string principalId,
        [Description("The role definition ID to assign")] string roleDefinitionId)
    {
        return await RoleAssignmentOperations.CreateRoleAssignment(tenantId, clientId, clientSecret, scope, principalId, roleDefinitionId);
    }

    [McpServerTool, Description("Deletes an Azure RBAC role assignment.")]
    public static async Task DeleteRoleAssignment(
        [Description("The tenant ID to use for authentication")] string tenantId,
        [Description("The client ID to use for authentication")] string clientId,
        [Description("The client secret to use for authentication")] string clientSecret,
        [Description("The scope of the role assignment (e.g., subscription ID, resource group name, or resource ID)")] string scope,
        [Description("The name of the role assignment to delete")] string roleAssignmentName)
    {
        await RoleAssignmentOperations.DeleteRoleAssignment(tenantId, clientId, clientSecret, scope, roleAssignmentName);
    }

    [McpServerTool, Description("Gets the list of Azure RBAC role definitions.")]
    public static async Task<IEnumerable<RoleDefinition>> ListRoleDefinitions(
        [Description("The tenant ID to use for authentication")] string tenantId,
        [Description("The client ID to use for authentication")] string clientId,
        [Description("The client secret to use for authentication")] string clientSecret,
        [Description("The scope to list role definitions for (e.g., subscription ID, resource group name, or resource ID)")] string scope)
    {
        return await RoleDefinitionOperations.ListRoleDefinitions(tenantId, clientId, clientSecret, scope);
    }

    [McpServerTool, Description("Creates a new Azure RBAC role definition.")]
    public static async Task<RoleDefinition> CreateRoleDefinition(
        [Description("The tenant ID to use for authentication")] string tenantId,
        [Description("The client ID to use for authentication")] string clientId,
        [Description("The client secret to use for authentication")] string clientSecret,
        [Description("The scope to create the role definition for (e.g., subscription ID, resource group name, or resource ID)")] string scope,
        [Description("The name of the role definition")] string name,
        [Description("The description of the role definition")] string description,
        [Description("The list of actions allowed by this role")] List<string> actions)
    {
        return await RoleDefinitionOperations.CreateRoleDefinition(tenantId, clientId, clientSecret, scope, name, description, actions);
    }

    [McpServerTool, Description("Deletes an Azure RBAC role definition.")]
    public static async Task DeleteRoleDefinition(
        [Description("The tenant ID to use for authentication")] string tenantId,
        [Description("The client ID to use for authentication")] string clientId,
        [Description("The client secret to use for authentication")] string clientSecret,
        [Description("The scope of the role definition (e.g., subscription ID, resource group name, or resource ID)")] string scope,
        [Description("The name of the role definition to delete")] string roleDefinitionName)
    {
        await RoleDefinitionOperations.DeleteRoleDefinition(tenantId, clientId, clientSecret, scope, roleDefinitionName);
    }
}
