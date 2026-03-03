using E_Shop.Domain.RepositoryContract;
using E_Shop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_Shop.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly EShopDbContext _db;

        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        public UnitOfWork(EShopDbContext db)
        {
            _db = db;
        }
        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IGenericRepository<T>)_repositories[typeof(T)];
            }
            var repository = new GenericRepository<T>(_db);
            _repositories.Add(typeof(T), repository);

            return repository;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _db.Database.CommitTransactionAsync();
        }

        public async Task<int> CompleteAsync()
        {
            return await _db.SaveChangesAsync();
        }
        public async Task RollbackTransactionAsync()
        {
            await _db.Database.RollbackTransactionAsync();
        }
        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
