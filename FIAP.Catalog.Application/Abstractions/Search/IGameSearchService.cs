using FIAP.Catalog.Domain.Entities;

namespace FIAP.Catalog.Application.Abstractions.Search;

public interface IGameSearchService
{
    Task IndexAsync(Game game);
    Task<IEnumerable<Game>> SearchAsync(string query);
}
