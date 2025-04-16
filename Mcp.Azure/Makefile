ENV_FILE := $$HOME/.env

.PHONY: build
build:
	dotnet build

.PHONY: restore
restore:
	dotnet restore

.PHONY: clean
clean:
	dotnet clean

.PHONY: test
test:
	DOTNET_ENVIRONMENT_FILE=$(ENV_FILE) dotnet test

.PHONY: run-server
run-server:
	dotnet run --project sample/Mcp.Azure.Server/Mcp.Azure.Server.csproj

.PHONY: run-console
run-console:
	@pushd sample/Mcp.Azure.Console && \
	dotnet run --project Mcp.Azure.Console.csproj --  $(ENV_FILE) && \
	popd

.PHONY: run-client
run-client:
	@pushd sample/Mcp.Azure.Client && \
	dotnet run --project Mcp.Azure.Client.csproj -- $(ENV_FILE) && \
	popd

.PHONY: run-sse-server
run-sse-server:
	dotnet run --project sample/Mcp.Azure.Server/Mcp.Azure.Server.csproj

.PHONY: inspect
inspect:
	@npx @modelcontextprotocol/inspector
