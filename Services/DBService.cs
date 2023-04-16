// <snippet_File>
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services;

public class DBService
{
    protected readonly IMongoDatabase db;

    // <snippet_ctor>
    public DBService(string url, string dbName) {
        var mongoClient = new MongoClient( url);
        var mongoDatabase = mongoClient.GetDatabase(dbName);
        this.db = mongoDatabase;
    }
    public IMongoCollection<T> GetCollection<T>(string name){
        return this.db.GetCollection<T>(name);
    }
}
// </snippet_File>
