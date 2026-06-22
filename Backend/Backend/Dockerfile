FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app

# Como o contexto já está na primeira pasta Backend, o .csproj está na subpasta Backend/
COPY Backend/*.csproj ./
RUN dotnet restore

# Copia todo o código da subpasta para a pasta atual do container
COPY Backend/ ./
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Corrige a permissão para instalar o curl sem dar erro no stage de produção
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=builder /app/publish .

# Volta para o usuário seguro do .NET 10
USER app

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Backend.dll"]