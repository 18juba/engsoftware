using EscolaApi.Data;
using EscolaApi.DTOs;
using Libsql.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    private readonly IDatabaseClient _db;

    public AlunoService(TursoClient turso)
    {
        _db = turso.Client;
    }

    public async Task<IEnumerable<AlunoDto>> ListarAsync()
    {
        var rs = await _db.Execute(
            "SELECT a.Id, u.Nome, u.Email, a.Matricula, a.Curso " +
            "FROM Alunos a JOIN Usuarios u ON a.UsuarioId = u.Id");

        var list = new List<AlunoDto>();

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var id = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : string.Empty;
            var email = r[2] is Text emailVal ? emailVal.Value : string.Empty;
            var matricula = r[3] is Text matVal ? matVal.Value : string.Empty;
            var curso = r[4] is Text cursoVal ? cursoVal.Value : string.Empty;

            list.Add(new AlunoDto(id, nome, email, matricula, curso));
        }

        return list;
    }

    public async Task<AlunoDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT a.Id, u.Nome, u.Email, a.Matricula, a.Curso " +
            "FROM Alunos a JOIN Usuarios u ON a.UsuarioId = u.Id WHERE a.Id = ?",
            new object[] { id });

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();

            var alunoId = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : string.Empty;
            var email = r[2] is Text emailVal ? emailVal.Value : string.Empty;
            var matricula = r[3] is Text matVal ? matVal.Value : string.Empty;
            var curso = r[4] is Text cursoVal ? cursoVal.Value : string.Empty;

            return new AlunoDto(alunoId, nome, email, matricula, curso);
        }

        return null;
    }

    public async Task<AlunoDto> CriarAsync(AlunoCreateDto dto)
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

        // Insere usuário
        await _db.Execute(
            "INSERT INTO Usuarios (Nome, Email, Senha, Tipo) VALUES (?, ?, ?, 'Aluno')",
            new object[] { dto.Nome, dto.Email, senhaHash });

        // Pega ID do usuário
        var rsUsuario = await _db.Execute("SELECT last_insert_rowid()");
        long usuarioId = 0;
        foreach (var row in rsUsuario.Rows)
        {
            var r = row.ToArray();
            usuarioId = r[0] is Integer idVal ? idVal.Value : 0;
        }

        // Insere aluno
        await _db.Execute(
            "INSERT INTO Alunos (Matricula, Curso, UsuarioId) VALUES (?, ?, ?)",
            new object[] { dto.Matricula, dto.Curso, usuarioId });

        // Pega ID do aluno
        var rsAluno = await _db.Execute("SELECT last_insert_rowid()");
        long alunoId = 0;
        foreach (var row in rsAluno.Rows)
        {
            var r = row.ToArray();
            alunoId = r[0] is Integer idVal ? idVal.Value : 0;
        }

        return new AlunoDto((int)alunoId, dto.Nome, dto.Email, dto.Matricula, dto.Curso);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT Id FROM Alunos WHERE Id = ?",
            new object[] { id });

        if (!rs.Rows.Any())
            return false;

        await _db.Execute("DELETE FROM Alunos WHERE Id = ?", new object[] { id });
        return true;
    }
}