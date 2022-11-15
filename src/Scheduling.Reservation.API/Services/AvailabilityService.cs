using Scheduling.Reservation.Interfaces;

namespace Scheduling.Reservation.Services;

public class AvailabilityService : IAvailabilityService
{
    public async Task<bool> ConfirmAvailability()
    {
        return await Task.FromResult(true);
    }
}
