using EscolaApi.Data;
using EscolaApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Database (Tratamento Anti-Erro de URI TCP) ────────────────────────
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    // Se rodar local pelo Visual Studio, pega do appsettings.Development.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// ── CORREÇÃO CIRÚRGICA DO ERRO ────────────────────────────────────────
// Se a string vier no formato do Docker antigo ou com prefixo 'tcp://'
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("tcp://"))
{
    // Limpa o 'tcp://' e extrai apenas o endereço puro
    connectionString = connectionString.Replace("tcp://", "");

    // Se contiver apenas o host:porta (ex: localhost:5432), reconstrói o formato correto do Postgres
    if (connectionString.Contains(":"))
    {
        var partes = connectionString.Split(':');
        var host = partes[0];
        // Remove possíveis caminhos adicionais se houver
        var porta = partes[1].Split('/')[0];

        // Reconstrói usando as credenciais padrão do seu .env
        connectionString = $"Host={host};Port={porta};Database=db;Username=postgres;Password=12345678;Pooling=true;";
    }
}
// ──────────────────────────────────────────────────────────────────────

Console.WriteLine($"[DATABASE] >>> Conectando via: {connectionString}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ── Services ─────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAlunoService, AlunoService>();
builder.Services.AddScoped<IProfessorService, ProfessorService>();
builder.Services.AddScoped<IDisciplinaService, DisciplinaService>();
builder.Services.AddScoped<ITurmaService, TurmaService>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<IAtividadeService, AtividadeService>();
builder.Services.AddScoped<IEntregaService, EntregaService>();

// ── JWT (Injeção via Docker ou AppSettings) ──────────────────
// Lê de Jwt__Key (Docker) ou do AppSettings se rodar local
var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key")
             ?? builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("JWT Key not configured.");

var jwtIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer") ?? builder.Configuration["Jwt:Issuer"];
var jwtAudience = Environment.GetEnvironmentVariable("Jwt__Audience") ?? builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ── Swagger ──────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Escola API",
        Version = "v1",
        Description = "API REST para sistema escolar com alunos, professores, disciplinas, turmas e atividades."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira seu token JWT diretamente no campo abaixo."
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc, null),
            new List<string>()
        }
    });
});

builder.Services.AddControllers();

// ── CORS ─────────────────────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Middleware ───────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Escola API v1"));
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── Auto-migrate on startup ───────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Executa as migrações do Postgres no banco assim que o container sobe
    db.Database.Migrate();
}

app.Run();