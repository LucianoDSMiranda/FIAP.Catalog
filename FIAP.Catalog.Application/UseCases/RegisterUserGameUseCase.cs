using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class RegisterUserGameUseCase
{
    private readonly IUserGameRepository _repository;
    private readonly ILogger<RegisterUserGameUseCase> _logger;

    public RegisterUserGameUseCase(IUserGameRepository repository, ILogger<RegisterUserGameUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid userId, Guid gameId)
    {
        _logger.LogInformation(
            "[Application][RegisterUserGameUseCase] Registering game {GameId} for user {UserId}",
            gameId, userId);

        var userGame = new UserGame
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GameId = gameId,
            PurchasedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(userGame);
    }
}
