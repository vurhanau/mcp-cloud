using Mcp.Azure.Authorization;
using Mcp.Azure.ManagedServiceIdentities;
using Mcp.Azure.Console;
using Mcp.Azure.Context;
using dotenv.net;
using Mcp.Azure.Graph;
using Mcp.Azure.ResourceManager;

var credentials = LoadAzureCredentials(args);
if (credentials == null)
{
    Console.WriteLine("Failed to load Azure credentials");
    return;
}

try
{
    LoadAzureContext(args);
    await ListServicePrincipals(credentials);
    ListManagedIdentities(credentials);
    await ListRoleAssignments(credentials);
    await ListResourceGroups(credentials);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
    }
}

AzureContext LoadAzureContext(string[] args)
{
    var context = AzureContext.LoadFromEnvironment(args.Length > 0 ? args[0] : null);
    if (context == null)
    {
        throw new InvalidOperationException("Azure context not found in environment variables.");
    }

    return context;
}

AzureCredentials? LoadAzureCredentials(string[] args)
{
    var envPath = args.Length > 0 
        ? args[0] 
        : Path.Combine(Directory.GetCurrentDirectory(), ".env");

    if (!File.Exists(envPath))
    {
        Console.WriteLine($"Error: .env file not found at path: {envPath}");
        return null;
    }

    DotEnv.Load(new DotEnvOptions(envFilePaths: [envPath]));

    var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
    var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
    var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
    var subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

    if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || 
        string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(subscriptionId))
    {
        Console.WriteLine("Error: Azure credentials not found in environment variables.");
        Console.WriteLine("Please make sure AZURE_TENANT_ID, AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, and AZURE_SUBSCRIPTION_ID are set.");
        return null;
    }

    return new AzureCredentials(tenantId, clientId, clientSecret, subscriptionId);
}

async Task ListResourceGroups(AzureCredentials credentials)
{
    Console.WriteLine("\nListing Resource Groups...");
    
    var resourceGroups = await ResourceManagerTools.ListResourceGroups(
        credentials.SubscriptionId,
        credentials.TenantId,
        credentials.ClientId,
        credentials.ClientSecret
    );

    var printer = new TablePrinter<ResourceGroup>("Resource Groups", new[]
    {
        new ColumnDefinition<ResourceGroup>("Name", rg => rg.Name),
        new ColumnDefinition<ResourceGroup>("Location", rg => rg.Location),
        new ColumnDefinition<ResourceGroup>("Managed By", rg => rg.ManagedBy ?? ""),
        new ColumnDefinition<ResourceGroup>("Tags", rg => rg.Tags != null ? string.Join(", ", rg.Tags.Select(t => $"{t.Key}={t.Value}")) : "")
    });

    printer.Print(resourceGroups);
}


async Task ListServicePrincipals(AzureCredentials credentials)
{
    Console.WriteLine("\nListing Service Principals...");
    
    var servicePrincipals = await AzureGraphTools.ListServicePrincipals(
        credentials.TenantId,
        credentials.ClientId, 
        credentials.ClientSecret
    );

    var printer = new TablePrinter<ServicePrincipal>("Service Principals", new[]
    {
        new ColumnDefinition<ServicePrincipal>("Name", sp => sp.DisplayName),
        new ColumnDefinition<ServicePrincipal>("App ID", sp => sp.AppId),
        new ColumnDefinition<ServicePrincipal>("Object ID", sp => sp.Id),
        new ColumnDefinition<ServicePrincipal>("Description", sp => sp.Description)
    });

    printer.Print(servicePrincipals);
}

void ListManagedIdentities(AzureCredentials credentials)
{
    var identities = AzureManagedServiceIdentitiesTools.ListManagedIdentities(
        credentials.TenantId, credentials.ClientId, credentials.ClientSecret, credentials.SubscriptionId);

    var printer = new TablePrinter<AzureManagedServiceIdentity>("Managed Identities",
    [
        new ColumnDefinition<AzureManagedServiceIdentity>("Name", i => i.Name),
        new ColumnDefinition<AzureManagedServiceIdentity>("Resource Group", i => i.ResourceGroupName),
        new ColumnDefinition<AzureManagedServiceIdentity>("Location", i => i.Location),
        new ColumnDefinition<AzureManagedServiceIdentity>("Principal ID", i => i.PrincipalId),
        new ColumnDefinition<AzureManagedServiceIdentity>("Client ID", i => i.ClientId),
        new ColumnDefinition<AzureManagedServiceIdentity>("Tenant ID", i => i.TenantId)
    ]);

    printer.Print(identities);
}

async Task ListRoleAssignments(AzureCredentials credentials)
{
    var scope = $"/subscriptions/{credentials.SubscriptionId}";
    var roleAssignments = await AzureAuthorizationTools.ListRoleAssignments(
        credentials.TenantId, credentials.ClientId, credentials.ClientSecret, scope);

    var printer = new TablePrinter<RoleAssignment>("Role Assignments",
    [
        new ColumnDefinition<RoleAssignment>("Name", a => a.Name),
        new ColumnDefinition<RoleAssignment>("Principal ID", a => a.PrincipalId),
        new ColumnDefinition<RoleAssignment>("Principal Type", a => a.PrincipalType),
        new ColumnDefinition<RoleAssignment>("Role Definition ID", a => a.RoleDefinitionId),
        new ColumnDefinition<RoleAssignment>("Scope", a => MaskSubscriptionId(a.Scope))
    ]);

    printer.Print(roleAssignments);
}

static string? MaskSubscriptionId(string? value)
{
    if (string.IsNullOrEmpty(value)) return value;
    
    // Check if the value looks like a subscription ID
    if (value.StartsWith("/subscriptions/"))
    {
        var parts = value.Split('/');
        if (parts.Length >= 3)
        {
            // Keep first 4 and last 4 characters of the subscription ID
            var subId = parts[2];
            if (subId.Length > 8)
            {
                var masked = subId.Substring(0, 4) + "..." + subId.Substring(subId.Length - 4);
                parts[2] = masked;
                return string.Join("/", parts);
            }
        }
    }
    return value;
}

record AzureCredentials(string TenantId, string ClientId, string ClientSecret, string SubscriptionId); 