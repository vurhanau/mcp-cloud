namespace Mcp.Azure.Authorization;

public class RoleAssignment
{
    public string Name { get; set; } = string.Empty;
    public string PrincipalId { get; set; } = string.Empty;
    public string PrincipalType { get; set; } = string.Empty;
    public string RoleDefinitionId { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}
