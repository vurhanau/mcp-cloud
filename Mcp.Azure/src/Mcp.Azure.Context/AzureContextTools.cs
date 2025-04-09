using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Mcp.Azure.Context;

[McpServerToolType]
public static class AzureContextTools
{
    [McpServerTool, Description("Load Azure context from environment variables.")]
    public static Task<AzureContext> LoadContext(string? envPath = null)
    {
        var context = AzureContext.LoadFromEnvironment(envPath);
        if (context == null)
        {
            throw new InvalidOperationException("Azure context not found in environment variables.");
        }

        return Task.FromResult(context);
    }
}
