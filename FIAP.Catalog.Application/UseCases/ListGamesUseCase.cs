using System.Text.Json;
using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class ListGamesUseCase
{
    private const string CacheKey = "catalog:games:all";

    private readonly IGameRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ListGamesUseCase> _logger;

    public ListGamesUseCase(IGameRepository repository, IDistributedCache cache, ILogger<ListGamesUseCase> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Game>> ExecuteAsync()
    {
        var cached = await _cache.GetStringAsync(CacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("[Application][ListGamesUseCase] Returning games from cache");
            return JsonSerializer.Deserialize<List<Game>>(cached) ?? new List<Game>();
        }

        _logger.LogInformation("[Application][ListGamesUseCase] Listing all games");
        var games = (await _repository.GetAllAsync()).ToList();

        await _cache.SetStringAsync(
            CacheKey,
            JsonSerializer.Serialize(games),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

        return games;
    }
}
