using EscolaApi.Data;
using EscolaApi.DTOs;
using EscolaApi.Models;
using Libsql.Client;
namespace EscolaApi.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ListarAsync();
    Task<UsuarioDto?> ObterPorIdAsync(int id);
    Task<UsuarioDto> CriarAsync(UsuarioCreateDto dto);
    Task<UsuarioDto?> AtualizarAsync(int id, UsuarioUpdateDto dto);
    Task<bool> DeletarAsync(int id);
}

public class UsuarioService : IUsuarioService
{
    private readonly IDatabaseClient _db;

    public UsuarioService(TursoClient turso)
    {
        _db = turso.Client;
    }

    public async Task<IEnumerable<UsuarioDto>> ListarAsync()
    {
        var rs = await _db.Execute("SELECT Id, Nome, Email, Tipo FROM Usuarios");
        var list = new List<UsuarioDto>();

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();
            var id = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : string.Empty;
            var email = r[2] is Text emailVal ? emailVal.Value : string.Empty;
            var tipo = r[3] is Integer tipoVal ? tipoVal.Value : 1;

            list.Add(new UsuarioDto(id, nome, email, (TipoUsuario)tipo));
        }
        return list;
    }

    public async Task<UsuarioDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id, Nome, Email, Tipo FROM Usuarios WHERE Id = ?",
            new object[] { id });

        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();
            var usuarioId = r[0] is Integer idVal ? (int)idVal.Value : 0;
            var nome = r[1] is Text nomeVal ? nomeVal.Value : string.Empty;
            var email = r[2] is Text emailVal ? emailVal.Value : string.Empty;
            var tipo = r[3] is Integer tipoVal ? tipoVal.Value : 1;

            return new UsuarioDto(usuarioId, nome, email, (TipoUsuario)tipo);
        }
        return null;
    }

    public async Task<UsuarioDto> CriarAsync(UsuarioCreateDto dto)
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

        await _db.Execute(
            "INSERT INTO Usuarios (Nome, Email, Senha, Tipo) VALUES (?, ?, ?, ?)",
            new object[] { dto.Nome, dto.Email, senhaHash, dto.Tipo.ToString() });

        var rs = await _db.Execute("SELECT last_insert_rowid()");
        long id = 0;
        foreach (var row in rs.Rows)
        {
            var r = row.ToArray();
            id = r[0] is Integer idVal ? idVal.Value : 0;
        }

        return new UsuarioDto((int)id, dto.Nome, dto.Email, dto.Tipo);
    }

    public async Task<UsuarioDto?> AtualizarAsync(int id, UsuarioUpdateDto dto)
    {
        // Verifica se o usuário existe
        var rsExiste = await _db.Execute(
            "SELECT Id FROM Usuarios WHERE Id = ?",
            new object[] { id });
        if (!rsExiste.Rows.Any()) return null;

        // Se o email foi informado, verifica conflito com outro usuário
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var rsEmail = await _db.Execute(
                "SELECT Id FROM Usuarios WHERE Email = ? AND Id != ?",
                new object[] { dto.Email, id });
            if (rsEmail.Rows.Any())
                throw new InvalidOperationException($"Email '{dto.Email}' já está em uso.");
        }

        // Aplica apenas os campos enviados (COALESCE mantém o valor atual se null)
        await _db.Execute(@"
            UPDATE Usuarios
            SET Nome  = COALESCE(?, Nome),
                Email = COALESCE(?, Email),
                Tipo  = COALESCE(?, Tipo)
            WHERE Id = ?",
            new object[] { dto.Nome!, dto.Email!, dto.Tipo?.ToString()!, id });

        // Retorna o registro atualizado
        return await ObterPorIdAsync(id);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Usuarios WHERE Id = ?", new object[] { id });
        if (!rs.Rows.Any()) return false;

        await _db.Execute("DELETE FROM Usuarios WHERE Id = ?", new object[] { id });
        return true;
    }
}