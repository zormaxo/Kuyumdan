using Core.Entities;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();

        Task<T> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> ListAllAsync();

        Task AddAsync(T entity);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<T> GetEntityWithSpec(ISpecification<T> spec);

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        Task<int> CountAsync(ISpecification<T> spec);

        Task<int> SaveChangesAsync();
    }
}