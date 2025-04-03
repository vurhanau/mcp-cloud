using Mcp.Azure.Authorization;
using Mcp.Azure.Context;
using Mcp.Azure.ManagedServiceIdentities;
using Mcp.Azure.Graph;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.None);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(typeof(AzureContextTools).Assembly)
    .WithToolsFromAssembly(typeof(AzureGraphTools).Assembly)
    .WithToolsFromAssembly(typeof(AzureAuthorizationTools).Assembly)
    .WithToolsFromAssembly(typeof(AzureManagedServiceIdentitiesTools).Assembly);

var app = builder.Build();
await app.RunAsync();
