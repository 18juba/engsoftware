using EscolaApi.Data;
using EscolaApi.DTOs;
using Libsql.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EscolaApi.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly IDatabaseClient _db;
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

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var id = r[0] is Integer idVal ? idVal.Value.ToString() : null;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : null;
            var email = r[2] is Text emailVal ? emailVal.Value : null;
            var senha = r[3] is Text senhaVal ? senhaVal.Value : null;
            var tipo = r[4] is Text tipoVal ? tipoVal.Value : null;

            if (senha is null || !BCrypt.Net.BCrypt.Verify(request.Senha, senha))
                return null;

            var token = GerarToken(id!, nome!, email!, tipo!);
            return new LoginResponse(token, nome!, tipo!);
        }

        return null; // Usuário não encontrado
    }

    private string GerarToken(string id, string nome, string email, string tipo)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
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