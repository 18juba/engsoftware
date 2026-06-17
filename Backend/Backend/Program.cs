using EscolaApi.Data;
using EscolaApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==================== CONFIGURAÇÕES ====================

// 1. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2. Turso Client
builder.Services.AddSingleton<TursoClient>();

// 3. JWT Authentication (Validação RIGOROSA ativada)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("A chave JWT não foi configurada nas variáveis de ambiente.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 🔒 Exige e valida a assinatura com a chave secreta de 32+ caracteres
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        // 🔒 Exige e valida quem emitiu o token (Issuer)
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        // 🔒 Exige e valida quem pode consumir o token (Audience)
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // 🔒 Exige e valida se o token já expirou
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2) // Pequena margem de tolerância para diferença de relógio entre os servidores
    };
});

// 4. Injeção de Dependência dos Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAlunoService, AlunoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProfessorService, ProfessorService>();
builder.Services.AddScoped<IDisciplinaService, DisciplinaService>();
builder.Services.AddScoped<ITurmaService, TurmaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 5. Configuração do Swagger Generator
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Escola API",
        Version = "v1",
        Description = "API do sistema de gestão escolar"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", null), new List<string>() }
    });
});

var app = builder.Build();

// ==================== MIDDLEWARES (PIPELINE) ====================

app.UseSwagger();

// 1ª Opção Visual: Interface Clássica do Swagger UI
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Escola API v1");
    options.RoutePrefix = "swagger";
});

// 2ª Opção Visual: Interface Moderna do Scalar
app.MapScalarApiReference(options =>
{
    options.Title = "Escola API - Documentação";
    options.Theme = ScalarTheme.Purple;
    options.OpenApiRoutePattern = "/swagger/v1/swagger.json";
});

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}));

app.UseCors("AllowAll");

// 🔒 Bloqueia as rotas de acordo com a configuração de autorização e JWT acima
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();