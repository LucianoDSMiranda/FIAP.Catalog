namespace FIAP.Catalog.Domain.Entities;

public class UserGame
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public DateTime PurchasedAt { get; set; }
}
