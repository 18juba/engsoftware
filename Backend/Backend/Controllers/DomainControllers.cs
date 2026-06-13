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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin,Professor")]
    public async Task<IActionResult> VincularDisciplina(int professorId, int disciplinaId)
        => await _service.VincularDisciplinaAsync(professorId, disciplinaId)
            ? NoContent()
            : BadRequest(new { message = "Vínculo não foi possível (ids inválidos ou já existente)." });

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar([FromBody] DisciplinaCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin,Professor")]
    public async Task<IActionResult> Criar([FromBody] TurmaCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deletar(int id)
        => await _service.DeletarAsync(id) ? NoContent() : NotFound();
}

// ── Matriculas ───────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatriculasController : ControllerBase
{
    private readonly IMatriculaService _service;
    public MatriculasController(IMatriculaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] MatriculaCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Professor")]
    public async Task<IActionResult> AlterarStatus(int id, [FromBody] StatusMatricula status)
        => await _service.AlterarStatusAsync(id, status) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deletar(int id)
        => await _service.DeletarAsync(id) ? NoContent() : NotFound();
}

// ── Atividades ───────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AtividadesController : ControllerBase
{
    private readonly IAtividadeService _service;
    public AtividadesController(IAtividadeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync());

    [HttpGet("turma/{turmaId}")]
    public async Task<IActionResult> ListarPorTurma(int turmaId) =>
        Ok(await _service.ListarPorTurmaAsync(turmaId));

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<IActionResult> Criar([FromBody] AtividadeCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<IActionResult> Deletar(int id)
        => await _service.DeletarAsync(id) ? NoContent() : NotFound();
}

// ── Entregas ─────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EntregasController : ControllerBase
{
    private readonly IEntregaService _service;
    public EntregasController(IEntregaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync());

    [HttpGet("atividade/{atividadeId}")]
    public async Task<IActionResult> ListarPorAtividade(int atividadeId) =>
        Ok(await _service.ListarPorAtividadeAsync(atividadeId));

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Aluno")]
    public async Task<IActionResult> Criar([FromBody] EntregaCreateDto dto)
    {
        var result = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
    }

    [HttpPatch("{id}/nota")]
    [Authorize(Roles = "Professor,Admin")]
    public async Task<IActionResult> LancarNota(int id, [FromBody] EntregaNotaDto dto)
        => await _service.LancarNotaAsync(id, dto.Nota) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deletar(int id)
        => await _service.DeletarAsync(id) ? NoContent() : NotFound();
}
