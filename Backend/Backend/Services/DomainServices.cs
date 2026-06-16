using EscolaApi.Data;
using EscolaApi.DTOs;
using EscolaApi.Models;
using LibSql.Client;

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
    private readonly ILibSqlClient _db;
    public DisciplinaService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<DisciplinaDto>> ListarAsync()
    {
        var rs = await _db.Execute("SELECT Id, Nome, Codigo, CargaHoraria FROM Disciplinas");
        var list = new List<DisciplinaDto>();
        await foreach (var row in rs.Rows)
            list.Add(new DisciplinaDto((int)(long)row[0], (string)row[1], (string)row[2], (int)(long)row[3]));
        return list;
    }

    public async Task<DisciplinaDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id, Nome, Codigo, CargaHoraria FROM Disciplinas WHERE Id = ?", new object[] { id });
        await foreach (var row in rs.Rows)
            return new DisciplinaDto((int)(long)row[0], (string)row[1], (string)row[2], (int)(long)row[3]);
        return null;
    }

    public async Task<DisciplinaDto> CriarAsync(DisciplinaCreateDto dto)
    {
        await _db.Execute("INSERT INTO Disciplinas (Nome, Codigo, CargaHoraria) VALUES (?, ?, ?)",
            new object[] { dto.Nome, dto.Codigo, dto.CargaHoraria });
        var rs = await _db.Execute("SELECT last_insert_rowid()");
        long id = 0; await foreach (var row in rs.Rows) id = (long)row[0];
        return new DisciplinaDto((int)id, dto.Nome, dto.Codigo, dto.CargaHoraria);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Disciplinas WHERE Id = ?", new object[] { id });
        bool existe = false; await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
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
    private readonly ILibSqlClient _db;
    public TurmaService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<TurmaDto>> ListarAsync()
    {
        var rs = await _db.Execute(
            "SELECT t.Id, t.Semestre, t.Horario, d.Id, d.Nome, d.Codigo, d.CargaHoraria " +
            "FROM Turmas t JOIN Disciplinas d ON t.DisciplinaId = d.Id");
        var list = new List<TurmaDto>();
        await foreach (var row in rs.Rows)
            list.Add(new TurmaDto((int)(long)row[0], (string)row[1], (string)row[2],
                new DisciplinaDto((int)(long)row[3], (string)row[4], (string)row[5], (int)(long)row[6])));
        return list;
    }

    public async Task<TurmaDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT t.Id, t.Semestre, t.Horario, d.Id, d.Nome, d.Codigo, d.CargaHoraria " +
            "FROM Turmas t JOIN Disciplinas d ON t.DisciplinaId = d.Id WHERE t.Id = ?",
            new object[] { id });
        await foreach (var row in rs.Rows)
            return new TurmaDto((int)(long)row[0], (string)row[1], (string)row[2],
                new DisciplinaDto((int)(long)row[3], (string)row[4], (string)row[5], (int)(long)row[6]));
        return null;
    }

    public async Task<TurmaDto> CriarAsync(TurmaCreateDto dto)
    {
        await _db.Execute("INSERT INTO Turmas (Semestre, Horario, DisciplinaId) VALUES (?, ?, ?)",
            new object[] { dto.Semestre, dto.Horario, dto.DisciplinaId });
        var rs = await _db.Execute("SELECT last_insert_rowid()");
        long id = 0; await foreach (var row in rs.Rows) id = (long)row[0];
        var d = await ObterDisciplina(dto.DisciplinaId);
        return new TurmaDto((int)id, dto.Semestre, dto.Horario, d!);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Turmas WHERE Id = ?", new object[] { id });
        bool existe = false; await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
        await _db.Execute("DELETE FROM Turmas WHERE Id = ?", new object[] { id });
        return true;
    }

    private async Task<DisciplinaDto?> ObterDisciplina(int id)
    {
        var rs = await _db.Execute("SELECT Id, Nome, Codigo, CargaHoraria FROM Disciplinas WHERE Id = ?", new object[] { id });
        await foreach (var row in rs.Rows)
            return new DisciplinaDto((int)(long)row[0], (string)row[1], (string)row[2], (int)(long)row[3]);
        return null;
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
    private readonly ILibSqlClient _db;
    public MatriculaService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<MatriculaDto>> ListarAsync()
    {
        var rs = await _db.Execute(
            "SELECT m.Id, m.Data, m.Status, m.TurmaId, a.Id, u.Nome, u.Email, a.Matricula, a.Curso " +
            "FROM Matriculas m JOIN Alunos a ON m.AlunoId = a.Id JOIN Usuarios u ON a.UsuarioId = u.Id");
        var list = new List<MatriculaDto>();
        await foreach (var row in rs.Rows)
            list.Add(new MatriculaDto((int)(long)row[0], DateTime.Parse((string)row[1]), (string)row[2],
                new AlunoDto((int)(long)row[4], (string)row[5], (string)row[6], (string)row[7], (string)row[8]),
                (int)(long)row[3]));
        return list;
    }

    public async Task<MatriculaDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT m.Id, m.Data, m.Status, m.TurmaId, a.Id, u.Nome, u.Email, a.Matricula, a.Curso " +
            "FROM Matriculas m JOIN Alunos a ON m.AlunoId = a.Id JOIN Usuarios u ON a.UsuarioId = u.Id WHERE m.Id = ?",
            new object[] { id });
        await foreach (var row in rs.Rows)
            return new MatriculaDto((int)(long)row[0], DateTime.Parse((string)row[1]), (string)row[2],
                new AlunoDto((int)(long)row[4], (string)row[5], (string)row[6], (string)row[7], (string)row[8]),
                (int)(long)row[3]);
        return null;
    }

    public async Task<MatriculaDto> CriarAsync(MatriculaCreateDto dto)
    {
        var data = DateTime.UtcNow.ToString("o");
        await _db.Execute(
            "INSERT INTO Matriculas (Data, Status, AlunoId, TurmaId) VALUES (?, 'Ativa', ?, ?)",
            new object[] { data, dto.AlunoId, dto.TurmaId });
        var rs = await _db.Execute("SELECT last_insert_rowid()");
        long id = 0; await foreach (var row in rs.Rows) id = (long)row[0];
        return (await ObterPorIdAsync((int)id))!;
    }

    public async Task<bool> AlterarStatusAsync(int id, StatusMatricula status)
    {
        var rs = await _db.Execute("SELECT Id FROM Matriculas WHERE Id = ?", new object[] { id });
        bool existe = false; await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
        await _db.Execute("UPDATE Matriculas SET Status = ? WHERE Id = ?",
            new object[] { status.ToString(), id });
        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Matriculas WHERE Id = ?", new object[] { id });
        bool existe = false; await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
        await _db.Execute("DELETE FROM Matriculas WHERE Id = ?", new object[] { id });
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
    private readonly ILibSqlClient _db;
    public AtividadeService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<AtividadeDto>> ListarAsync()
    {
        var rs = await _db.Execute("SELECT Id, Titulo, Descricao, Prazo, TurmaId FROM Atividades");
        var list = new List<AtividadeDto>();
        await foreach (var row in rs.Rows)
            list.Add(new AtividadeDto((int)(long)row[0], (string)row[1], (string)row[2],
                DateTime.Parse((string)row[3]), (int)(long)row[4]));
        return list;
    }

    public async Task<IEnumerable<AtividadeDto>> ListarPorTurmaAsync(int turmaId)
    {
        var rs = await _db.Execute(
            "SELECT Id, Titulo, Descricao, Prazo, TurmaId FROM Atividades WHERE TurmaId = ?",
            new object[] { turmaId });
        var list = new List<AtividadeDto>();
        await foreach (var row in rs.Rows)
            list.Add(new AtividadeDto((int)(long)row[0], (string)row[1], (string)row[2],
                DateTime.Parse((string)row[3]), (int)(long)row[4]));
        return list;
    }

    public async Task<AtividadeDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT Id, Titulo, Descricao, Prazo, TurmaId FROM Atividades WHERE Id = ?",
            new object[] { id });
        await foreach (var row in rs.Rows)
            return new AtividadeDto((int)(long)row[0], (string)row[1], (string)row[2],
                DateTime.Parse((string)row[3]), (int)(long)row[4]);
        return null;
    }

    public async Task<AtividadeDto> CriarAsync(AtividadeCreateDto dto)
    {
        await _db.Execute(
            "INSERT INTO Atividades (Titulo, Descricao, Prazo, TurmaId) VALUES (?, ?, ?, ?)",
            new object[] { dto.Titulo, dto.Descricao, dto.Prazo.ToString("o"), dto.TurmaId });
        var rs = await _db.Execute("SELECT last_insert_rowid()");
        long id = 0; await foreach (var row in rs.Rows) id = (long)row[0];
        return new AtividadeDto((int)id, dto.Titulo, dto.Descricao, dto.Prazo, dto.TurmaId);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Atividades WHERE Id = ?", new object[] { id });
        bool existe = false; await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
        await _db.Execute("DELETE FROM Atividades WHERE Id = ?", new object[] { id });
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
    private readonly ILibSqlClient _db;
    public EntregaService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<EntregaDto>> ListarAsync()
    {
        var rs = await _db.Execute("SELECT Id, DataEntrega, Nota, AlunoId, AtividadeId FROM Entregas");
        var list = new List<EntregaDto>();
        await foreach (var row in rs.Rows)
            list.Add(new EntregaDto((int)(long)row[0], DateTime.Parse((string)row[1]),
                row[2] is null ? null : (decimal)(double)row[2], (int)(long)row[3], (int)(long)row[4]));
        return list;
    }

    public async Task<IEnumerable<EntregaDto>> ListarPorAtividadeAsync(int atividadeId)
    {
        var rs = await _db.Execute(
            "SELECT Id, DataEntrega, Nota, AlunoId, AtividadeId FROM Entregas WHERE AtividadeId = ?",
            new object[] { atividadeId });
        var list = new List<EntregaDto>();
        await foreach (var row in rs.Rows)
            list.Add(new EntregaDto((int)(long)row[0], DateTime.Parse((string)row[1]),
                row[2] is null ? null : (decimal)(double)row[2], (int)(long)row[3], (int)(long)row[4]));
        return list;
    }

    public async Task<EntregaDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT Id, DataEntrega, Nota, AlunoId, AtividadeId FROM Entregas WHERE Id = ?",
            new object[] { id });
        await foreach (var row in rs.Rows)
            return new EntregaDto((int)(long)row[0], DateTime.Parse((string)row[1]),
                row[2] is null ? null : (decimal)(double)row[2], (int)(long)row[3], (int)(long)row[4]);
        return null;
    }

    public async Task<EntregaDto> CriarAsync(EntregaCreateDto dto)
    {
        var data = DateTime.UtcNow.ToString("o");
        await _db.Execute(
            "INSERT INTO Entregas (DataEntrega, AlunoId, AtividadeId) VALUES (?, ?, ?)",
            new object[] { data, dto.AlunoId, dto.AtividadeId });
        var rs = await _db.Execute("SELECT last_insert_rowid()");
        long id = 0; await foreach (var row in rs.Rows) id = (long)row[0];
        return new EntregaDto((int)id, DateTime.UtcNow, null, dto.AlunoId, dto.AtividadeId);
    }

    public async Task<bool> LancarNotaAsync(int id, decimal nota)
    {
        var rs = await _db.Execute("SELECT Id FROM Entregas WHERE Id = ?", new object[] { id });
        bool existe = false; await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
        await _db.Execute("UPDATE Entregas SET Nota = ? WHERE Id = ?", new object[] { (double)nota, id });
        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Entregas WHERE Id = ?", new object[] { id });
        bool existe = false; await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
        await _db.Execute("DELETE FROM Entregas WHERE Id = ?", new object[] { id });
        return true;
    }
}