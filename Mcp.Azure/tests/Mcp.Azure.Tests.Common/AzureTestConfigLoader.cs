using Azure.Identity;
using dotenv.net;

namespace Mcp.Azure.Tests.Common;

public class AzureTestConfigLoader
{
    public static AzureTestConfig FromDotEnv()
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var envFile = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT_FILE") ?? Path.Combine(homeDirectory, ".env");
        return FromDotEnv(envFile);
    }

    public static AzureTestConfig FromDotEnv(string dotEnvFilePath)
    {
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: [dotEnvFilePath]));

        var tenantId = GetEnvironmentVariable("AZURE_TENANT_ID");
        var clientId = GetEnvironmentVariable("AZURE_CLIENT_ID");
        var clientSecret = GetEnvironmentVariable("AZURE_CLIENT_SECRET");
        var subscriptionId = GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");
        var principalId = GetEnvironmentVariable("TEST_PRINCIPAL_ID");

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

        return new AzureTestConfig(
            subscriptionId: subscriptionId,
            tenantId: tenantId,
            clientId: clientId,
            clientSecret: clientSecret,
            credential: credential,
            principalId: principalId
        );
    }

    private static string GetEnvironmentVariable(string variableName)
    {
        return Environment.GetEnvironmentVariable(variableName) 
               ?? throw new InvalidOperationException($"{variableName} environment variable is not set");
    }
}
