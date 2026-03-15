using System.ComponentModel.DataAnnotations;

namespace FIAP.Catalog.Models;

public class PurchaseRequest
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public string Email { get; set; }
}