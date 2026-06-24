using EscolaApi.Data;
using EscolaApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Prometheus;
using Scalar.AspNetCore;
using Serilog;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

// ==================== SERILOG (deve ser o primeiro) ====================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando aplicação...");

    var builder = WebApplication.CreateBuilder(args);

    // Substitui o logger padrão pelo Serilog
    builder.Host.UseSerilog((ctx, services, config) =>
        config.ReadFrom.Configuration(ctx.Configuration)
              .ReadFrom.Services(services)
              .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

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

    // 3. JWT Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("A chave JWT não foi configurada nas variáveis de ambiente.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

    // 4. Injeção de Dependência dos Services
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IAlunoService, AlunoService>();
    builder.Services.AddScoped<IUsuarioService, UsuarioService>();
    builder.Services.AddScoped<IProfessorService, ProfessorService>();
    builder.Services.AddScoped<IDisciplinaService, DisciplinaService>();
    builder.Services.AddScoped<ITurmaService, TurmaService>();

    // 5. Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy("API no ar"))
        .AddCheck("turso", () =>
        {
            var url = builder.Configuration["Turso:DatabaseUrl"];
            var token = builder.Configuration["Turso:AuthToken"];
            return !string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(token)
                ? HealthCheckResult.Healthy("Configuração do Turso OK")
                : HealthCheckResult.Unhealthy("Configurações do Turso ausentes");
        }, tags: ["ready", "db"]);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // 6. Swagger
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

    // Loga cada requisição HTTP automaticamente
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "{RequestMethod} {RequestPath} → {StatusCode} ({Elapsed:0.000}ms)";
    });

    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Escola API v1");
        options.RoutePrefix = "swagger";
    });

    app.MapScalarApiReference(options =>
    {
        options.Title = "Escola API - Documentação";
        options.Theme = ScalarTheme.Purple;
        options.OpenApiRoutePattern = "/swagger/v1/swagger.json";
    });

    // Health Check simples — retorna "healthy" se a API está no ar
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            var result = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                environment = app.Environment.EnvironmentName,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description
                })
            });
            await context.Response.WriteAsync(result);
        }
    });

    // Health Check de prontidão — verifica dependências (banco, etc.)
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            var result = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description
                })
            });
            await context.Response.WriteAsync(result);
        }
    });

    // Métricas Prometheus — coleta contadores HTTP automaticamente
    app.UseHttpMetrics();
    app.MapMetrics(); // expõe em /metrics

    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}