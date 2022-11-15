using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Scheduling.Domain.Entities;
using Scheduling.Domain.Enums;
using Scheduling.Domain.Models;
using Scheduling.Infrastructure.Interfaces;

namespace Scheduling.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly IMongoCollection<ReservationEntity> _collection;

    public ReservationRepository(IMongoCollection<ReservationEntity> collection)
    {
        _collection = collection;
    }

    public async Task<ReservationEntity> CreateAsync(ReservationEntity reservationDbo)
    {
        await _collection.InsertOneAsync(reservationDbo);
        return reservationDbo;
    }
    
    public async Task<List<ReservationEntity>> CreateAsync(List<ReservationEntity> reservations)
    {
        await _collection.InsertManyAsync(reservations);
        return reservations;
    }

    public async Task<List<ReservationEntity>> GetAsync(string productGroup, string account)
    {
        return await _collection.Find(x => x.Account == account).ToListAsync();
    }

    public async Task<List<ReservationEntity>> FilterAsync(FilterDefinition<ReservationEntity> filters)
    {
        var serializerRegistry = BsonSerializer.SerializerRegistry;
        var documentSerializer = serializerRegistry.GetSerializer<ReservationEntity>();
        Console.WriteLine("filters.Render(documentSerializer, serializerRegistry).ToString()");
        Console.WriteLine(filters.Render(documentSerializer, serializerRegistry).ToString());
        var deletedFilter = Builders<ReservationEntity>.Filter.Ne("Status", ReservationStatus.Cancelled);
        var idAndStateFilter = Builders<ReservationEntity>.Filter.And(new[] { filters, deletedFilter });

        var results = await _collection.Find(filters).ToListAsync();

        return results;
    }

    public async Task<ReservationEntity> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<ReservationEntity>> QueryAsync(string productGroup, string account)
    {
        return await _collection.Find(x => x.ProductGroup == productGroup && x.Account == account).ToListAsync();
    }

    Task<ReservationEntity> IReservationRepository.UpdateAsync(ReservationEntity reservation)
    {
        throw new NotImplementedException();
    }

    Task<bool> IReservationRepository.RemoveAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(ReservationEntity reservationDbo) =>
        await _collection.ReplaceOneAsync(x => x.Id == reservationDbo.Id, reservationDbo);

    public async Task RemoveAsync(string id)
    {
        await UpdateStatusAsync(id, ReservationStatus.Cancelled);
    }

    public async Task<ReservationEntity> UpdateStatusAsync(string id, ReservationStatus status)
    {
        // Check if valid ReservationStatus
        if (!Enum.IsDefined(typeof(ReservationStatus), status)) return await Task.FromResult<ReservationEntity>(null!);

        var filter = Builders<ReservationEntity>.Filter.Eq("Id", id);
        var update = Builders<ReservationEntity>.Update.Set("Status", status);
        var result = await _collection.UpdateOneAsync(filter, update);
        if (result == null)
        {
            return await Task.FromResult<ReservationEntity>(null!);
        }

        var records = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return records;
    }
}
