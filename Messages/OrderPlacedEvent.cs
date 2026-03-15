namespace FIAP.Messages;

public class OrderPlacedEvent
{
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Price { get; set; }
    public string Email { get; set; }
}