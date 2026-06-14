using EscolaApi.Data;
using EscolaApi.DTOs;
using EscolaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaApi.Services;

public interface IProfessorService
{
    Task<IEnumerable<ProfessorDto>> ListarAsync();
    Task<ProfessorDto?> ObterPorIdAsync(int id);
    Task<ProfessorDto> CriarAsync(ProfessorCreateDto dto);
    Task<bool> VincularDisciplinaAsync(int professorId, int disciplinaId);
    Task<bool> DeletarAsync(int id);
}

public class ProfessorService : IProfessorService
{
    private readonly AppDbContext _context;

    public ProfessorService(AppDbContext context) => _context = context;

    private static ProfessorDto ToDto(Professor p, Usuario u) =>
        new(p.Id, u.Nome, u.Email, p.Siape, p.Departamento);

    public async Task<IEnumerable<ProfessorDto>> ListarAsync()
        => await _context.Professores
            .Include(p => p.Usuario)
            .Select(p => new ProfessorDto(p.Id, p.Usuario.Nome, p.Usuario.Email, p.Siape, p.Departamento))
            .ToListAsync();

    public async Task<ProfessorDto?> ObterPorIdAsync(int id)
    {
        var p = await _context.Professores.Include(x => x.Usuario).FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? null : ToDto(p, p.Usuario);
    }

    public async Task<ProfessorDto> CriarAsync(ProfessorCreateDto dto)
    {
        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Tipo = TipoUsuario.Professor
        };
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var professor = new Professor
        {
            Siape = dto.Siape,
            Departamento = dto.Departamento,
            UsuarioId = usuario.Id
        };
        _context.Professores.Add(professor);
        await _context.SaveChangesAsync();

        return ToDto(professor, usuario);
    }

    public async Task<bool> VincularDisciplinaAsync(int professorId, int disciplinaId)
    {
        if (!await _context.Professores.AnyAsync(p => p.Id == professorId)) return false;
        if (!await _context.Disciplinas.AnyAsync(d => d.Id == disciplinaId)) return false;
        if (await _context.ProfessorDisciplinas.AnyAsync(pd =>
            pd.ProfessorId == professorId && pd.DisciplinaId == disciplinaId)) return false;

        _context.ProfessorDisciplinas.Add(new ProfessorDisciplina
        {
            ProfessorId = professorId,
            DisciplinaId = disciplinaId
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var p = await _context.Professores.FindAsync(id);
        if (p is null) return false;
        _context.Professores.Remove(p);
        await _context.SaveChangesAsync();
        return true;
    }
}
