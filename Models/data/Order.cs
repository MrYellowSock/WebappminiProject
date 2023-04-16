// <snippet_UsingSystemTextJsonSerialization>
using System.Text.Json.Serialization;
// </snippet_UsingSystemTextJsonSerialization>
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("OwnerUserId")]
    [JsonPropertyName("OwnerUserId")]
    public Int32 OwnerUserId {get; set;}

    [BsonElement("OwnerName")]
    [JsonPropertyName("OwnerName")]
    public string OwnerName {get; set;} = null!;

    [BsonElement("OwnerContact")]
    [JsonPropertyName("OwnerContact")]
    public string OwnerContact {get; set;} = null!;

    [BsonElement("Location")]
    [JsonPropertyName("Location")]
    public string Location {get; set;} = null!;

    [BsonElement("Completed")]
    [JsonPropertyName("Completed")]
    public long Completed {get; set;}

    [BsonElement("Created")]
    [JsonPropertyName("Created")]
    public long Created {get; set;}
}
