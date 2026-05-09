using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using FIAP.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class PurchaseGameUseCase
{
    private readonly IGameRepository _gameRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PurchaseGameUseCase> _logger;

    public PurchaseGameUseCase(
        IGameRepository gameRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<PurchaseGameUseCase> logger)
    {
        _gameRepository = gameRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(PurchaseRequest request)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);
        if (game == null)
        {
            _logger.LogWarning("[Application][PurchaseGameUseCase] Game {GameId} not found", request.GameId);
            return false;
        }

        _logger.LogInformation(
            "[Application][PurchaseGameUseCase] Publishing OrderPlacedEvent for user {UserId}, game {GameId}",
            request.UserId, request.GameId);

        await _publishEndpoint.Publish(new OrderPlacedEvent
        {
            UserId = request.UserId,
            GameId = request.GameId,
            Price = game.Price,
            Email = request.Email
        });

        return true;
    }
}
