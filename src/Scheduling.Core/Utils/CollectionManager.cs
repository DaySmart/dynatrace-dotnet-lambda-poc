using MongoDB.Driver;
using Scheduling.Core.Models;
using Scheduling.Core.Models.Availability;

namespace Scheduling.Core.Utils;

public class CollectionManager
{
    public static IMongoCollection<ReservationE> CreateReservationCollection(IMongoDatabase mongoDatabase,
        string collectionName)
    {
        if (null == mongoDatabase)
        {
            throw new ArgumentNullException(nameof(mongoDatabase), "Expected valid database reference to be provided.");
        }
        
        var collection = mongoDatabase.GetCollection<ReservationDbo>(collectionName);
        
        if (null == collection)
        {
            throw new InvalidOperationException(
                $"A failure occurred when creating the collection '{collectionName}'. Processing cannot continue.");
        }
        
        var lookupQueryBasic = new CreateIndexModel<ReservationDbo>(new IndexKeysDefinitionBuilder<ReservationDbo>()
                .Ascending(x => x.Account)
                .Ascending(x => x.Status)
                .Ascending("StartTime.DateTime"),
                new CreateIndexOptions() { Sparse = true }
        );

        collection.Indexes.CreateOne(lookupQueryBasic);
        
        
        var lookupQueryAdvanced = new CreateIndexModel<ReservationDbo>(new IndexKeysDefinitionBuilder<ReservationDbo>()
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
    
    public static IMongoCollection<AssignmentDbo> CreateAvailabilityCollection(IMongoDatabase mongoDatabase,
        string collectionName)
    {
        if (null == mongoDatabase)
        {
            throw new ArgumentNullException(nameof(mongoDatabase), "Expected valid database reference to be provided.");
        }
        
        var collection = mongoDatabase.GetCollection<AssignmentDbo>(collectionName);
        
        if (null == collection)
        {
            throw new InvalidOperationException(
                $"A failure occurred when creating the collection '{collectionName}'. Processing cannot continue.");
        }
        
        // Add Additional Indexes Here
        
        return collection;
    }
}