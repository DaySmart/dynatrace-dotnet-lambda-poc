namespace Scheduling.Reservation.Interfaces;

public interface IAvailabilityService
{
    Task<bool> ConfirmAvailability();
}
