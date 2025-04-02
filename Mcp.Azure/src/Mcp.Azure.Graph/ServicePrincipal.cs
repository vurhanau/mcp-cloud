namespace Mcp.Azure.Graph;

public record ServicePrincipal(
    string Id,
    string DisplayName,
    string AppId,
    Guid AppOwnerOrganizationId,
    string? Description); 