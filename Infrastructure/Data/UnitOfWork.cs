using System.Collections;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _context;
        private Hashtable _repositories;
        public UnitOfWork(StoreContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            // Check if the hashtale is null
            if (_repositories == null) _repositories = new Hashtable();

            // Get the name of repository
            var type = typeof(TEntity).Name;

            // Check the repository if exists in the hashtable
            if (!_repositories.ContainsKey(type))
            {
                // Create a new instance of the repository
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType
                (typeof(TEntity)), _context);

                // Add to hash table
                _repositories.Add(type, repositoryInstance);
            }

            // return
            return (IGenericRepository<TEntity>) _repositories[type];
        }
    }
}