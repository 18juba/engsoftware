using EscolaApi.Data;
using EscolaApi.DTOs;
using LibSql.Client;

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
    private readonly ILibSqlClient _db;

    public ProfessorService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<ProfessorDto>> ListarAsync()
    {
        var rs = await _db.Execute(
            "SELECT p.Id, u.Nome, u.Email, p.Siape, p.Departamento " +
            "FROM Professores p JOIN Usuarios u ON p.UsuarioId = u.Id");
        var list = new List<ProfessorDto>();
        await foreach (var row in rs.Rows)
            list.Add(new ProfessorDto(
                (int)(long)row[0], (string)row[1], (string)row[2],
                (string)row[3], (string)row[4]));
        return list;
    }

    public async Task<ProfessorDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT p.Id, u.Nome, u.Email, p.Siape, p.Departamento " +
            "FROM Professores p JOIN Usuarios u ON p.UsuarioId = u.Id WHERE p.Id = ?",
            new object[] { id });
        await foreach (var row in rs.Rows)
            return new ProfessorDto(
                (int)(long)row[0], (string)row[1], (string)row[2],
                (string)row[3], (string)row[4]);
        return null;
    }

    public async Task<ProfessorDto> CriarAsync(ProfessorCreateDto dto)
    {
        var senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        await _db.Execute(
            "INSERT INTO Usuarios (Nome, Email, Senha, Tipo) VALUES (?, ?, ?, 'Professor')",
            new object[] { dto.Nome, dto.Email, senha });
        var rsUid = await _db.Execute("SELECT last_insert_rowid()");
        long uid = 0;
        await foreach (var row in rsUid.Rows) uid = (long)row[0];

        await _db.Execute(
            "INSERT INTO Professores (Siape, Departamento, UsuarioId) VALUES (?, ?, ?)",
            new object[] { dto.Siape, dto.Departamento, uid });
        var rsPid = await _db.Execute("SELECT last_insert_rowid()");
        long pid = 0;
        await foreach (var row in rsPid.Rows) pid = (long)row[0];

        return new ProfessorDto((int)pid, dto.Nome, dto.Email, dto.Siape, dto.Departamento);
    }

    public async Task<bool> VincularDisciplinaAsync(int professorId, int disciplinaId)
    {
        var rsP = await _db.Execute("SELECT Id FROM Professores WHERE Id = ?", new object[] { professorId });
        bool p = false; await foreach (var _ in rsP.Rows) p = true;
        if (!p) return false;

        var rsD = await _db.Execute("SELECT Id FROM Disciplinas WHERE Id = ?", new object[] { disciplinaId });
        bool d = false; await foreach (var _ in rsD.Rows) d = true;
        if (!d) return false;

        var rsV = await _db.Execute(
            "SELECT ProfessorId FROM ProfessorDisciplinas WHERE ProfessorId = ? AND DisciplinaId = ?",
            new object[] { professorId, disciplinaId });
        bool v = false; await foreach (var _ in rsV.Rows) v = true;
        if (v) return false;

        await _db.Execute(
            "INSERT INTO ProfessorDisciplinas (ProfessorId, DisciplinaId) VALUES (?, ?)",
            new object[] { professorId, disciplinaId });
        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Professores WHERE Id = ?", new object[] { id });
        bool existe = false; await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
        await _db.Execute("DELETE FROM Professores WHERE Id = ?", new object[] { id });
        return true;
    }
}