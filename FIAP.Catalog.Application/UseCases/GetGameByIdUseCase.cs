using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class GetGameByIdUseCase
{
    private readonly IGameRepository _repository;
    private readonly ILogger<GetGameByIdUseCase> _logger;

    public GetGameByIdUseCase(IGameRepository repository, ILogger<GetGameByIdUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Game?> ExecuteAsync(Guid id)
    {
        _logger.LogInformation("[Application][GetGameByIdUseCase] Getting game {GameId}", id);
        return await _repository.GetByIdAsync(id);
    }
}
