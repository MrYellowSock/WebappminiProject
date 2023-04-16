using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services;
public class HostOrderService : OrderService<HostOrder>{
    public HostOrderService(IMongoCollection<HostOrder> col):base(col)
    {
    }
    
    public async Task<List<HostOrder>> FindAllNonClosed(long time) =>
        await this._collection.Find(x => time < x.Closed).ToListAsync();
    public async Task<HostOrder?> FindByBuyer(BuyerOrder buyerOrder) =>
        await this._collection.Find(x => x.Id == buyerOrder.AttachedHostId).FirstOrDefaultAsync();
}