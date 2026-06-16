using EscolaApi.Data;
using EscolaApi.DTOs;
using LibSql.Client;

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
    private readonly ILibSqlClient _db;

    public AlunoService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<AlunoDto>> ListarAsync()
    {
        var rs = await _db.Execute(
            "SELECT a.Id, u.Nome, u.Email, a.Matricula, a.Curso " +
            "FROM Alunos a JOIN Usuarios u ON a.UsuarioId = u.Id");
        var list = new List<AlunoDto>();
        await foreach (var row in rs.Rows)
            list.Add(new AlunoDto(
                (int)(long)row[0], (string)row[1], (string)row[2],
                (string)row[3], (string)row[4]));
        return list;
    }

    public async Task<AlunoDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT a.Id, u.Nome, u.Email, a.Matricula, a.Curso " +
            "FROM Alunos a JOIN Usuarios u ON a.UsuarioId = u.Id WHERE a.Id = ?",
            new object[] { id });
        await foreach (var row in rs.Rows)
            return new AlunoDto(
                (int)(long)row[0], (string)row[1], (string)row[2],
                (string)row[3], (string)row[4]);
        return null;
    }

    public async Task<AlunoDto> CriarAsync(AlunoCreateDto dto)
    {
        var senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        await _db.Execute(
            "INSERT INTO Usuarios (Nome, Email, Senha, Tipo) VALUES (?, ?, ?, 'Aluno')",
            new object[] { dto.Nome, dto.Email, senha });

        var rsId = await _db.Execute("SELECT last_insert_rowid()");
        long usuarioId = 0;
        await foreach (var row in rsId.Rows) usuarioId = (long)row[0];

        await _db.Execute(
            "INSERT INTO Alunos (Matricula, Curso, UsuarioId) VALUES (?, ?, ?)",
            new object[] { dto.Matricula, dto.Curso, usuarioId });

        var rsAlunoId = await _db.Execute("SELECT last_insert_rowid()");
        long alunoId = 0;
        await foreach (var row in rsAlunoId.Rows) alunoId = (long)row[0];

        return new AlunoDto((int)alunoId, dto.Nome, dto.Email, dto.Matricula, dto.Curso);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Alunos WHERE Id = ?", new object[] { id });
        bool existe = false;
        await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
        await _db.Execute("DELETE FROM Alunos WHERE Id = ?", new object[] { id });
        return true;
    }
}