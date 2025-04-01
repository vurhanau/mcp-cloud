using dotenv.net;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using System.Text.Json;

// Get .env file path from command line args or use default
var envPath = args.Length > 0 ? args[0] : "../../.env";
Console.WriteLine($"Using .env file: {Path.GetFullPath(envPath)}");

// Load .env file
DotEnv.Load(options: new DotEnvOptions(envFilePaths: [envPath]));

var client = await McpClientFactory.CreateAsync(new()
{
    Id = "everything",
    Name = "Everything",
    TransportType = TransportTypes.StdIo,
    TransportOptions = new()
    {
        ["command"] = "dotnet",
        ["arguments"] = "run --project ../Mcp.Azure.Server/Mcp.Azure.Server.csproj",
    }
});

// Print the list of tools available from the server.
foreach (var tool in await client.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}

// Get Azure credentials from environment variables
var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
var subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

Console.WriteLine($"Tenant ID: {tenantId}");
Console.WriteLine($"Client ID: {clientId}");
Console.WriteLine($"Client Secret: {MaskSecret(clientSecret)}");
Console.WriteLine($"Subscription ID: {subscriptionId}");

if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || 
    string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(subscriptionId))
{
    throw new InvalidOperationException($"Azure credentials not found in environment variables. Please check your .env file at: {envPath}");
}

// Execute a tool with credentials from .env file
var result = await client.CallToolAsync(
    "ListRoleAssignments",
    new Dictionary<string, object?>
    {
        ["tenantId"] = tenantId,
        ["clientId"] = clientId,
        ["clientSecret"] = clientSecret,
        ["scope"] = $"/subscriptions/{subscriptionId}"
    },
    CancellationToken.None);

// Print the results
foreach (var assignment in result.Content.Where(c => c.Type == "text"))
{
    try
    {
        if (assignment.Text is null) continue;
        // Try to parse and pretty print as JSON
        var jsonDoc = JsonDocument.Parse(assignment.Text);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var prettyJson = JsonSerializer.Serialize(jsonDoc, options);
        Console.WriteLine(prettyJson);
    }
    catch (JsonException)
    {
        // If not JSON, print as plain text
        Console.WriteLine(assignment.Text);
    }
}

static string MaskSecret(string? secret)
{
    if (string.IsNullOrEmpty(secret)) return "null";
    if (secret.Length <= 8) return "***";
    return $"{secret[..4]}...{secret[^4..]}";
}
