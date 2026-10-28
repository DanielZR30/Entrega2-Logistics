using Logistics.Application.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _context;

        public Repository(DataContext context)
        {
            _context = context;
        }

        public Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Add(entity);
            return Task.FromResult(entity);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            T? entity = _context.Find<T>(id);
            if (entity != null)
            {
                _context.Remove(entity);
            }
            return Task.CompletedTask;
        }

        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            T? entity = _context.Find<T>(id);
            return Task.FromResult(entity);
        }

        public async Task<IEnumerable<T>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }

        public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Update(entity);
            return Task.FromResult(entity);
        }
    }
}
