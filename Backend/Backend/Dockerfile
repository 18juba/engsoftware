FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app

COPY Backend/Backend/Backend.csproj ./
RUN dotnet restore Backend.csproj

COPY Backend/Backend/ ./
RUN dotnet publish Backend.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=builder /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Backend.dll"]