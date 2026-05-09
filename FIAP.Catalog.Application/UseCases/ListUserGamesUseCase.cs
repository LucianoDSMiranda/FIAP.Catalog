using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class ListUserGamesUseCase
{
    private readonly IUserGameRepository _userGameRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<ListUserGamesUseCase> _logger;

    public ListUserGamesUseCase(
        IUserGameRepository userGameRepository,
        IGameRepository gameRepository,
        ILogger<ListUserGamesUseCase> logger)
    {
        _userGameRepository = userGameRepository;
        _gameRepository = gameRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Game>> ExecuteAsync(Guid userId)
    {
        _logger.LogInformation("[Application][ListUserGamesUseCase] Listing games for user {UserId}", userId);
        var gameIds = await _userGameRepository.GetGameIdsByUserIdAsync(userId);
        return await _gameRepository.GetByIdsAsync(gameIds);
    }
}
