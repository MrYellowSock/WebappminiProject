// <snippet_UsingSystemTextJsonSerialization>
using System.Text.Json.Serialization;
// </snippet_UsingSystemTextJsonSerialization>
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.Models;

public class HomeIndex{
    public List<KeyValuePair<int,HostOrder>> Hosts = null!;
    public List<string> Aliases = null!;
    public Int32? UserId;
    public HomeIndex(List<KeyValuePair<int,HostOrder>> Hosts , List<string> Aliases ){
        this.Hosts = Hosts;
        this.Aliases = Aliases;
    }
    public HomeIndex(List<KeyValuePair<int,HostOrder>> Hosts , List<string> Aliases ,Int32 UserId):this(Hosts,Aliases){
        this.UserId = UserId;
    }
}