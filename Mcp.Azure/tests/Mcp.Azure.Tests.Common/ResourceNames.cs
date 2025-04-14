namespace Mcp.Azure.Tests.Common;

public class ResourceNames
{
    public static string GenerateResourceGroupName() => "azuremcp-rg" + Suffix();

    public static string GenerateRoleDefinitionName() => "azuremcp-rd" + Suffix();

    public static string GenerateManagedServiceIdentityName() => "azuremcp-msi" + Suffix();

    private static string Suffix()
    {
        var dateString = DateTime.UtcNow.ToString("yyyyMMdd");
        var id = Guid.NewGuid().ToString("N")[..8];

        return $"-{dateString}-{id}";
    }
}
