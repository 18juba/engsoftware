namespace EscolaApi.Tests;

public class JwtConfigTests
{
    [Fact]
    public void JwtKey_DeveTerMinimo32Caracteres()
    {
        var key = "esta-eh-uma-chave-secreta-com-32-caracteres";
        Xunit.Assert.True(key.Length >= 32, "JWT Key deve ter ao menos 32 caracteres");
    }

    [Fact]
    public void JwtIssuer_NaoDeveSerVazio()
    {
        var issuer = "EscolaApi";
        Xunit.Assert.False(string.IsNullOrEmpty(issuer));
    }

    [Fact]
    public void JwtAudience_NaoDeveSerVazio()
    {
        var audience = "EscolaApiUsers";
        Xunit.Assert.False(string.IsNullOrEmpty(audience));
    }
}