using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CapstoneProjectRegistration.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetAsync(Expression<Func<T, bool>> filter);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter);
    Task<List<T>> GetAllWithIncludeAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>> include);
    Task<T> GetAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include);

    Task RemoveByIdAsync(object id);
    Task<T?> GetByIdAsync(int id);

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
}
