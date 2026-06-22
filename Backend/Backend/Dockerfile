FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app

# Copia o csproj apontando para a subpasta correta
COPY Backend/*.csproj ./
RUN dotnet restore

# Copia todo o conteúdo da subpasta de código para o container
COPY Backend/ ./
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Altera para root temporariamente para instalar o curl sem erro de permissão
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=builder /app/publish .

# Volta para o usuário seguro do .NET
USER app

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Backend.dll"]