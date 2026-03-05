namespace E_Shop.Application.ServicesContract
{
    public interface IMemoryCacheService
    {
        Task<T> GetByAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
        Task<T> SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<T> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default) where T : class;

    }
}
