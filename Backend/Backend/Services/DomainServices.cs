using EscolaApi.Data;
using EscolaApi.DTOs;
using EscolaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaApi.Services;

// ── Disciplina ───────────────────────────────────────────────
public interface IDisciplinaService
{
    Task<IEnumerable<DisciplinaDto>> ListarAsync();
    Task<DisciplinaDto?> ObterPorIdAsync(int id);
    Task<DisciplinaDto> CriarAsync(DisciplinaCreateDto dto);
    Task<bool> DeletarAsync(int id);
}

public class DisciplinaService : IDisciplinaService
{
    private readonly AppDbContext _context;
    public DisciplinaService(AppDbContext context) => _context = context;

    private static DisciplinaDto ToDto(Disciplina d) => new(d.Id, d.Nome, d.Codigo, d.CargaHoraria);

    public async Task<IEnumerable<DisciplinaDto>> ListarAsync()
        => await _context.Disciplinas.Select(d => new DisciplinaDto(d.Id, d.Nome, d.Codigo, d.CargaHoraria)).ToListAsync();

    public async Task<DisciplinaDto?> ObterPorIdAsync(int id)
    {
        var d = await _context.Disciplinas.FindAsync(id);
        return d is null ? null : ToDto(d);
    }

    public async Task<DisciplinaDto> CriarAsync(DisciplinaCreateDto dto)
    {
        var d = new Disciplina { Nome = dto.Nome, Codigo = dto.Codigo, CargaHoraria = dto.CargaHoraria };
        _context.Disciplinas.Add(d);
        await _context.SaveChangesAsync();
        return ToDto(d);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var d = await _context.Disciplinas.FindAsync(id);
        if (d is null) return false;
        _context.Disciplinas.Remove(d);
        await _context.SaveChangesAsync();
        return true;
    }
}

// ── Turma ────────────────────────────────────────────────────
public interface ITurmaService
{
    Task<IEnumerable<TurmaDto>> ListarAsync();
    Task<TurmaDto?> ObterPorIdAsync(int id);
    Task<TurmaDto> CriarAsync(TurmaCreateDto dto);
    Task<bool> DeletarAsync(int id);
}

public class TurmaService : ITurmaService
{
    private readonly AppDbContext _context;
    public TurmaService(AppDbContext context) => _context = context;

    private static TurmaDto ToDto(Turma t) =>
        new(t.Id, t.Semestre, t.Horario, new DisciplinaDto(t.Disciplina.Id, t.Disciplina.Nome, t.Disciplina.Codigo, t.Disciplina.CargaHoraria));

    public async Task<IEnumerable<TurmaDto>> ListarAsync()
        => await _context.Turmas.Include(t => t.Disciplina)
            .Select(t => new TurmaDto(t.Id, t.Semestre, t.Horario,
                new DisciplinaDto(t.Disciplina.Id, t.Disciplina.Nome, t.Disciplina.Codigo, t.Disciplina.CargaHoraria)))
            .ToListAsync();

    public async Task<TurmaDto?> ObterPorIdAsync(int id)
    {
        var t = await _context.Turmas.Include(x => x.Disciplina).FirstOrDefaultAsync(x => x.Id == id);
        return t is null ? null : ToDto(t);
    }

    public async Task<TurmaDto> CriarAsync(TurmaCreateDto dto)
    {
        var t = new Turma { Semestre = dto.Semestre, Horario = dto.Horario, DisciplinaId = dto.DisciplinaId };
        _context.Turmas.Add(t);
        await _context.SaveChangesAsync();
        await _context.Entry(t).Reference(x => x.Disciplina).LoadAsync();
        return ToDto(t);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var t = await _context.Turmas.FindAsync(id);
        if (t is null) return false;
        _context.Turmas.Remove(t);
        await _context.SaveChangesAsync();
        return true;
    }
}

// ── Matricula ────────────────────────────────────────────────
public interface IMatriculaService
{
    Task<IEnumerable<MatriculaDto>> ListarAsync();
    Task<MatriculaDto?> ObterPorIdAsync(int id);
    Task<MatriculaDto> CriarAsync(MatriculaCreateDto dto);
    Task<bool> AlterarStatusAsync(int id, StatusMatricula status);
    Task<bool> DeletarAsync(int id);
}

public class MatriculaService : IMatriculaService
{
    private readonly AppDbContext _context;
    public MatriculaService(AppDbContext context) => _context = context;

    private static MatriculaDto ToDto(Matricula m) =>
        new(m.Id, m.Data, m.Status.ToString(),
            new AlunoDto(m.Aluno.Id, m.Aluno.Usuario.Nome, m.Aluno.Usuario.Email, m.Aluno.Matricula, m.Aluno.Curso),
            m.TurmaId);

    private IQueryable<Matricula> Query() =>
        _context.Matriculas.Include(m => m.Aluno).ThenInclude(a => a.Usuario);

    public async Task<IEnumerable<MatriculaDto>> ListarAsync()
        => await Query().Select(m => new MatriculaDto(m.Id, m.Data, m.Status.ToString(),
            new AlunoDto(m.Aluno.Id, m.Aluno.Usuario.Nome, m.Aluno.Usuario.Email, m.Aluno.Matricula, m.Aluno.Curso),
            m.TurmaId)).ToListAsync();

    public async Task<MatriculaDto?> ObterPorIdAsync(int id)
    {
        var m = await Query().FirstOrDefaultAsync(x => x.Id == id);
        return m is null ? null : ToDto(m);
    }

    public async Task<MatriculaDto> CriarAsync(MatriculaCreateDto dto)
    {
        var m = new Matricula { AlunoId = dto.AlunoId, TurmaId = dto.TurmaId, Data = DateTime.UtcNow, Status = StatusMatricula.Ativa };
        _context.Matriculas.Add(m);
        await _context.SaveChangesAsync();
        await _context.Entry(m).Reference(x => x.Aluno).LoadAsync();
        await _context.Entry(m.Aluno).Reference(x => x.Usuario).LoadAsync();
        return ToDto(m);
    }

    public async Task<bool> AlterarStatusAsync(int id, StatusMatricula status)
    {
        var m = await _context.Matriculas.FindAsync(id);
        if (m is null) return false;
        m.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var m = await _context.Matriculas.FindAsync(id);
        if (m is null) return false;
        _context.Matriculas.Remove(m);
        await _context.SaveChangesAsync();
        return true;
    }
}

// ── Atividade ────────────────────────────────────────────────
public interface IAtividadeService
{
    Task<IEnumerable<AtividadeDto>> ListarAsync();
    Task<IEnumerable<AtividadeDto>> ListarPorTurmaAsync(int turmaId);
    Task<AtividadeDto?> ObterPorIdAsync(int id);
    Task<AtividadeDto> CriarAsync(AtividadeCreateDto dto);
    Task<bool> DeletarAsync(int id);
}

public class AtividadeService : IAtividadeService
{
    private readonly AppDbContext _context;
    public AtividadeService(AppDbContext context) => _context = context;

    private static AtividadeDto ToDto(Atividade a) =>
        new(a.Id, a.Titulo, a.Descricao, a.Prazo, a.TurmaId);

    public async Task<IEnumerable<AtividadeDto>> ListarAsync()
        => await _context.Atividades.Select(a => new AtividadeDto(a.Id, a.Titulo, a.Descricao, a.Prazo, a.TurmaId)).ToListAsync();

    public async Task<IEnumerable<AtividadeDto>> ListarPorTurmaAsync(int turmaId)
        => await _context.Atividades.Where(a => a.TurmaId == turmaId)
            .Select(a => new AtividadeDto(a.Id, a.Titulo, a.Descricao, a.Prazo, a.TurmaId)).ToListAsync();

    public async Task<AtividadeDto?> ObterPorIdAsync(int id)
    {
        var a = await _context.Atividades.FindAsync(id);
        return a is null ? null : ToDto(a);
    }

    public async Task<AtividadeDto> CriarAsync(AtividadeCreateDto dto)
    {
        var a = new Atividade { Titulo = dto.Titulo, Descricao = dto.Descricao, Prazo = dto.Prazo, TurmaId = dto.TurmaId };
        _context.Atividades.Add(a);
        await _context.SaveChangesAsync();
        return ToDto(a);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var a = await _context.Atividades.FindAsync(id);
        if (a is null) return false;
        _context.Atividades.Remove(a);
        await _context.SaveChangesAsync();
        return true;
    }
}

// ── Entrega ──────────────────────────────────────────────────
public interface IEntregaService
{
    Task<IEnumerable<EntregaDto>> ListarAsync();
    Task<IEnumerable<EntregaDto>> ListarPorAtividadeAsync(int atividadeId);
    Task<EntregaDto?> ObterPorIdAsync(int id);
    Task<EntregaDto> CriarAsync(EntregaCreateDto dto);
    Task<bool> LancarNotaAsync(int id, decimal nota);
    Task<bool> DeletarAsync(int id);
}

public class EntregaService : IEntregaService
{
    private readonly AppDbContext _context;
    public EntregaService(AppDbContext context) => _context = context;

    private static EntregaDto ToDto(Entrega e) =>
        new(e.Id, e.DataEntrega, e.Nota, e.AlunoId, e.AtividadeId);

    public async Task<IEnumerable<EntregaDto>> ListarAsync()
        => await _context.Entregas.Select(e => new EntregaDto(e.Id, e.DataEntrega, e.Nota, e.AlunoId, e.AtividadeId)).ToListAsync();

    public async Task<IEnumerable<EntregaDto>> ListarPorAtividadeAsync(int atividadeId)
        => await _context.Entregas.Where(e => e.AtividadeId == atividadeId)
            .Select(e => new EntregaDto(e.Id, e.DataEntrega, e.Nota, e.AlunoId, e.AtividadeId)).ToListAsync();

    public async Task<EntregaDto?> ObterPorIdAsync(int id)
    {
        var e = await _context.Entregas.FindAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<EntregaDto> CriarAsync(EntregaCreateDto dto)
    {
        var e = new Entrega { AlunoId = dto.AlunoId, AtividadeId = dto.AtividadeId, DataEntrega = DateTime.UtcNow };
        _context.Entregas.Add(e);
        await _context.SaveChangesAsync();
        return ToDto(e);
    }

    public async Task<bool> LancarNotaAsync(int id, decimal nota)
    {
        var e = await _context.Entregas.FindAsync(id);
        if (e is null) return false;
        e.Nota = nota;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var e = await _context.Entregas.FindAsync(id);
        if (e is null) return false;
        _context.Entregas.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }
}
