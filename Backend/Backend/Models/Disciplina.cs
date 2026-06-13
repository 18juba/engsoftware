namespace EscolaApi.Models;

public class Disciplina
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int CargaHoraria { get; set; }

    public ICollection<ProfessorDisciplina> ProfessorDisciplinas { get; set; } = new List<ProfessorDisciplina>();
    public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
}
