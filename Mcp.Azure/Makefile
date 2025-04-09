ENV_FILE := $$HOME/.env

.PHONY: build clean test run-server run-console

build:
	dotnet build

restore:
	dotnet restore

clean:
	dotnet clean

test:
	DOTNET_ENVIRONMENT_FILE=$(ENV_FILE) dotnet test

run-server:
	dotnet run --project sample/Mcp.Azure.Server/Mcp.Azure.Server.csproj

run-console:
	@pushd sample/Mcp.Azure.Console && \
	dotnet run --project Mcp.Azure.Console.csproj --  $(ENV_FILE) && \
	popd

run-client:
	@pushd sample/Mcp.Azure.Client && \
	dotnet run --project Mcp.Azure.Client.csproj -- $(ENV_FILE) && \
	popd
