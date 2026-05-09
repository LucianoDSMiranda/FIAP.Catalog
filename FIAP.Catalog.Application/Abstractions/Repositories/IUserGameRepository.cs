using FIAP.Catalog.Domain.Entities;

namespace FIAP.Catalog.Application.Abstractions.Repositories;

public interface IUserGameRepository
{
    Task AddAsync(UserGame userGame);
    Task<IEnumerable<Guid>> GetGameIdsByUserIdAsync(Guid userId);
}
