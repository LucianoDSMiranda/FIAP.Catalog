using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using FIAP.Catalog.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace FIAP.Catalog.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly MongoContext _context;
    private readonly ILogger<GameRepository> _logger;

    public GameRepository(MongoContext context, ILogger<GameRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        _logger.LogInformation("[Infrastructure][GameRepository] Fetching all games");
        return await _context.Games.Find(FilterDefinition<Game>.Empty).ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("[Infrastructure][GameRepository] Fetching game by id {GameId}", id);
        return await _context.Games.Find(g => g.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Game>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.ToList();
        _logger.LogInformation("[Infrastructure][GameRepository] Fetching {Count} games by ids", idList.Count);
        if (idList.Count == 0) return Enumerable.Empty<Game>();
        return await _context.Games.Find(g => idList.Contains(g.Id)).ToListAsync();
    }

    public async Task AddAsync(Game game)
    {
        _logger.LogInformation("[Infrastructure][GameRepository] Inserting game {GameId}", game.Id);
        await _context.Games.InsertOneAsync(game);
    }
}
