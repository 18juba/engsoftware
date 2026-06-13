using EscolaApi.Data;
using EscolaApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Conexão com o Banco de Dados (Detecta se está no Docker ou Local)
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    // Se estiver rodando local fora do Docker, usa o appsettings.json (localhost)
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Serviços de negócio
builder.Services.AddScoped<IAlunoService, AlunoService>();

// 3. JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 4. CONFIGURAÇÃO DO SWAGGER PARA ACEITAR JWT NOS TESTES (.NET 10 / SWASHBUCKLE 10+)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Escola API - Backend de Negócios", Version = "v1" });

    // 1. Define o esquema de segurança (Botão Authorize) usando a estrutura correta do .NET 10
    var securitySchemeReference = new OpenApiSecuritySchemeReference("Bearer", null);

    // Registra a definição de segurança no Swagger usando o formato que a v10 exige
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta forma: Bearer {seu_token_aqui}"
    });

    // 2. CORREÇÃO .NET 10: Usando a função Lambda com OpenApiSecuritySchemeReference
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            securitySchemeReference,
            new List<string>()
        }
    });
});

var app = builder.Build();

// 5. Migrações automáticas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Escola API v1");
    c.RoutePrefix = "swagger";
});

// 6. Ordem obrigatória
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();