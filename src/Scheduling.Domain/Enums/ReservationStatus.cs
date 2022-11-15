namespace Scheduling.Domain.Enums;

public enum ReservationStatus
{
    Processing = 0,
    Open = 1,
    CheckedIn = 2,
    ClosedAndEditing = 3,
    Closed = 4,
    Cancelled = 5,
    Voided = 6,
    NoShow = 7,
    Pending = 8,
    Rejected = 9
}
