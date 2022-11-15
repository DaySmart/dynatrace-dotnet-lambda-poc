namespace Scheduling.Reservation.Models;

public class ReservationDatabaseSettings
{
    public const string SettingsKey = @"ReservationDatabase";
    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string CollectionName { get; set; }
}
