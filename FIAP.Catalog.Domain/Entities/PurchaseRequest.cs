namespace FIAP.Catalog.Domain.Entities;

public class PurchaseRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public string Email { get; set; } = string.Empty;
}
