using FIAP.Catalog.Application.UseCases;
using FIAP.Messages;
using FIAP.Messages.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Infrastructure.Messaging;

public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly RegisterUserGameUseCase _registerUserGame;
    private readonly ILogger<PaymentProcessedEventConsumer> _logger;

    public PaymentProcessedEventConsumer(
        RegisterUserGameUseCase registerUserGame,
        ILogger<PaymentProcessedEventConsumer> logger)
    {
        _registerUserGame = registerUserGame;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var message = context.Message;

        if (message.Status == PaymentStatus.Approved)
        {
            await _registerUserGame.ExecuteAsync(message.UserId, message.GameId);
            _logger.LogInformation(
                "Game {GameId} added to user {UserId}'s library.",
                message.GameId, message.UserId);
        }
        else
        {
            _logger.LogWarning(
                "Payment rejected for user {UserId}, game {GameId}.",
                message.UserId, message.GameId);
        }
    }
}
