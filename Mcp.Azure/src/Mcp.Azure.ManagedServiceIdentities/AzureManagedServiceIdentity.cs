namespace Mcp.Azure.ManagedServiceIdentities;

public class AzureManagedServiceIdentity
{
    public string Name { get; set; } = string.Empty;
    public string ResourceGroupName { get; set; } = string.Empty;
    public string? PrincipalId { get; set; }
    public string? ClientId { get; set; }
    public string? TenantId { get; set; }
    public string Location { get; set; } = string.Empty;
    public IDictionary<string, string>? Tags { get; set; }
} 