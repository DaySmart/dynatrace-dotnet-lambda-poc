using Scheduling.Domain.Enums;
using Scheduling.Domain.Models;

namespace Scheduling.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationItem> CreateAsync(ReservationItem reservation);
    Task<ReservationItem> GetByIdAsync(string id);
    Task<ReservationItem> UpdateStatusAsync(string id, ReservationStatus status);
    Task RemoveAsync(string id);
    Task<List<ReservationItem>> FilterAsync();
}
