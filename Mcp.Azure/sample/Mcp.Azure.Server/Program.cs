using Mcp.Azure.Rbac;
using Microsoft.Extensions.Hosting;

var builder = AzureRbacServer.CreateBuilder(args);
await builder.Build().RunAsync();
