using ArticulationAPI.DTOs;
using ArticulationAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArticulationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        try
        {
            return Ok(await _auth.RegisterAsync(dto));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            return Ok(await _auth.LoginAsync(dto));
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}