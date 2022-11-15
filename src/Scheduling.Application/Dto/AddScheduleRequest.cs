using Scheduling.Domain.Enums;

namespace Scheduling.Application.Dto;

public class AddScheduleRequest
{
    public ScheduleHoursType Type { get; set; }

    public int AvailableSpots { get; set; }
    public int Day { get; set; }
    public int StartTime { get; set; }
    public int EndTime { get; set; }
}
