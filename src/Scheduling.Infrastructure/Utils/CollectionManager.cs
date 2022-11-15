using MongoDB.Driver;
using Scheduling.Domain.Entities;

namespace Scheduling.Infrastructure.Utils;

public class CollectionManager
{
    public static IMongoCollection<ReservationEntity> CreateReservationCollection(IMongoDatabase mongoDatabase,
        string collectionName)
    {
        if (null == mongoDatabase)
        {
            throw new ArgumentNullException(nameof(mongoDatabase), "Expected valid database reference to be provided.");
        }

        var collection = mongoDatabase.GetCollection<ReservationEntity>(collectionName);

        if (null == collection)
        {
            throw new InvalidOperationException(
                $"A failure occurred when creating the collection '{collectionName}'. Processing cannot continue.");
        }

        var lookupQueryBasic = new CreateIndexModel<ReservationEntity>(
            new IndexKeysDefinitionBuilder<ReservationEntity>()
                .Ascending(x => x.Account)
                .Ascending(x => x.Status)
                .Ascending("StartTime.DateTime"),
            new CreateIndexOptions() { Sparse = true }
        );

        collection.Indexes.CreateOne(lookupQueryBasic);


        var lookupQueryAdvanced = new CreateIndexModel<ReservationEntity>(
            new IndexKeysDefinitionBuilder<ReservationEntity>()
                .Ascending(x => x.Account)
                .Ascending(x => x.Status)
                .Ascending(x => x.Type)
                .Ascending("StartTime.DateTime")
                .Ascending(x => x.Assignments),
            new CreateIndexOptions() { Sparse = true }
        );

        collection.Indexes.CreateOne(lookupQueryAdvanced);

        // Add Additional Indexes Here

        return collection;
    }
}