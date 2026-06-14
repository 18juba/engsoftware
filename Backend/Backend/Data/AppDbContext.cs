using EscolaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Professor> Professores => Set<Professor>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<ProfessorDisciplina> ProfessorDisciplinas => Set<ProfessorDisciplina>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<Atividade> Atividades => Set<Atividade>();
    public DbSet<Entrega> Entregas => Set<Entrega>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuario
        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Nome).IsRequired().HasMaxLength(150);
            e.Property(u => u.Email).IsRequired().HasMaxLength(200);
            e.Property(u => u.Senha).IsRequired();
            e.Property(u => u.Tipo).HasConversion<string>();
        });

        // Aluno (herança via tabela separada - Table Per Type)
        modelBuilder.Entity<Aluno>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasIndex(a => a.Matricula).IsUnique();
            e.Property(a => a.Matricula).IsRequired().HasMaxLength(20);
            e.Property(a => a.Curso).IsRequired().HasMaxLength(150);
            e.HasOne(a => a.Usuario)
             .WithOne(u => u.Aluno)
             .HasForeignKey<Aluno>(a => a.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Professor
        modelBuilder.Entity<Professor>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Siape).IsUnique();
            e.Property(p => p.Siape).IsRequired().HasMaxLength(20);
            e.Property(p => p.Departamento).IsRequired().HasMaxLength(150);
            e.HasOne(p => p.Usuario)
             .WithOne(u => u.Professor)
             .HasForeignKey<Professor>(p => p.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Disciplina
        modelBuilder.Entity<Disciplina>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasIndex(d => d.Codigo).IsUnique();
            e.Property(d => d.Nome).IsRequired().HasMaxLength(200);
            e.Property(d => d.Codigo).IsRequired().HasMaxLength(20);
        });

        // ProfessorDisciplina (N:N)
        modelBuilder.Entity<ProfessorDisciplina>(e =>
        {
            e.HasKey(pd => new { pd.ProfessorId, pd.DisciplinaId });
            e.HasOne(pd => pd.Professor)
             .WithMany(p => p.ProfessorDisciplinas)
             .HasForeignKey(pd => pd.ProfessorId);
            e.HasOne(pd => pd.Disciplina)
             .WithMany(d => d.ProfessorDisciplinas)
             .HasForeignKey(pd => pd.DisciplinaId);
        });

        // Turma
        modelBuilder.Entity<Turma>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Semestre).IsRequired().HasMaxLength(10);
            e.Property(t => t.Horario).IsRequired().HasMaxLength(100);
            e.HasOne(t => t.Disciplina)
             .WithMany(d => d.Turmas)
             .HasForeignKey(t => t.DisciplinaId);
        });

        // Matricula
        modelBuilder.Entity<Matricula>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Status).HasConversion<string>();
            e.HasOne(m => m.Aluno)
             .WithMany(a => a.Matriculas)
             .HasForeignKey(m => m.AlunoId);
            e.HasOne(m => m.Turma)
             .WithMany(t => t.Matriculas)
             .HasForeignKey(m => m.TurmaId);
        });

        // Atividade
        modelBuilder.Entity<Atividade>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Titulo).IsRequired().HasMaxLength(200);
            e.HasOne(a => a.Turma)
             .WithMany(t => t.Atividades)
             .HasForeignKey(a => a.TurmaId);
        });

        // Entrega
        modelBuilder.Entity<Entrega>(e =>
        {
            e.HasKey(en => en.Id);
            e.Property(en => en.Nota).HasPrecision(5, 2);
            e.HasOne(en => en.Aluno)
             .WithMany(a => a.Entregas)
             .HasForeignKey(en => en.AlunoId);
            e.HasOne(en => en.Atividade)
             .WithMany(a => a.Entregas)
             .HasForeignKey(en => en.AtividadeId);
        });
    }
}
