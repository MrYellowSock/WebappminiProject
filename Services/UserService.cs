using BookStoreApi.Models;
using MongoDB.Driver;

namespace BookStoreApi.Services;
public class UserService : CollectionService<User>
{

    public UserService(IMongoCollection<User> col):base(col)
    {
    }
    
    public async Task<User?> FindByFId(Int32 id) =>
        await this._collection.Find(x => x.FriendlyId == id).FirstOrDefaultAsync();
    
    public async Task<User?> FindMaxFId() =>
        await this._collection.Find(x => true)
            .SortByDescending(x=>x.FriendlyId)
            .Limit(1)
            .FirstOrDefaultAsync();
    public async Task Update(Int32 id, User u) =>
        await this._collection.ReplaceOneAsync(x=>x.FriendlyId == id, u);

    public async Task Insert(User u) =>
        await this._collection.InsertOneAsync(u);
}