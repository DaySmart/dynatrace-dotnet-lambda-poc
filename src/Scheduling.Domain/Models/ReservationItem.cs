using Scheduling.Domain.Entities;
using Scheduling.Domain.Enums;

namespace Scheduling.Domain.Models
{
    public class ReservationItem
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
}
