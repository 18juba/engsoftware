namespace EscolaApi.Models;

public class Turma
{
    public int Id { get; set; }
    public string Semestre { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;

    public int DisciplinaId { get; set; }
    public Disciplina Disciplina { get; set; } = null!;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    public ICollection<Atividade> Atividades { get; set; } = new List<Atividade>();
}
