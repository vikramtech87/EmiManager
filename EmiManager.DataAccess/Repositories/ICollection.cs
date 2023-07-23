namespace EmiManager.DataAccess.Repositories {
    public interface ICollection<T> where T : IIdentifiable {
        Task CreateAsync(T document);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetAsync(string id);
        Task RemoveAsync(string id);
        Task UpdateAsync(T document);
    }
}