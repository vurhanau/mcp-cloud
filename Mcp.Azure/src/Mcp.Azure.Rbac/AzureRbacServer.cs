using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Mcp.Azure.Rbac;

public static class AzureRbacServer
{
    public static HostApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Logging.AddConsole();
        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();
        return builder;
    }
}

[McpServerToolType]
public static class AzureRbacTools
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
        
        var token = await credential.GetTokenAsync(
            new TokenRequestContext(["https://management.azure.com/.default"]), 
            CancellationToken.None);
        var tokenCredential = new TokenCredentials(token.Token);
        
        var client = new AuthorizationManagementClient(tokenCredential) { SubscriptionId = scope };

        var assignments = new List<RoleAssignment>();
        var result = await client.RoleAssignments.ListForScopeAsync(scope);
        
        foreach (var assignment in result)
        {
            assignments.Add(new RoleAssignment
            {
                Name = assignment.Name,
                PrincipalId = assignment.PrincipalId,
                PrincipalType = assignment.PrincipalType,
                RoleDefinitionId = assignment.RoleDefinitionId,
                Scope = assignment.Scope
            });
        }

        return assignments;
    }
}

public class RoleAssignment
{
    public string Name { get; set; } = string.Empty;
    public string PrincipalId { get; set; } = string.Empty;
    public string PrincipalType { get; set; } = string.Empty;
    public string RoleDefinitionId { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}
