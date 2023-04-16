using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services;
public class BuyerOrderService : OrderService<BuyerOrder>{
    public BuyerOrderService(IMongoCollection<BuyerOrder> col):base(col)
    {
    }
    
    public async Task<List<BuyerOrder>> FindByHost(string hostOrderId) =>
        await this._collection.Find(x => x.AttachedHostId == hostOrderId).ToListAsync();
    public async Task<List<BuyerOrder>> FindByHostUnfinished(string hostOrderId) =>
        await this._collection.Find(x => x.AttachedHostId == hostOrderId && x.Completed == 0).ToListAsync();
}