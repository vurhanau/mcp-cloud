using dotenv.net;

namespace Mcp.Azure.Context;

public class AzureContext
{
    public string TenantId { get; }
    public string ClientId { get; }
    public string ClientSecret { get; }
    public string SubscriptionId { get; }

    private AzureContext(string tenantId, string clientId, string clientSecret, string subscriptionId)
    {
        TenantId = tenantId;
        ClientId = clientId;
        ClientSecret = clientSecret;
        SubscriptionId = subscriptionId;
    }

    public static AzureContext? LoadFromEnvironment(string? envPath = null)
    {
        envPath ??= Path.Combine(Directory.GetCurrentDirectory(), ".env");

        if (File.Exists(envPath))
        {
            DotEnv.Load(new DotEnvOptions(envFilePaths: [envPath]));
        }

        var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
        var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
        var subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || 
            string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(subscriptionId))
        {
            return null;
        }

        return new AzureContext(tenantId, clientId, clientSecret, subscriptionId);
    }
} 