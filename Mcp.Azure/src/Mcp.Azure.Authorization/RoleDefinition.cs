namespace Mcp.Azure.Authorization;

public record RoleDefinition(
    string Name,
    string Description,
    string RoleType,
    List<string> Permissions
); 