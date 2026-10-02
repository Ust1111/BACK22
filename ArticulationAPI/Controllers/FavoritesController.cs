using System.Security.Claims;
using ArticulationAPI.DTOs;
using ArticulationAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ArticulationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FavoritesService _favorites;

    public FavoritesController(FavoritesService favorites)
    {
        _favorites = favorites;
    }

    private Guid UserId => Guid.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<FavoritePairDto>>> GetAll()
        => Ok(await _favorites.GetAllAsync(UserId));
    
    [HttpGet("query")]
    [EnableQuery]
    public ActionResult<IQueryable<Models.FavoritePair>> GetAllQuery()
        => Ok(_favorites.GetAllQueryable(UserId));

    [HttpPost]
    public async Task<ActionResult<FavoritePairDto>> Add([FromBody] FavoritePairDto dto)
        => Accepted(await _favorites.AddAsync(UserId, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _favorites.DeleteAsync(UserId, id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}