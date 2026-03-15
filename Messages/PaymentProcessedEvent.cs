using FIAP.Messages.Enums;

namespace FIAP.Messages;

public class PaymentProcessedEvent
{
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Price { get; set; }
    public PaymentStatus Status { get; set; }
}
