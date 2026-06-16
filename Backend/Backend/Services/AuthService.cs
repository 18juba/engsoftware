using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EscolaApi.Data;
using EscolaApi.DTOs;
using EscolaApi.Models;
using LibSql.Client;
using Microsoft.IdentityModel.Tokens;

namespace EscolaApi.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly ILibSqlClient _db;
    private readonly IConfiguration _config;

    public AuthService(TursoClient turso, IConfiguration config)
    {
        _db = turso.Client;
        _config = config;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var rs = await _db.Execute(
            "SELECT Id, Nome, Email, Senha, Tipo FROM Usuarios WHERE Email = ?",
            new object[] { request.Email });

        string? id = null, nome = null, email = null, senha = null, tipo = null;
        await foreach (var row in rs.Rows)
        {
            id    = row[0].ToString();
            nome  = (string)row[1];
            email = (string)row[2];
            senha = (string)row[3];
            tipo  = (string)row[4];
        }

        if (senha is null || !BCrypt.Net.BCrypt.Verify(request.Senha, senha))
            return null;

        var token = GerarToken(id!, nome!, email!, tipo!);
        return new LoginResponse(token, nome!, tipo!);
    }

    private string GerarToken(string id, string nome, string email, string tipo)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, nome),
            new Claim(ClaimTypes.Role, tipo)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}