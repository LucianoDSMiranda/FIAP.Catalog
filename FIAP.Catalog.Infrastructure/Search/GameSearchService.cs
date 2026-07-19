using Elastic.Clients.Elasticsearch;
using FIAP.Catalog.Application.Abstractions.Search;
using FIAP.Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FIAP.Catalog.Infrastructure.Search;

public class GameSearchService : IGameSearchService
{
    private const string IndexName = "games";

    private readonly ElasticsearchClient _client;
    private readonly ILogger<GameSearchService> _logger;

    public GameSearchService(ElasticsearchClient client, ILogger<GameSearchService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task IndexAsync(Game game)
    {
        try
        {
            await _client.IndexAsync(game, i => i.Index(IndexName).Id(game.Id.ToString()));
        }
        catch (Exception ex)
        {
            // Resiliência: falha ao indexar não deve derrubar a operação principal (create/update).
            _logger.LogError(ex, "[Infrastructure][GameSearchService] Falha ao indexar o jogo {GameId}", game.Id);
        }
    }

    public async Task<IEnumerable<Game>> SearchAsync(string query)
    {
        try
        {
            var response = await _client.SearchAsync<Game>(s => s
                .Index(IndexName)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(new[] { "name", "description" })
                        .Query(query)
                        .Fuzziness(new Fuzziness("AUTO")))));

            if (!response.IsValidResponse)
            {
                _logger.LogWarning("[Infrastructure][GameSearchService] Busca inválida para '{Query}'", query);
                return Enumerable.Empty<Game>();
            }

            // Documentos já retornam ordenados por relevância (_score) pelo Elasticsearch.
            return response.Documents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Infrastructure][GameSearchService] Falha na busca '{Query}'", query);
            return Enumerable.Empty<Game>();
        }
    }
}
