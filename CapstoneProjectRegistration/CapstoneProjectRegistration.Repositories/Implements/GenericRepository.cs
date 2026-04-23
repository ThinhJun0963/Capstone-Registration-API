using System.Linq.Expressions;
using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CapstoneProjectRegistration.Repositories.Implements;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly CapstoneDbContext _context;
    public readonly DbSet<T> _db;

    public GenericRepository(CapstoneDbContext context)
    {
        _context = context;
        _db = _context.Set<T>();
    }
    public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {

        IQueryable<T> query = _db;
        return await query.FirstOrDefaultAsync(filter);

    }
    public async Task<T> GetAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {

        IQueryable<T> query = _db;
        if (include != null)
        {
            query = include(query);
        }
        return await query.FirstOrDefaultAsync(filter);

    }
    public async Task RemoveByIdAsync(object id)
    {
#nullable disable
        T existing = await _db.FindAsync(id);
#nullable restore
        if (existing != null)
        {
            _db.Remove(existing);
        }
        else throw new Exception();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }
    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter)
    {
        if (filter != null)
        {
            return await _db.Where(filter).ToListAsync();
        }
        return await _db.ToListAsync();
    }

    public async Task<List<T>> GetAllWithIncludeAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>> include)
    {
        IQueryable<T> query = _db;
        query = include(query);
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}
