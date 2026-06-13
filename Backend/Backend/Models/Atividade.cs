namespace EscolaApi.Models;

public class Atividade
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime Prazo { get; set; }

    public int TurmaId { get; set; }
    public Turma Turma { get; set; } = null!;

    public ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}
