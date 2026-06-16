using EscolaApi.Data;
using EscolaApi.DTOs;
using Libsql.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    private readonly IDatabaseClient _db;

    public DisciplinaService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<DisciplinaDto>> ListarAsync()
    {
        var rs = await _db.Execute("SELECT Id, Nome, Codigo, CargaHoraria FROM Disciplinas");
        var list = new List<DisciplinaDto>();

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var id = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : string.Empty;
            var codigo = r[2] is Text codVal ? codVal.Value : string.Empty;
            var cargaHoraria = r[3] is Integer cargaVal ? (int)cargaVal.Value : 0;

            list.Add(new DisciplinaDto(id, nome, codigo, cargaHoraria));
        }
        return list;
    }

    public async Task<DisciplinaDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id, Nome, Codigo, CargaHoraria FROM Disciplinas WHERE Id = ?",
            new object[] { id });

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var disciplinaId = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : string.Empty;
            var codigo = r[2] is Text codVal ? codVal.Value : string.Empty;
            var cargaHoraria = r[3] is Integer cargaVal ? (int)cargaVal.Value : 0;

            return new DisciplinaDto(disciplinaId, nome, codigo, cargaHoraria);
        }
        return null;
    }

    public async Task<DisciplinaDto> CriarAsync(DisciplinaCreateDto dto)
    {
        await _db.Execute("INSERT INTO Disciplinas (Nome, Codigo, CargaHoraria) VALUES (?, ?, ?)",
            new object[] { dto.Nome, dto.Codigo, dto.CargaHoraria });

        var rs = await _db.Execute("SELECT last_insert_rowid()");
        long id = 0;
        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();
            id = r[0] is Integer idVal ? idVal.Value : 0;
        }

        return new DisciplinaDto((int)id, dto.Nome, dto.Codigo, dto.CargaHoraria);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Disciplinas WHERE Id = ?", new object[] { id });
        if (!rs.Rows.Any()) return false;

        await _db.Execute("DELETE FROM Disciplinas WHERE Id = ?", new object[] { id });
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
    private readonly IDatabaseClient _db;

    public TurmaService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<TurmaDto>> ListarAsync()
    {
        var rs = await _db.Execute(@"
            SELECT t.Id, t.Semestre, t.Horario, 
                   d.Id, d.Nome, d.Codigo, d.CargaHoraria 
            FROM Turmas t JOIN Disciplinas d ON t.DisciplinaId = d.Id");

        var list = new List<TurmaDto>();

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var id = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var semestre = r[1] is Text semVal ? semVal.Value : string.Empty;
            var horario = r[2] is Text horVal ? horVal.Value : string.Empty;

            var discId = r[3] is Integer discIdVal ? (int)discIdVal.Value : 0;
            var discNome = r[4] is Text discNomeVal ? discNomeVal.Value : string.Empty;
            var discCodigo = r[5] is Text discCodVal ? discCodVal.Value : string.Empty;
            var discCarga = r[6] is Integer discCargaVal ? (int)discCargaVal.Value : 0;

            var disciplina = new DisciplinaDto(discId, discNome, discCodigo, discCarga);

            list.Add(new TurmaDto(id, semestre, horario, disciplina));
        }
        return list;
    }

    public async Task<TurmaDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(@"
            SELECT t.Id, t.Semestre, t.Horario, 
                   d.Id, d.Nome, d.Codigo, d.CargaHoraria 
            FROM Turmas t JOIN Disciplinas d ON t.DisciplinaId = d.Id 
            WHERE t.Id = ?", new object[] { id });

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var turmaId = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var semestre = r[1] is Text semVal ? semVal.Value : string.Empty;
            var horario = r[2] is Text horVal ? horVal.Value : string.Empty;

            var discId = r[3] is Integer discIdVal ? (int)discIdVal.Value : 0;
            var discNome = r[4] is Text discNomeVal ? discNomeVal.Value : string.Empty;
            var discCodigo = r[5] is Text discCodVal ? discCodVal.Value : string.Empty;
            var discCarga = r[6] is Integer discCargaVal ? (int)discCargaVal.Value : 0;

            var disciplina = new DisciplinaDto(discId, discNome, discCodigo, discCarga);

            return new TurmaDto(turmaId, semestre, horario, disciplina);
        }
        return null;
    }

    public async Task<TurmaDto> CriarAsync(TurmaCreateDto dto)
    {
        await _db.Execute("INSERT INTO Turmas (Semestre, Horario, DisciplinaId) VALUES (?, ?, ?)",
            new object[] { dto.Semestre, dto.Horario, dto.DisciplinaId });

        var rs = await _db.Execute("SELECT last_insert_rowid()");
        long id = 0;
        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();
            id = r[0] is Integer idVal ? idVal.Value : 0;
        }

        var disciplina = await ObterDisciplinaPorId(dto.DisciplinaId);
        return new TurmaDto((int)id, dto.Semestre, dto.Horario, disciplina!);
    }

    private async Task<DisciplinaDto?> ObterDisciplinaPorId(int id)
    {
        var rs = await _db.Execute("SELECT Id, Nome, Codigo, CargaHoraria FROM Disciplinas WHERE Id = ?",
            new object[] { id });

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var discId = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : string.Empty;
            var codigo = r[2] is Text codVal ? codVal.Value : string.Empty;
            var carga = r[3] is Integer cargaVal ? (int)cargaVal.Value : 0;

            return new DisciplinaDto(discId, nome, codigo, carga);
        }
        return null;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Turmas WHERE Id = ?", new object[] { id });
        if (!rs.Rows.Any()) return false;

        await _db.Execute("DELETE FROM Turmas WHERE Id = ?", new object[] { id });
        return true;
    }
}