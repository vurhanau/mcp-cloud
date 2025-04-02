restore:
	pushd Mcp.Azure && dotnet restore && popd

build:
	pushd Mcp.Azure && dotnet build && popd

run:
	dotnet run --project Mcp.Azure/sample/Mcp.Azure.Server/Mcp.Azure.Server.csproj
