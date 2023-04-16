using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services;
public class OrderService<T> : CollectionService<T> where T :Order
{

    public OrderService(IMongoCollection<T> col):base(col)
    {
    }
    
    public async Task Delete(string id) =>
        await this._collection.DeleteManyAsync(x=>x.Id == id);
    public async Task<List<T>> FindAll() =>
        await this._collection.Find(x => true).ToListAsync();
    public async Task<List<T>> FindByOwner(Int32 ownerId) =>
        await this._collection.Find(x => x.OwnerUserId == ownerId).ToListAsync();
    
    public async Task<List<T>> FindUnfinished(Int32 ownerId) =>
        await this._collection.Find(x => x.OwnerUserId == ownerId && x.Completed == 0).ToListAsync();
    public async Task<T?> FindById(string id) =>
        await this._collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task Insert(T item) =>
        await this._collection.InsertOneAsync(item);

    public async Task UpdateAsync(string id, T replacement) =>
        await this._collection.ReplaceOneAsync(x => x.Id == id, replacement);

    public async Task RemoveAsync(string id) =>
        await this._collection.DeleteOneAsync(x => x.Id == id);
}
// </snippet_File>
