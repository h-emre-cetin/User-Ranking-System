using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class LeaderBoardRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("user_id")]
        public string UserId { get; set; }
        [BsonElement("rank")]
        public int Rank { get; set; }
        [BsonElement("total_point")]
        public int TotalPoint { get; set; }
        [BsonElement("month")]
        public int Month { get; set; }
        [BsonElement("prize")]
        public string Prize { get; set; } = String.Empty;
    }
}
