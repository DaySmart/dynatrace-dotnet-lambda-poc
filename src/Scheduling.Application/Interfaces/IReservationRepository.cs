using MongoDB.Driver;
using Scheduling.Domain.Entities;
using Scheduling.Domain.Enums;
using Scheduling.Domain.Models;

namespace Scheduling.Infrastructure.Interfaces;

public interface IReservationRepository
{
    Task<ReservationEntity> CreateAsync(ReservationEntity reservation);
    Task<List<ReservationEntity>> FilterAsync(FilterDefinition<ReservationEntity> filters);
    Task<ReservationEntity> GetByIdAsync(string id);
    Task<List<ReservationEntity>> QueryAsync(string productGroup, string account);
    Task<ReservationEntity> UpdateAsync(ReservationEntity reservation);
    Task<bool> RemoveAsync(string id);
    Task<ReservationEntity> UpdateStatusAsync(string id, ReservationStatus status);
}
