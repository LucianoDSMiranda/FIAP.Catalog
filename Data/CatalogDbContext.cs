using FIAP.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace FIAP.Catalog.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Game> Games { get; set; }
    public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
    public DbSet<UserGame> UserGames { get; set; }
}