using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class ListGamesUseCase
{
    private readonly IGameRepository _repository;
    private readonly ILogger<ListGamesUseCase> _logger;

    public ListGamesUseCase(IGameRepository repository, ILogger<ListGamesUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Game>> ExecuteAsync()
    {
        _logger.LogInformation("[Application][ListGamesUseCase] Listing all games");
        return await _repository.GetAllAsync();
    }
}
