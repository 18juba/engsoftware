namespace EscolaApi.Models;

public class Professor
{
    public int Id { get; set; }
    public string Siape { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    // Professor ministra Disciplinas (N:N via tabela ProfessorDisciplina)
    public ICollection<ProfessorDisciplina> ProfessorDisciplinas { get; set; } = new List<ProfessorDisciplina>();
}
