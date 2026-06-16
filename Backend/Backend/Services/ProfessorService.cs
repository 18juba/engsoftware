using EscolaApi.Data;
using EscolaApi.DTOs;
using Libsql.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    private readonly IDatabaseClient _db;

    public ProfessorService(TursoClient turso)
    {
        _db = turso.Client;
    }

    public async Task<IEnumerable<ProfessorDto>> ListarAsync()
    {
        var rs = await _db.Execute(
            "SELECT p.Id, u.Nome, u.Email, p.Siape, p.Departamento " +
            "FROM Professores p JOIN Usuarios u ON p.UsuarioId = u.Id");

        var list = new List<ProfessorDto>();

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var id = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : string.Empty;
            var email = r[2] is Text emailVal ? emailVal.Value : string.Empty;
            var siape = r[3] is Text siapeVal ? siapeVal.Value : string.Empty;
            var departamento = r[4] is Text depVal ? depVal.Value : string.Empty;

            list.Add(new ProfessorDto(id, nome, email, siape, departamento));
        }

        return list;
    }

    public async Task<ProfessorDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT p.Id, u.Nome, u.Email, p.Siape, p.Departamento " +
            "FROM Professores p JOIN Usuarios u ON p.UsuarioId = u.Id WHERE p.Id = ?",
            new object[] { id });

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var professorId = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : string.Empty;
            var email = r[2] is Text emailVal ? emailVal.Value : string.Empty;
            var siape = r[3] is Text siapeVal ? siapeVal.Value : string.Empty;
            var departamento = r[4] is Text depVal ? depVal.Value : string.Empty;

            return new ProfessorDto(professorId, nome, email, siape, departamento);
        }

        return null;
    }

    public async Task<ProfessorDto> CriarAsync(ProfessorCreateDto dto)
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

        // Insere Usuário
        await _db.Execute(
            "INSERT INTO Usuarios (Nome, Email, Senha, Tipo) VALUES (?, ?, ?, 'Professor')",
            new object[] { dto.Nome, dto.Email, senhaHash });

        // Pega ID do usuário
        var rsUsuario = await _db.Execute("SELECT last_insert_rowid()");
        long usuarioId = 0;
        foreach (var row in rsUsuario.Rows)
        {
            var r = row.ToArray();
            usuarioId = r[0] is Integer idVal ? idVal.Value : 0;
        }

        // Insere Professor
        await _db.Execute(
            "INSERT INTO Professores (Siape, Departamento, UsuarioId) VALUES (?, ?, ?)",
            new object[] { dto.Siape, dto.Departamento, usuarioId });

        // Pega ID do professor
        var rsProfessor = await _db.Execute("SELECT last_insert_rowid()");
        long professorId = 0;
        foreach (var row in rsProfessor.Rows)
        {
            var r = row.ToArray();
            professorId = r[0] is Integer idVal ? idVal.Value : 0;
        }

        return new ProfessorDto((int)professorId, dto.Nome, dto.Email, dto.Siape, dto.Departamento);
    }

    public async Task<bool> VincularDisciplinaAsync(int professorId, int disciplinaId)
    {
        // Verifica se professor existe
        var rsP = await _db.Execute("SELECT Id FROM Professores WHERE Id = ?", new object[] { professorId });
        if (!rsP.Rows.Any()) return false;

        // Verifica se disciplina existe
        var rsD = await _db.Execute("SELECT Id FROM Disciplinas WHERE Id = ?", new object[] { disciplinaId });
        if (!rsD.Rows.Any()) return false;

        // Verifica se vínculo já existe
        var rsV = await _db.Execute(
            "SELECT ProfessorId FROM ProfessorDisciplinas WHERE ProfessorId = ? AND DisciplinaId = ?",
            new object[] { professorId, disciplinaId });

        if (rsV.Rows.Any()) return false;

        // Cria o vínculo
        await _db.Execute(
            "INSERT INTO ProfessorDisciplinas (ProfessorId, DisciplinaId) VALUES (?, ?)",
            new object[] { professorId, disciplinaId });

        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Professores WHERE Id = ?", new object[] { id });
        if (!rs.Rows.Any()) return false;

        await _db.Execute("DELETE FROM Professores WHERE Id = ?", new object[] { id });
        return true;
    }
}