.PHONY: build clean test run-server run-console

build:
	dotnet build

restore:
	dotnet restore

clean:
	dotnet clean

test:
	dotnet test

run-server:
	dotnet run --project sample/Mcp.Azure.Server/Mcp.Azure.Server.csproj

run-console:
	@pushd sample/Mcp.Azure.Console && \
	dotnet run --project Mcp.Azure.Console.csproj --  ../../.env && \
	popd

run-client:
	@pushd sample/Mcp.Azure.Client && \
	dotnet run --project Mcp.Azure.Client.csproj -- ../../.env && \
	popd
