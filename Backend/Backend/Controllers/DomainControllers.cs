using EscolaApi.DTOs;
using EscolaApi.Models;
using EscolaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscolaApi.Controllers;

// ── Usuarios ─────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;
    public UsuariosController(IUsuarioService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Criar([FromBody] UsuarioCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
        => await _service.DeletarAsync(id) ? NoContent() : NotFound();
}

// ── Alunos ───────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlunosController : ControllerBase
{
    private readonly IAlunoService _service;
    public AlunosController(IAlunoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Criar([FromBody] AlunoCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
        => await _service.DeletarAsync(id) ? NoContent() : NotFound();
}

// ── Professores ──────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfessoresController : ControllerBase
{
    private readonly IProfessorService _service;
    public ProfessoresController(IProfessorService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Criar([FromBody] ProfessorCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpPost("{professorId}/disciplinas/{disciplinaId}")]
    public async Task<IActionResult> VincularDisciplina(int professorId, int disciplinaId)
        => await _service.VincularDisciplinaAsync(professorId, disciplinaId)
            ? NoContent()
            : BadRequest(new { message = "Vínculo não foi possível (ids inválidos ou já existente)." });

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
        => await _service.DeletarAsync(id) ? NoContent() : NotFound();
}

// ── Disciplinas ──────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DisciplinasController : ControllerBase
{
    private readonly IDisciplinaService _service;
    public DisciplinasController(IDisciplinaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] DisciplinaCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
        => await _service.DeletarAsync(id) ? NoContent() : NotFound();
}

// ── Turmas ───────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurmasController : ControllerBase
{
    private readonly ITurmaService _service;
    public TurmasController(ITurmaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TurmaCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
        => await _service.DeletarAsync(id) ? NoContent() : NotFound();
}