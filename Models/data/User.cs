// <snippet_UsingSystemTextJsonSerialization>
using System.Text.Json.Serialization;
// </snippet_UsingSystemTextJsonSerialization>
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("FriendlyId")]
    [JsonPropertyName("FriendlyId")]
    public Int32 FriendlyId {get; set;}

    [BsonElement("LastActive")]
    [JsonPropertyName("LastActive")]
    public long LastActive {get; set;}

    [BsonElement("Created")]
    [JsonPropertyName("Created")]
    public long Created {get; set;}
}
