using Scheduling.Domain.Enums;
using Scheduling.Domain.Models;

namespace Scheduling.Application.Dto;

public class ReservationResponse
{
    public string Id { get; set; }

    public string Account { get; set; }
    public string ProductGroup { get; set; }

    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }

    public int Duration { get; set; }

    public string Timezone { get; set; }
    public int Spots { get; set; }
    public ReservationStatus Status { get; set; }
    public ReservationType Type { get; set; }

    public string[] Assignments { get; set; }

    public List<AssignmentSummary> AssignmentDetails { get; set; }
}
