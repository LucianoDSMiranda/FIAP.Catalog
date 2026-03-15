using System.ComponentModel.DataAnnotations;

namespace FIAP.Catalog.Models;

public class Game
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}