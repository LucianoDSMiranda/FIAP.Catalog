using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Application.Abstractions.Search;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class UpdateGameUseCase
{
    private readonly IGameRepository _repository;
    private readonly IGameSearchService _search;
    private readonly IDistributedCache _cache;
    private readonly ILogger<UpdateGameUseCase> _logger;

    public UpdateGameUseCase(
        IGameRepository repository,
        IGameSearchService search,
        IDistributedCache cache,
        ILogger<UpdateGameUseCase> logger)
    {
        _repository = repository;
        _search = search;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Game?> ExecuteAsync(Guid id, Game game)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return null;

        game.Id = id;

        _logger.LogInformation("[Application][UpdateGameUseCase] Updating game {GameId}", id);
        await _repository.UpdateAsync(game);

        await _search.IndexAsync(game);
        await _cache.RemoveAsync("catalog:games:all");
        await _cache.RemoveAsync($"catalog:games:{id}");

        return game;
    }
}
