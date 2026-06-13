using EscolaApi.DTOs;
using EscolaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EscolaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Autenticar usuário e obter token JWT</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new { message = "Email ou senha inválidos." });
        return Ok(result);
    }
}
