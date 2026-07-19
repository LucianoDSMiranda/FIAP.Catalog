using FIAP.Catalog.Domain.Entities;

namespace FIAP.Catalog.Application.Abstractions.Repositories;

public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(Guid id);
    Task<IEnumerable<Game>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task AddAsync(Game game);
    Task UpdateAsync(Game game);
}
