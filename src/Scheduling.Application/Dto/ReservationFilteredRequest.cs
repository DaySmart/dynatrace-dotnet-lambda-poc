using System.ComponentModel.DataAnnotations;
using Scheduling.Domain.Enums;

namespace Scheduling.Application.Dto;

public class ReservationFilteredRequest
{
    [Required]
    public string ProductGroup { get; set; }
    [Required]
    public string Account { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }

    public List<string> Customers { get; set; }
    public List<string> Locations { get; set; }
    public List<string> Employees { get; set; }
    public List<string> Services { get; set; }
    public List<string> Resources { get; set; }
    public List<string> Rooms { get; set; }
    public List<string> Routes { get; set; }
    public ReservationStatus? Status { get; set; }
    public ReservationType? Type { get; set; }
}
