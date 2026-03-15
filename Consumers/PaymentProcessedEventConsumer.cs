using FIAP.Catalog.Data;
using FIAP.Catalog.Models;
using FIAP.Messages;
using FIAP.Messages.Enums;
using MassTransit;

namespace FIAP.Catalog.Consumers;

public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly CatalogDbContext _context;

    public PaymentProcessedEventConsumer(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var message = context.Message;

        if (message.Status == PaymentStatus.Approved)
        {
            var userId = message.UserId;
            var userGame = new UserGame
            {
                UserId = userId,
                GameId = message.GameId,
                PurchasedAt = DateTime.UtcNow
            };
            _context.UserGames.Add(userGame);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Game {message.GameId} added to user {userId}'s library.");
        }
        else
        {
            Console.WriteLine($"Payment rejected for user {message.UserId}, game {message.GameId}.");
        }
    }
}