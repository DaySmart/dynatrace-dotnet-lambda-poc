using Scheduling.Domain.Enums;

namespace Scheduling.Domain.Models;

public class ReservationTypeFilter
{
    public ReservationTypeFilter(int type, List<string> keys)
    {
        Type = type;
        Keys = keys;
    }

    public int Type { get; set; }
    public List<string> Keys { get; set; }
}