using System.ComponentModel.DataAnnotations;

namespace FIAP.Catalog.Domain.Entities;

public class Game
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
