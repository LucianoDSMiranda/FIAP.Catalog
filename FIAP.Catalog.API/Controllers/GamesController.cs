using FIAP.Catalog.Application.UseCases;
using FIAP.Catalog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.Catalog.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly ListGamesUseCase _listGames;
    private readonly GetGameByIdUseCase _getGameById;
    private readonly CreateGameUseCase _createGame;
    private readonly PurchaseGameUseCase _purchaseGame;
    private readonly ListUserGamesUseCase _listUserGames;

    public GamesController(
        ListGamesUseCase listGames,
        GetGameByIdUseCase getGameById,
        CreateGameUseCase createGame,
        PurchaseGameUseCase purchaseGame,
        ListUserGamesUseCase listUserGames)
    {
        _listGames = listGames;
        _getGameById = getGameById;
        _createGame = createGame;
        _purchaseGame = purchaseGame;
        _listUserGames = listUserGames;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetGames()
    {
        var games = await _listGames.ExecuteAsync();
        return Ok(games);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Game>> GetGame(Guid id)
    {
        var game = await _getGameById.ExecuteAsync(id);
        if (game == null) return NotFound();
        return Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame(Game game)
    {
        var created = await _createGame.ExecuteAsync(game);
        return CreatedAtAction(nameof(GetGame), new { id = created.Id }, created);
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseGame(PurchaseRequest request)
    {
        var ok = await _purchaseGame.ExecuteAsync(request);
        if (!ok) return NotFound("Game not found");
        return Ok();
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetUserGames(Guid userId)
    {
        var games = await _listUserGames.ExecuteAsync(userId);
        return Ok(games);
    }
}
