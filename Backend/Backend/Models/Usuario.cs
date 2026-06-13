namespace EscolaApi.Models;

public enum TipoUsuario
{
    Aluno,
    Professor,
    Admin
}

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public TipoUsuario Tipo { get; set; }

    // Navegação
    public Aluno? Aluno { get; set; }
    public Professor? Professor { get; set; }
}
