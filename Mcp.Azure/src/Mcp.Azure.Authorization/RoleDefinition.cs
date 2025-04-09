namespace Mcp.Azure.Authorization;

public record RoleDefinition(
    string Id,
    string Name,
    string RoleName,
    string Description,
    string RoleType,
    List<string> Permissions
); 