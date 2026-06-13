namespace EscolaApi.Models;

public class Entrega
{
    public int Id { get; set; }
    public DateTime DataEntrega { get; set; }
    public decimal? Nota { get; set; }

    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;

    public int AtividadeId { get; set; }
    public Atividade Atividade { get; set; } = null!;
}
