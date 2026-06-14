namespace EscolaApi.Models;

public enum StatusMatricula
{
    Ativa,
    Trancada,
    Concluida,
    Cancelada
}

public class Matricula
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public StatusMatricula Status { get; set; }

    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;

    public int TurmaId { get; set; }
    public Turma Turma { get; set; } = null!;
}
