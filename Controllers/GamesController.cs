using Microsoft.AspNetCore.Mvc;
using FIAP.Catalog.Models;
using FIAP.Catalog.Data;
using MassTransit;
using FIAP.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FIAP.Catalog.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly CatalogDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public GamesController(CatalogDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Game>> GetGames()
    {
        return Ok(_context.Games.ToList());
    }

    [HttpGet("{id}")]
    public ActionResult<Game> GetGame(Guid id)
    {
        var game = _context.Games.Find(id);
        if (game == null)
        {
            return NotFound();
        }
        return Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame(Game game)
    {
        _context.Games.Add(game);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseGame(PurchaseRequest request)
    {
        var game = _context.Games.Find(request.GameId);
        if (game == null)
        {
            return NotFound("Game not found");
        }

        await _publishEndpoint.Publish(new OrderPlacedEvent
        {
            UserId = request.UserId,
            GameId = request.GameId,
            Price = game.Price,
            Email = request.Email
        });

        return Ok();
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetUserGames(Guid userId)
    {
        var gameIds = await _context.UserGames.Where(ug => ug.UserId == userId).Select(ug => ug.GameId).ToListAsync();
        var games = await _context.Games.Where(g => gameIds.Contains(g.Id)).ToListAsync();
        return Ok(games);
    }
}