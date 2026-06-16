using EscolaApi.Data;
using EscolaApi.DTOs;
using EscolaApi.Models;
using LibSql.Client;

namespace EscolaApi.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ListarAsync();
    Task<UsuarioDto?> ObterPorIdAsync(int id);
    Task<UsuarioDto> CriarAsync(UsuarioCreateDto dto);
    Task<bool> DeletarAsync(int id);
}

public class UsuarioService : IUsuarioService
{
    private readonly ILibSqlClient _db;

    public UsuarioService(TursoClient turso) => _db = turso.Client;

    public async Task<IEnumerable<UsuarioDto>> ListarAsync()
    {
        var rs = await _db.Execute("SELECT Id, Nome, Email, Tipo FROM Usuarios");
        var list = new List<UsuarioDto>();
        await foreach (var row in rs.Rows)
            list.Add(new UsuarioDto(
                (int)(long)row[0], (string)row[1], (string)row[2],
                Enum.Parse<TipoUsuario>((string)row[3])));
        return list;
    }

    public async Task<UsuarioDto?> ObterPorIdAsync(int id)
    {
        var rs = await _db.Execute(
            "SELECT Id, Nome, Email, Tipo FROM Usuarios WHERE Id = ?",
            new object[] { id });
        await foreach (var row in rs.Rows)
            return new UsuarioDto(
                (int)(long)row[0], (string)row[1], (string)row[2],
                Enum.Parse<TipoUsuario>((string)row[3]));
        return null;
    }

    public async Task<UsuarioDto> CriarAsync(UsuarioCreateDto dto)
    {
        var senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        await _db.Execute(
            "INSERT INTO Usuarios (Nome, Email, Senha, Tipo) VALUES (?, ?, ?, ?)",
            new object[] { dto.Nome, dto.Email, senha, dto.Tipo.ToString() });
        var rs = await _db.Execute("SELECT last_insert_rowid()");
        long id = 0;
        await foreach (var row in rs.Rows) id = (long)row[0];
        return new UsuarioDto((int)id, dto.Nome, dto.Email, dto.Tipo);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var rs = await _db.Execute("SELECT Id FROM Usuarios WHERE Id = ?", new object[] { id });
        bool existe = false;
        await foreach (var _ in rs.Rows) existe = true;
        if (!existe) return false;
        await _db.Execute("DELETE FROM Usuarios WHERE Id = ?", new object[] { id });
        return true;
    }
}