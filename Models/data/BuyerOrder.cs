// <snippet_UsingSystemTextJsonSerialization>
using System.Text.Json.Serialization;
// </snippet_UsingSystemTextJsonSerialization>
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.Models;

public class BuyerOrder : Order
{
    [BsonElement("Menu")]
    [JsonPropertyName("Menu")]
    public string Menu {get; set;} = null!;


    [BsonElement("Price")]
    [JsonPropertyName("Price")]
    public UInt32 Price {get; set;}
    
    [BsonElement("AttachedHostId")]
    [JsonPropertyName("AttachedHostId")]
    public string AttachedHostId {get; set;} = null!;

    [BsonElement("HostChecked")]
    [JsonPropertyName("HostChecked")]
    public bool HostChecked {get; set;}

    [BsonElement("BuyerChecked")]
    [JsonPropertyName("BuyerChecked")]
    public bool BuyerChecked {get; set;}
}
