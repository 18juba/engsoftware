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
    Task<ProfessorDto?> AtualizarAsync(int id, ProfessorUpdateDto dto);
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
        // Verifica se email já existe
        var rsEmail = await _db.Execute(
            "SELECT COUNT(*) FROM Usuarios WHERE Email = ?",
            new object[] { dto.Email });
        foreach (var row in rsEmail.Rows)
        {
            var r = row.ToArray();
            var count = r[0] is Integer c ? (int)c.Value : 0;
            if (count > 0)
                throw new InvalidOperationException($"Email '{dto.Email}' já está em uso.");
        }

        // Verifica se SIAPE já existe
        var rsSiape = await _db.Execute(
            "SELECT COUNT(*) FROM Professores WHERE Siape = ?",
            new object[] { dto.Siape });
        foreach (var row in rsSiape.Rows)
        {
            var r = row.ToArray();
            var count = r[0] is Integer c ? (int)c.Value : 0;
            if (count > 0)
                throw new InvalidOperationException($"SIAPE '{dto.Siape}' já está em uso.");
        }

        var senhaHash = "12345678";
        if (!String.IsNullOrEmpty(dto.Senha))
        {
            senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        }

        // Insere usuário
        await _db.Execute(
            "INSERT INTO Usuarios (Nome, Email, Senha, Tipo) VALUES (?, ?, ?, 'Professor')",
            new object[] { dto.Nome, dto.Email, senhaHash });

        // Pega ID do usuário pelo email
        var rsUsuario = await _db.Execute(
            "SELECT Id FROM Usuarios WHERE Email = ?",
            new object[] { dto.Email });
        int usuarioId = 0;
        foreach (var row in rsUsuario.Rows)
        {
            var r = row.ToArray();
            usuarioId = r[0] is Integer idVal ? (int)idVal.Value : 0;
        }

        // Insere professor
        await _db.Execute(
            "INSERT INTO Professores (Siape, Departamento, UsuarioId) VALUES (?, ?, ?)",
            new object[] { dto.Siape, dto.Departamento, usuarioId });

        // Pega ID do professor pelo SIAPE
        var rsProfessor = await _db.Execute(
            "SELECT Id FROM Professores WHERE Siape = ?",
            new object[] { dto.Siape });
        int professorId = 0;
        foreach (var row in rsProfessor.Rows)
        {
            var r = row.ToArray();
            professorId = r[0] is Integer idVal2 ? (int)idVal2.Value : 0;
        }

        return new ProfessorDto(professorId, dto.Nome, dto.Email, dto.Siape, dto.Departamento);
    }

    public async Task<ProfessorDto?> AtualizarAsync(int id, ProfessorUpdateDto dto)
    {
        // Verifica se o professor existe e obtém o UsuarioId vinculado
        var rsProfessor = await _db.Execute(
            "SELECT UsuarioId FROM Professores WHERE Id = ?",
            new object[] { id });

        int usuarioId = 0;
        foreach (var row in rsProfessor.Rows)
        {
            var r = row.ToArray();
            usuarioId = r[0] is Integer uId ? (int)uId.Value : 0;
        }
        if (usuarioId == 0) return null;

        // Se o SIAPE foi informado, verifica conflito com outro professor
        if (!string.IsNullOrWhiteSpace(dto.Siape))
        {
            var rsSiape = await _db.Execute(
                "SELECT Id FROM Professores WHERE Siape = ? AND Id != ?",
                new object[] { dto.Siape, id });
            if (rsSiape.Rows.Any())
                throw new InvalidOperationException($"SIAPE '{dto.Siape}' já está em uso.");
        }

        // Se o email foi informado, verifica conflito com outro usuário
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var rsEmail = await _db.Execute(
                "SELECT Id FROM Usuarios WHERE Email = ? AND Id != ?",
                new object[] { dto.Email, usuarioId });
            if (rsEmail.Rows.Any())
                throw new InvalidOperationException($"Email '{dto.Email}' já está em uso.");
        }

        // Atualiza Usuarios (Nome e Email pertencem à tabela pai)
        await _db.Execute(@"
            UPDATE Usuarios
            SET Nome  = COALESCE(?, Nome),
                Email = COALESCE(?, Email)
            WHERE Id = ?",
            new object[] { dto.Nome!, dto.Email!, usuarioId });

        // Atualiza Professores (Siape e Departamento pertencem à tabela filha)
        await _db.Execute(@"
            UPDATE Professores
            SET Siape        = COALESCE(?, Siape),
                Departamento = COALESCE(?, Departamento)
            WHERE Id = ?",
            new object[] { dto.Siape!, dto.Departamento!, id });

        // Retorna o registro atualizado
        return await ObterPorIdAsync(id);
    }

    public async Task<bool> VincularDisciplinaAsync(int professorId, int disciplinaId)
    {
        int pId = (int)professorId;
        int dId = (int)disciplinaId;

        var rsP = await _db.Execute(
            "SELECT Id FROM Professores WHERE Id = ?",
            new object[] { pId });
        if (!rsP.Rows.Any()) return false;

        var rsD = await _db.Execute(
            "SELECT Id FROM Disciplinas WHERE Id = ?",
            new object[] { dId });
        if (!rsD.Rows.Any()) return false;

        var rsV = await _db.Execute(
            "SELECT ProfessorId FROM ProfessorDisciplinas WHERE ProfessorId = ? AND DisciplinaId = ?",
            new object[] { pId, dId });
        if (rsV.Rows.Any()) return false;

        await _db.Execute(
            "INSERT INTO ProfessorDisciplinas (ProfessorId, DisciplinaId) VALUES (?, ?)",
            new object[] { pId, dId });

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