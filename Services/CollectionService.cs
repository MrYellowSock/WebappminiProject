// <snippet_File>
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services;

public class CollectionService<T>
{
    protected readonly IMongoCollection<T> _collection;

    // <snippet_ctor>
    public CollectionService(IMongoCollection<T> col){
        this._collection = col;
    }
}
// </snippet_File>
