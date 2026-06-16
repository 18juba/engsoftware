using Libsql.Client;

using ILibSqlClient = Libsql.Client.IDatabaseClient;

namespace EscolaApi.Data;

public class TursoClient
{
    private readonly IDatabaseClient _client;

    public TursoClient(IConfiguration config)
    {
        var url = Environment.GetEnvironmentVariable("TURSO_DATABASE_URL")
            ?? config["Turso:DatabaseUrl"]!;
        var token = Environment.GetEnvironmentVariable("TURSO_AUTH_TOKEN")
            ?? config["Turso:AuthToken"]!;

        _client = DatabaseClient.Create(opts =>
        {
            opts.Url = url;
            opts.AuthToken = token;
        }).GetAwaiter().GetResult();
    }

    public ILibSqlClient Client => _client;
}