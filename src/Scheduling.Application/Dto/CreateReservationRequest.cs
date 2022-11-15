using System.ComponentModel.DataAnnotations;
using Scheduling.Domain.Models;using Scheduling.Domain.Enums;


namespace Scheduling.Application.Dto;

public class CreateReservationRequest
{
    [Required]
    public string Account { get; set; }
    [Required]
    public string ProductGroup { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public int Duration { get; set; }
    public int Spots { get; set; }

    public ReservationType Type { get; set; }

    public string[] Assignments { get; set; }

    public List<AssignmentSummary> AssignmentDetails { get; set; }
}
