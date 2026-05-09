using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class CreateGameUseCase
{
    private readonly IGameRepository _repository;
    private readonly ILogger<CreateGameUseCase> _logger;

    public CreateGameUseCase(IGameRepository repository, ILogger<CreateGameUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Game> ExecuteAsync(Game game)
    {
        if (game.Id == Guid.Empty)
            game.Id = Guid.NewGuid();

        _logger.LogInformation("[Application][CreateGameUseCase] Creating game {GameId} {Name}", game.Id, game.Name);
        await _repository.AddAsync(game);
        return game;
    }
}
