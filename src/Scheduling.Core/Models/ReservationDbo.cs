using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Scheduling.Core.Enums;

namespace Scheduling.Core.Models;

public class Reservation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
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

    public List<AssignmentSumm> AssignmentDetails { get; set; }
}
