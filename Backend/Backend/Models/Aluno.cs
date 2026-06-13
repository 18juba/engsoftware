namespace EscolaApi.Models;

public class Aluno
{
    public int Id { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string Curso { get; set; } = string.Empty;

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    public ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}
