FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app

# Como o GitHub Actions constrói a partir de Backend/Backend, os arquivos estão na raiz do contexto
COPY *.csproj ./
RUN dotnet restore

# Copia tudo o que está na pasta atual do código
COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=builder /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Backend.dll"]