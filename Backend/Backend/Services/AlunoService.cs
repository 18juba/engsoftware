using EscolaApi.Data;
using EscolaApi.DTOs;
using EscolaApi.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace EscolaApi.Services;

public interface IAlunoService
{
    Task<IEnumerable<AlunoDto>> ListarAsync();
    Task<AlunoDto?> ObterPorIdAsync(int id);
    Task<AlunoDto> CriarAsync(AlunoCreateDto dto);
    Task<bool> DeletarAsync(int id);
}

public class AlunoService : IAlunoService
{
    private readonly AppDbContext _context;

    public AlunoService(AppDbContext context) => _context = context;

    private static AlunoDto ToDto(Aluno a, Usuario u) =>
        new(a.Id, u.Nome, u.Email, a.Matricula, a.Curso);

    public async Task<IEnumerable<AlunoDto>> ListarAsync()
        => await _context.Alunos
            .Include(a => a.Usuario)
            .Select(a => new AlunoDto(a.Id, a.Usuario.Nome, a.Usuario.Email, a.Matricula, a.Curso))
            .ToListAsync();

    public async Task<AlunoDto?> ObterPorIdAsync(int id)
    {
        var a = await _context.Alunos.Include(x => x.Usuario).FirstOrDefaultAsync(x => x.Id == id);
        return a is null ? null : ToDto(a, a.Usuario);
    }

    public async Task<AlunoDto> CriarAsync(AlunoCreateDto dto)
    {
        try
        {
            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Tipo = TipoUsuario.Aluno
            };

            var aluno = new Aluno
            {
                Matricula = dto.Matricula,
                Curso = dto.Curso,
                Usuario = usuario
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            return ToDto(aluno, usuario);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            var erroReal = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"[ERRO BANCO POSTGRES]: {erroReal}", ex);
        }
    }
    public async Task<bool> DeletarAsync(int id)
    {
        var a = await _context.Alunos.FindAsync(id);
        if (a is null) return false;
        _context.Alunos.Remove(a);
        await _context.SaveChangesAsync();
        return true;
    }
}
