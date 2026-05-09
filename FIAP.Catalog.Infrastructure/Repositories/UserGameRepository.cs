using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using FIAP.Catalog.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace FIAP.Catalog.Infrastructure.Repositories;

public class UserGameRepository : IUserGameRepository
{
    private readonly MongoContext _context;
    private readonly ILogger<UserGameRepository> _logger;

    public UserGameRepository(MongoContext context, ILogger<UserGameRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(UserGame userGame)
    {
        _logger.LogInformation(
            "[Infrastructure][UserGameRepository] Inserting userGame {UserId} -> {GameId}",
            userGame.UserId, userGame.GameId);

        await _context.UserGames.InsertOneAsync(userGame);
    }

    public async Task<IEnumerable<Guid>> GetGameIdsByUserIdAsync(Guid userId)
    {
        _logger.LogInformation(
            "[Infrastructure][UserGameRepository] Fetching game ids for user {UserId}",
            userId);

        var userGames = await _context.UserGames.Find(ug => ug.UserId == userId).ToListAsync();
        return userGames.Select(ug => ug.GameId);
    }
}
