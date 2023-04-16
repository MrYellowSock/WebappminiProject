// <snippet_UsingSystemTextJsonSerialization>
using System.Text.Json.Serialization;
// </snippet_UsingSystemTextJsonSerialization>
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.Models;

public class HostOrder : Order
{    
    [BsonElement("TargetLocation")]
    [JsonPropertyName("TargetLocation")]
    public string TargetLocation {get; set;} = null!;

    [BsonElement("Closed")]
    [JsonPropertyName("Closed")]
    public long Closed {get; set;}

    [BsonElement("Limit")]
    [JsonPropertyName("Limit")]
    public Int32 Limit {get; set;}
}
