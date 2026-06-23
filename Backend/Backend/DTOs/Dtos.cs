using EscolaApi.Models;

namespace EscolaApi.DTOs;

// ── Auth ────────────────────────────────────────────────────
public record LoginRequest(string Email, string Senha);
public record LoginResponse(string Token, string Nome, string Tipo);

// ── Usuario ─────────────────────────────────────────────────
public record UsuarioCreateDto(
    string Nome,
    string Email,
    string Senha,
    TipoUsuario Tipo);

public record UsuarioUpdateDto(
    string? Nome,
    string? Email,
    TipoUsuario? Tipo   // EscolaApi.Models.TipoUsuario
);

public record UsuarioDto(int Id, string Nome, string Email, TipoUsuario Tipo);

// ── Aluno ────────────────────────────────────────────────────
public record AlunoCreateDto(
    string Nome,
    string Email,
    string Senha,
    string Matricula,
    string Curso);

public record AlunoUpdateDto(
        string? Nome,
        string? Email,
        string? Matricula,
        string? Curso
    );

public record AlunoDto(int Id, string Nome, string Email, string Matricula, string Curso);

// ── Professor ────────────────────────────────────────────────
public record ProfessorCreateDto(
    string Nome,
    string Email,
    string Senha,
    string Siape,
    string Departamento);

public record ProfessorUpdateDto(
    string? Nome,
    string? Email,
    string? Siape,
    string? Departamento
);

public record ProfessorDto(int Id, string Nome, string Email, string Siape, string Departamento);

// ── Disciplina ───────────────────────────────────────────────

public record DisciplinaUpdateDto(
    string? Nome,
    string? Codigo,
    int CargaHoraria
);

public record DisciplinaCreateDto(string Nome, string Codigo, int CargaHoraria);
public record DisciplinaDto(int Id, string Nome, string Codigo, int CargaHoraria);

// ── Turma ────────────────────────────────────────────────────
public record TurmaUpdateDto(
    string? Semestre,
    string? Horario,
    int? DisciplinaId
);
public record TurmaCreateDto(string Semestre, string Horario, int DisciplinaId);
public record TurmaDto(int Id, string Semestre, string Horario, DisciplinaDto Disciplina);

// ── Matricula ────────────────────────────────────────────────
public record MatriculaCreateDto(int AlunoId, int TurmaId);
public record MatriculaDto(int Id, DateTime Data, string Status, AlunoDto Aluno, int TurmaId);

// ── Atividade ────────────────────────────────────────────────
public record AtividadeCreateDto(string Titulo, string Descricao, DateTime Prazo, int TurmaId);
public record AtividadeDto(int Id, string Titulo, string Descricao, DateTime Prazo, int TurmaId);

// ── Entrega ──────────────────────────────────────────────────
public record EntregaCreateDto(int AlunoId, int AtividadeId);
public record EntregaNotaDto(decimal Nota);
public record EntregaDto(int Id, DateTime DataEntrega, decimal? Nota, int AlunoId, int AtividadeId);
