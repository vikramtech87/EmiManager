using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;

namespace EmiManager.DataAccess.Repositories;

public class Collection<T> : ICollection<T> where T : IIdentifiable {
    private readonly IMongoCollection<T> _collection;

    public Collection(IMongoCollection<T> collection) {
        _collection = collection;
    }

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<T?> GetAsync(string id) =>
        await _collection.Find(d => d.Id == id).SingleOrDefaultAsync();

    public async Task CreateAsync(T document) => await _collection.InsertOneAsync(document);

    public async Task UpdateAsync(T document) =>
        await _collection.ReplaceOneAsync(d => d.Id == document.Id, document);

    public async Task RemoveAsync(string id) =>
        await _collection.DeleteOneAsync(d => d.Id == id);
}
