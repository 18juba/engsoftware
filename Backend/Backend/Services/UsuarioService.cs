using EscolaApi.Data;
using EscolaApi.DTOs;
using EscolaApi.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace EscolaApi.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ListarAsync();
    Task<UsuarioDto?> ObterPorIdAsync(int id);
    Task<UsuarioDto> CriarAsync(UsuarioCreateDto dto);
    Task<bool> DeletarAsync(int id);
}

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<UsuarioDto>> ListarAsync()
        => await _context.Usuarios
            .Select(u => new UsuarioDto(u.Id, u.Nome, u.Email, u.Tipo))
            .ToListAsync();

    public async Task<UsuarioDto?> ObterPorIdAsync(int id)
    {
        var u = await _context.Usuarios.FindAsync(id);
        return u is null ? null : new UsuarioDto(u.Id, u.Nome, u.Email, u.Tipo);
    }

    public async Task<UsuarioDto> CriarAsync(UsuarioCreateDto dto)
    {
        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Tipo = dto.Tipo
        };
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email, usuario.Tipo);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var u = await _context.Usuarios.FindAsync(id);
        if (u is null) return false;
        _context.Usuarios.Remove(u);
        await _context.SaveChangesAsync();
        return true;
    }
}
