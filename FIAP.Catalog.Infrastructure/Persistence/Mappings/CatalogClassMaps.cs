using FIAP.Catalog.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace FIAP.Catalog.Infrastructure.Persistence.Mappings;

public static class CatalogClassMaps
{
    private static bool _registered;
    private static readonly object _lock = new();

    public static void Register()
    {
        if (_registered) return;
        lock (_lock)
        {
            if (_registered) return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Game)))
            {
                BsonClassMap.RegisterClassMap<Game>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(g => g.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(UserGame)))
            {
                BsonClassMap.RegisterClassMap<UserGame>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(ug => ug.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            _registered = true;
        }
    }
}
