using Mcp.Azure.Authorization;
using Mcp.Azure.Context;
using Mcp.Azure.ManagedServiceIdentities;
using Mcp.Azure.Graph;
using Mcp.Azure.ResourceManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

Console.WriteLine("Starting Azure MCP server...");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly(typeof(AzureContextTools).Assembly)
    .WithToolsFromAssembly(typeof(AzureResourceManagerServer).Assembly)
    .WithToolsFromAssembly(typeof(AzureGraphTools).Assembly)
    .WithToolsFromAssembly(typeof(AzureAuthorizationTools).Assembly)
    .WithToolsFromAssembly(typeof(AzureManagedServiceIdentitiesTools).Assembly);

var app = builder.Build();

app.MapMcp();

app.Run();
