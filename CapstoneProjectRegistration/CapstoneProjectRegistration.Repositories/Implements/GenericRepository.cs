using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectRegistration.Repositories.Implements;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly CapstoneDbContext _context;

    public GenericRepository(CapstoneDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
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
