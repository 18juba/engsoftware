using EscolaApi.Data;
using EscolaApi.DTOs;
using Libsql.Client;

namespace EscolaApi.Services;

public interface IAlunoService
{
    Task<IEnumerable<AlunoDto>> ListarAsync();
    Task<AlunoDto?> ObterPorIdAsync(int id);
    Task<AlunoDto> CriarAsync(AlunoCreateDto dto);
    Task<AlunoDto?> AtualizarAsync(int id, AlunoUpdateDto dto);
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

        // Verifica se matrícula já existe
        var rsMatricula = await _db.Execute(
            "SELECT COUNT(*) FROM Alunos WHERE Matricula = ?",
            new object[] { dto.Matricula });
        foreach (var row in rsMatricula.Rows)
        {
            var r = row.ToArray();
            var count = r[0] is Integer c ? (int)c.Value : 0;
            if (count > 0)
                throw new InvalidOperationException($"Matrícula '{dto.Matricula}' já está em uso.");
        }

        var senhaHash = "12345678";
        if (!String.IsNullOrEmpty(dto.Senha))
        {
            senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        }
            

        // Insere usuário
        await _db.Execute(
            "INSERT INTO Usuarios (Nome, Email, Senha, Tipo) VALUES (?, ?, ?, 'Aluno')",
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

        // Insere aluno
        await _db.Execute(
            "INSERT INTO Alunos (Matricula, Curso, UsuarioId) VALUES (?, ?, ?)",
            new object[] { dto.Matricula, dto.Curso, usuarioId });

        // Pega ID do aluno pela matrícula (evita last_insert_rowid race condition)
        var rsAluno = await _db.Execute(
            "SELECT Id FROM Alunos WHERE Matricula = ?",
            new object[] { dto.Matricula });
        int alunoId = 0;
        foreach (var row in rsAluno.Rows)
        {
            var r = row.ToArray();
            alunoId = r[0] is Integer idVal2 ? (int)idVal2.Value : 0;
        }

        // Busca o aluno recém criado para confirmar persistência
        var rsConfirma = await _db.Execute(
            "SELECT a.Id, u.Nome, u.Email, a.Matricula, a.Curso " +
            "FROM Alunos a JOIN Usuarios u ON a.UsuarioId = u.Id " +
            "WHERE a.Id = ?",
            new object[] { alunoId });

        foreach (var row in rsConfirma.Rows)
        {
            var r = row.ToArray();
            return new AlunoDto(
                Id: r[0] is Integer id ? (int)id.Value : alunoId,
                Nome: r[1] is Text nome ? nome.Value : dto.Nome,
                Email: r[2] is Text email ? email.Value : dto.Email,
                Matricula: r[3] is Text mat ? mat.Value : dto.Matricula,
                Curso: r[4] is Text curso ? curso.Value : dto.Curso
            );
        }

        return new AlunoDto(alunoId, dto.Nome, dto.Email, dto.Matricula, dto.Curso);
    }

    public async Task<AlunoDto?> AtualizarAsync(int id, AlunoUpdateDto dto)
    {
        // Verifica se o aluno existe e obtém o UsuarioId vinculado
        var rsAluno = await _db.Execute(
            "SELECT UsuarioId FROM Alunos WHERE Id = ?",
            new object[] { id });

        int usuarioId = 0;
        foreach (var row in rsAluno.Rows)
        {
            var r = row.ToArray();
            usuarioId = r[0] is Integer uId ? (int)uId.Value : 0;
        }
        if (usuarioId == 0) return null;

        // Se a matrícula foi informada, verifica conflito com outro aluno
        if (!string.IsNullOrWhiteSpace(dto.Matricula))
        {
            var rsMat = await _db.Execute(
                "SELECT Id FROM Alunos WHERE Matricula = ? AND Id != ?",
                new object[] { dto.Matricula, id });
            if (rsMat.Rows.Any())
                throw new InvalidOperationException($"Matrícula '{dto.Matricula}' já está em uso.");
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

        // Atualiza Alunos (Matricula e Curso pertencem à tabela filha)
        await _db.Execute(@"
            UPDATE Alunos
            SET Matricula = COALESCE(?, Matricula),
                Curso     = COALESCE(?, Curso)
            WHERE Id = ?",
            new object[] { dto.Matricula!, dto.Curso!, id });

        // Retorna o registro atualizado
        return await ObterPorIdAsync(id);
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