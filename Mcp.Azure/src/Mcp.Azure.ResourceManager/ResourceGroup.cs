namespace Mcp.Azure.ResourceManager;

public record ResourceGroup(
    string Name,
    string Location,
    string? ManagedBy = null,
    Dictionary<string, string>? Tags = null
); 