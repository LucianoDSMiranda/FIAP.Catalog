using FIAP.Catalog.Application.Abstractions.Search;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Application.UseCases;

public class SearchGamesUseCase
{
    private readonly IGameSearchService _search;
    private readonly ILogger<SearchGamesUseCase> _logger;

    public SearchGamesUseCase(IGameSearchService search, ILogger<SearchGamesUseCase> logger)
    {
        _search = search;
        _logger = logger;
    }

    public async Task<IEnumerable<Game>> ExecuteAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<Game>();

        _logger.LogInformation("[Application][SearchGamesUseCase] Searching games '{Query}'", query);
        return await _search.SearchAsync(query);
    }
}
