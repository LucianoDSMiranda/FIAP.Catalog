using System.Text.Json;
using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class GetGameByIdUseCase
{
    private readonly IGameRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetGameByIdUseCase> _logger;

    public GetGameByIdUseCase(IGameRepository repository, IDistributedCache cache, ILogger<GetGameByIdUseCase> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Game?> ExecuteAsync(Guid id)
    {
        var cacheKey = $"catalog:games:{id}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("[Application][GetGameByIdUseCase] Returning game {GameId} from cache", id);
            return JsonSerializer.Deserialize<Game>(cached);
        }

        _logger.LogInformation("[Application][GetGameByIdUseCase] Getting game {GameId}", id);
        var game = await _repository.GetByIdAsync(id);
        if (game is null)
            return null;

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(game),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

        return game;
    }
}
