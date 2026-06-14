using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using EscolaApi.Data;
using EscolaApi.DTOs;
using EscolaApi.Models;

namespace EscolaApi.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, IConfiguration config, ILogger<AuthService> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Tentativa de login recebida para o e-mail: {Email}", request.Email);

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
        {
            _logger.LogWarning("Falha na autenticação: E-mail ou senha inválidos para {Email}", request.Email);
            return null;
        }

        _logger.LogInformation("Usuário {Email} autenticado com sucesso. Gerando token JWT...", request.Email);

        var token = GerarToken(usuario);

        return new LoginResponse(token, usuario.Nome, usuario.Tipo.ToString());
    }

    private string GerarToken(Usuario usuario)
    {
        var jwtKey = _config["Jwt:Key"];

        if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
        {
            _logger.LogCritical("A chave configurada em 'Jwt:Key' está vazia ou possui menos de 32 caracteres (256 bits).");
            throw new InvalidOperationException("Configuração de segurança JWT inválida no servidor.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Tipo.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}